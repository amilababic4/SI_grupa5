(function () {
    "use strict";
    if (!Auth.guard(["Bibliotekar", "Administrator"])) return;
    const staff = Auth.currentUser();

    function render() {
        const objavaPrijave = DB.query("forumObjavaPrijave", (p) => p.status === "otvorena").map((p) => {
            const objava = DB.find("forumObjave", p.objavaId);
            const prijavio = DB.find("korisnici", p.prijavioKorisnikId);
            return { tip: "Forum objava", naslov: objava ? objava.naslov : "—", prijavio, razlog: p.razlog, datum: p.datumKreiranja, source: "forum-objava", id: p.id, targetId: p.objavaId, link: objava ? `../forum/details.html?id=${objava.id}` : null };
        });
        const komentarPrijave = DB.query("forumKomentarPrijave", (p) => p.status === "otvorena").map((p) => {
            const komentar = DB.find("forumKomentari", p.komentarId);
            const objava = komentar ? DB.find("forumObjave", komentar.objavaId) : null;
            const prijavio = DB.find("korisnici", p.prijavioKorisnikId);
            return { tip: "Forum komentar", naslov: komentar ? komentar.sadrzaj.slice(0, 60) : "—", prijavio, razlog: p.razlog, datum: p.datumKreiranja, source: "forum-komentar", id: p.id, targetId: p.komentarId, link: objava ? `../forum/details.html?id=${objava.id}` : null };
        });
        const all = objavaPrijave.concat(komentarPrijave).sort((a, b) => b.datum.localeCompare(a.datum));

        const tbody = document.getElementById("tbody");
        if (!all.length) { document.getElementById("empty-msg").style.display = "block"; tbody.innerHTML = ""; return; }
        tbody.innerHTML = all.map((p) => `
            <tr>
                <td>${Common.escapeHtml(p.tip)}</td>
                <td>${Common.escapeHtml(p.naslov)}</td>
                <td>${p.prijavio ? Common.escapeHtml(p.prijavio.ime + " " + p.prijavio.prezime) : "—"}</td>
                <td>${Common.escapeHtml(p.razlog || "—")}</td>
                <td>${Common.formatDate(p.datum)}</td>
                <td style="display:flex;gap:.4rem;flex-wrap:wrap;">
                    ${p.link ? `<a href="${p.link}" class="btn btn-secondary btn-sm">Detalji</a>` : ""}
                    <button class="btn btn-danger btn-sm" data-remove="${p.source}:${p.id}:${p.targetId}">Ukloni sadržaj</button>
                    <button class="btn btn-secondary btn-sm" data-resolve="${p.source}:${p.id}">Razriješi</button>
                </td>
            </tr>
        `).join("");

        tbody.querySelectorAll("[data-resolve]").forEach((btn) => btn.addEventListener("click", () => {
            const [source, id] = btn.getAttribute("data-resolve").split(":");
            const coll = source === "forum-objava" ? "forumObjavaPrijave" : "forumKomentarPrijave";
            DB.update(coll, Number(id), { status: "razrijesena", razrijesioKorisnikId: staff.id, datumRazrjesenja: new Date().toISOString().slice(0, 10) });
            render();
        }));
        tbody.querySelectorAll("[data-remove]").forEach((btn) => btn.addEventListener("click", () => {
            if (!confirm("Ukloniti ovaj sadržaj? Autor će dobiti opomenu.")) return;
            const [source, id, targetId] = btn.getAttribute("data-remove").split(":");
            let author = null;
            if (source === "forum-objava") {
                const objava = DB.find("forumObjave", Number(targetId));
                if (objava) { author = DB.find("korisnici", objava.korisnikId); DB.remove("forumObjave", objava.id); }
                DB.update("forumObjavaPrijave", Number(id), { status: "razrijesena", razrijesioKorisnikId: staff.id, datumRazrjesenja: new Date().toISOString().slice(0, 10) });
            } else {
                const komentar = DB.find("forumKomentari", Number(targetId));
                if (komentar) { author = DB.find("korisnici", komentar.korisnikId); DB.remove("forumKomentari", komentar.id); }
                DB.update("forumKomentarPrijave", Number(id), { status: "razrijesena", razrijesioKorisnikId: staff.id, datumRazrjesenja: new Date().toISOString().slice(0, 10) });
            }
            if (author) {
                const strikes = (author.brojUklonjenihSadrzaja || 0) + 1;
                const patch = { brojUklonjenihSadrzaja: strikes };
                if (strikes >= 3) patch.datumZabraneDo = new Date(Date.now() + 7 * 86400000).toISOString().slice(0, 10);
                DB.update("korisnici", author.id, patch);
            }
            render();
        }));
    }

    render();
})();
