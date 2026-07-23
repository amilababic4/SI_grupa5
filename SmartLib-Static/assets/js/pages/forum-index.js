(function () {
    "use strict";
    if (!Auth.guard()) return;

    const CATEGORIES = ["Opšta diskusija", "Preporuke knjiga", "Pitanja", "Recenzije"];
    const activeCat = Common.qs("kategorija", "");

    const navEl = document.getElementById("cat-nav");
    navEl.innerHTML = `<li><a href="index.html" class="${activeCat ? "" : "active"}">Sve kategorije</a></li>` +
        CATEGORIES.map((c) => `<li><a href="index.html?kategorija=${encodeURIComponent(c)}" class="${activeCat === c ? "active" : ""}">${Common.escapeHtml(c)}</a></li>`).join("");

    const posts = DB.getAll("forumObjave")
        .filter((p) => !activeCat || p.kategorija === activeCat)
        .sort((a, b) => b.datumKreiranja.localeCompare(a.datumKreiranja));

    const el = document.getElementById("posts-list");
    if (!posts.length) {
        el.innerHTML = "<p style='color:var(--sl-muted);'>Nema objava u ovoj kategoriji. Budite prvi koji će nešto podijeliti!</p>";
        return;
    }
    el.innerHTML = posts.map((p) => {
        const autor = DB.find("korisnici", p.korisnikId);
        const komentari = DB.query("forumKomentari", (k) => k.objavaId === p.id).length;
        const reakcije = DB.query("forumReakcije", (r) => r.objavaId === p.id).length;
        return `<a class="forum-post-card" href="details.html?id=${p.id}">
            <div class="cat-badge">${Common.escapeHtml(p.kategorija)}${p.zakljucana ? " · Zaključano" : ""}</div>
            <h3>${Common.escapeHtml(p.naslov)}</h3>
            <p class="excerpt">${Common.escapeHtml((p.sadrzaj || "").slice(0, 140))}${p.sadrzaj.length > 140 ? "…" : ""}</p>
            <div class="meta">
                <span>${autor ? Common.escapeHtml(autor.ime + " " + autor.prezime) : "Korisnik"}</span>
                <span>${Common.formatDate(p.datumKreiranja)}</span>
                <span>💬 ${komentari}</span>
                <span>👍 ${reakcije}</span>
            </div>
        </a>`;
    }).join("");
})();
