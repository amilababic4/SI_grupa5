(function () {
    "use strict";
    if (!Auth.guard()) return;
    const user = Auth.currentUser();
    const postId = Number(Common.qs("id"));
    const post = DB.find("forumObjave", postId);
    if (!post) { document.querySelector("main").innerHTML = "<p>Objava nije pronađena.</p>"; return; }

    function renderPost() {
        const autor = DB.find("korisnici", post.korisnikId);
        const reakcije = DB.query("forumReakcije", (r) => r.objavaId === postId);
        const jaReagovao = reakcije.some((r) => r.korisnikId === user.id);
        document.getElementById("post-container").innerHTML = `
            <div style="font-size:.75rem;font-weight:700;color:var(--sl-blue);text-transform:uppercase;">${Common.escapeHtml(post.kategorija)}${post.zakljucana ? " · Zaključano" : ""}</div>
            <h1>${Common.escapeHtml(post.naslov)}</h1>
            <div style="color:var(--sl-muted);font-size:.85rem;margin-bottom:1rem;">${autor ? Common.escapeHtml(autor.ime + " " + autor.prezime) : "Korisnik"} (${autor ? Common.escapeHtml(autor.uloga) : ""}) · ${Common.formatDate(post.datumKreiranja)}</div>
            <p>${Common.escapeHtml(post.sadrzaj)}</p>
            <div style="display:flex;gap:.5rem;margin-top:1rem;flex-wrap:wrap;">
                <button class="btn ${jaReagovao ? "btn-primary" : "btn-secondary"} btn-sm" id="react-btn">👍 Korisno (${reakcije.length})</button>
                ${Auth.isStaff() ? `<button class="btn btn-danger btn-sm" id="delete-post-btn">Obriši objavu</button>` : ""}
                ${!Auth.isStaff() && post.korisnikId !== user.id ? `<button class="btn btn-secondary btn-sm" id="report-post-btn">Prijavi</button>` : ""}
            </div>
        `;
        document.getElementById("react-btn").addEventListener("click", () => {
            const existing = DB.query("forumReakcije", (r) => r.objavaId === postId && r.korisnikId === user.id)[0];
            if (existing) DB.remove("forumReakcije", existing.id);
            else DB.insert("forumReakcije", { tip: "korisno", datumKreiranja: new Date().toISOString().slice(0, 10), objavaId: postId, korisnikId: user.id });
            renderPost();
        });
        if (document.getElementById("delete-post-btn")) {
            document.getElementById("delete-post-btn").addEventListener("click", () => {
                if (!confirm("Obrisati ovu objavu i sve komentare?")) return;
                DB.query("forumKomentari", (k) => k.objavaId === postId).forEach((k) => DB.remove("forumKomentari", k.id));
                DB.remove("forumObjave", postId);
                window.location.href = "index.html";
            });
        }
        if (document.getElementById("report-post-btn")) {
            document.getElementById("report-post-btn").addEventListener("click", () => {
                if (DB.query("forumObjavaPrijave", (p) => p.objavaId === postId && p.prijavioKorisnikId === user.id).length) { alert("Već ste prijavili ovu objavu."); return; }
                const razlog = prompt("Razlog prijave:");
                if (razlog === null) return;
                DB.insert("forumObjavaPrijave", { objavaId: postId, prijavioKorisnikId: user.id, razlog, datumKreiranja: new Date().toISOString().slice(0, 10), status: "otvorena", razrijesioKorisnikId: null, datumRazrjesenja: null });
                alert("Objava je prijavljena.");
            });
        }
    }

    function renderComments() {
        const komentari = DB.query("forumKomentari", (k) => k.objavaId === postId).sort((a, b) => a.datumKreiranja.localeCompare(b.datumKreiranja));
        const el = document.getElementById("comments-list");
        el.innerHTML = komentari.length ? komentari.map((k) => {
            const autor = DB.find("korisnici", k.korisnikId);
            return `<div class="comment-item">
                <div style="font-weight:700;">${autor ? Common.escapeHtml(autor.ime + " " + autor.prezime) : "Korisnik"} <span style="font-weight:400;color:var(--sl-muted);font-size:.8rem;">${Common.formatDate(k.datumKreiranja)}</span></div>
                <p>${Common.escapeHtml(k.sadrzaj)}</p>
                <div style="display:flex;gap:.5rem;">
                    ${Auth.isStaff() ? `<button class="btn btn-danger btn-sm" data-del-comment="${k.id}">Obriši</button>` : ""}
                    ${!Auth.isStaff() && k.korisnikId !== user.id ? `<button class="btn btn-secondary btn-sm" data-report-comment="${k.id}">Prijavi</button>` : ""}
                </div>
            </div>`;
        }).join("") : "<p style='color:var(--sl-muted);'>Nema komentara.</p>";

        el.querySelectorAll("[data-del-comment]").forEach((btn) => btn.addEventListener("click", () => {
            if (!confirm("Obrisati ovaj komentar?")) return;
            DB.remove("forumKomentari", Number(btn.getAttribute("data-del-comment")));
            renderComments();
        }));
        el.querySelectorAll("[data-report-comment]").forEach((btn) => btn.addEventListener("click", () => {
            const kId = Number(btn.getAttribute("data-report-comment"));
            if (DB.query("forumKomentarPrijave", (p) => p.komentarId === kId && p.prijavioKorisnikId === user.id).length) { alert("Već ste prijavili ovaj komentar."); return; }
            const razlog = prompt("Razlog prijave:");
            if (razlog === null) return;
            DB.insert("forumKomentarPrijave", { komentarId: kId, prijavioKorisnikId: user.id, razlog, datumKreiranja: new Date().toISOString().slice(0, 10), status: "otvorena", razrijesioKorisnikId: null, datumRazrjesenja: null });
            alert("Komentar je prijavljen.");
        }));
    }

    if (post.zakljucana) {
        document.getElementById("comment-form").style.display = "none";
        document.getElementById("locked-msg").style.display = "block";
    } else {
        document.getElementById("comment-form").addEventListener("submit", (e) => {
            e.preventDefault();
            DB.insert("forumKomentari", {
                sadrzaj: document.getElementById("comment-text").value.trim(),
                datumKreiranja: new Date().toISOString().slice(0, 10),
                objavaId: postId,
                korisnikId: user.id,
            });
            document.getElementById("comment-text").value = "";
            renderComments();
        });
    }

    renderPost();
    renderComments();
})();
