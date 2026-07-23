(function () {
    "use strict";
    if (!Auth.guard(["Član"])) return;
    const user = Auth.currentUser();

    function getOrCreateWishlist() {
        let list = DB.query("listaKolekcija", (l) => l.korisnikId === user.id && l.isWishlist)[0];
        if (!list) {
            list = DB.insert("listaKolekcija", {
                korisnikId: user.id, naziv: "Lista želja", opis: null, javna: false,
                datumKreiranja: new Date().toISOString().slice(0, 10), datumAzuriranja: new Date().toISOString().slice(0, 10), isWishlist: true,
            });
        }
        return list;
    }

    const wishlist = getOrCreateWishlist();
    document.getElementById("toggle-public").checked = wishlist.javna;
    renderPublicLink();

    document.getElementById("toggle-public").addEventListener("change", (e) => {
        DB.update("listaKolekcija", wishlist.id, { javna: e.target.checked });
        renderPublicLink();
    });

    function renderPublicLink() {
        const fresh = DB.find("listaKolekcija", wishlist.id);
        const box = document.getElementById("public-link-box");
        if (fresh.javna) {
            box.style.display = "block";
            box.textContent = `Javni link: ${location.origin}${Auth.resolveFromRoot("lista-kolekcija/details.html")}?id=${wishlist.id}`;
        } else box.style.display = "none";
    }

    function render() {
        const stavke = DB.query("listaKolekcijaStavke", (s) => s.listaKolekcijaId === wishlist.id).sort((a, b) => b.datumDodavanja.localeCompare(a.datumDodavanja));
        const el = document.getElementById("wishlist-items");
        if (!stavke.length) { document.getElementById("empty-msg").style.display = "block"; return; }
        el.innerHTML = stavke.map((s) => {
            const knjiga = DB.find("knjige", s.knjigaId);
            if (!knjiga) return "";
            const dostupno = DB.query("primjerci", (p) => p.knjigaId === knjiga.id && p.status === "dostupan").length > 0;
            return `<div class="wishlist-item">
                <img src="${Common.bookCoverUrl(knjiga.naslov)}" alt="" />
                <div class="info">
                    <a href="../knjiga/details.html?id=${knjiga.id}" style="font-weight:700;">${Common.escapeHtml(knjiga.naslov)}</a>
                    <div style="color:var(--sl-muted);font-size:.85rem;">${Common.escapeHtml(knjiga.autor)}${knjiga.godinaIzdanja ? ", " + knjiga.godinaIzdanja : ""}</div>
                    <span class="status-${dostupno ? "dostupan" : "nedostupan"}">${dostupno ? "Dostupno" : "Zauzeto"}</span>
                </div>
                <button class="btn btn-danger btn-sm" data-remove="${s.id}">Ukloni</button>
            </div>`;
        }).join("");
        el.querySelectorAll("[data-remove]").forEach((btn) => btn.addEventListener("click", () => {
            DB.remove("listaKolekcijaStavke", Number(btn.getAttribute("data-remove")));
            render();
        }));
    }

    render();
})();
