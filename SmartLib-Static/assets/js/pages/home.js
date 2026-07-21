(function () {
    "use strict";

    function pick(arr, n) {
        const copy = arr.slice();
        const out = [];
        while (copy.length && out.length < n) {
            out.push(copy.splice(Math.floor(Math.random() * copy.length), 1)[0]);
        }
        return out;
    }

    function renderBooks() {
        const user = Auth.currentUser();
        const knjige = DB.getAll("knjige");
        const grid = document.getElementById("home-book-grid");
        let list;
        if (user) {
            document.getElementById("home-books-heading").textContent = "Preporučeno za vas";
            document.getElementById("home-books-lead").textContent = "Na osnovu vaše aktivnosti u katalogu.";
            list = pick(knjige, 4);
        } else {
            document.getElementById("home-books-heading").textContent = "Nasumični naslovi";
            document.getElementById("home-books-lead").textContent = "Prijavite se za personalizovane preporuke.";
            list = pick(knjige, 4);
        }
        grid.innerHTML = list.map((k) => `
            <a data-nav-href="knjiga/details.html?id=${k.id}">
                <img src="${Common.bookCoverUrl(k.naslov)}" alt="${Common.escapeHtml(k.naslov)}" />
                <div class="bg-title">${Common.escapeHtml(k.naslov)}</div>
                <div class="bg-author">${Common.escapeHtml(k.autor)}</div>
            </a>
        `).join("");
        grid.querySelectorAll("[data-nav-href]").forEach((a) => a.setAttribute("href", Auth.resolveFromRoot(a.getAttribute("data-nav-href"))));
    }

    function renderStats() {
        document.getElementById("stat-books").textContent = DB.getAll("knjige").length;
        document.getElementById("stat-members").textContent = DB.query("korisnici", (u) => u.uloga === "Član" && u.status === "aktivan").length;
        document.getElementById("stat-loans").textContent = DB.query("zaduzenja", (z) => z.status !== "zatvoreno").length;
        const today = new Date().toISOString().slice(0, 10);
        document.getElementById("stat-events").textContent = DB.query("dogadjaji", (d) => d.datum >= today).length;
    }

    function renderEvents() {
        const today = new Date().toISOString().slice(0, 10);
        const events = DB.query("dogadjaji", (d) => d.datum >= today).sort((a, b) => a.datum.localeCompare(b.datum)).slice(0, 4);
        const el = document.getElementById("home-events-list");
        if (!events.length) { el.innerHTML = "<p>Trenutno nema predstojećih događaja.</p>"; return; }
        el.innerHTML = events.map((e) => `
            <div class="mini-list-item">
                <div><strong>${Common.escapeHtml(e.naslov)}</strong><br><span style="color:var(--sl-muted);font-size:.85rem;">${Common.formatDate(e.datum)}${e.sat ? " u " + e.sat : ""} · ${Common.escapeHtml(e.lokacija || "")}</span></div>
            </div>
        `).join("");
    }

    function renderNews() {
        const news = DB.getAll("vijesti").sort((a, b) => b.datumObjave.localeCompare(a.datumObjave)).slice(0, 4);
        const el = document.getElementById("home-news-list");
        el.innerHTML = news.map((v) => `
            <div class="mini-list-item">
                <div><strong>${Common.escapeHtml(v.naslov)}</strong><br><span style="color:var(--sl-muted);font-size:.85rem;">${Common.formatDate(v.datumObjave)} · ${Common.escapeHtml(v.kategorija)}</span></div>
            </div>
        `).join("");
    }

    renderBooks();
    renderStats();
    renderEvents();
    renderNews();
})();
