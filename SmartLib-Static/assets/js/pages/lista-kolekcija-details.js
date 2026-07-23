(function () {
    "use strict";
    if (!Auth.guard()) return;
    const user = Auth.currentUser();
    const kolId = Number(Common.qs("id"));
    const kolekcija = DB.find("listaKolekcija", kolId);
    if (!kolekcija) { document.querySelector("main").innerHTML = "<p>Kolekcija nije pronađena.</p>"; return; }

    const jeVlasnik = kolekcija.korisnikId === user.id;
    if (!jeVlasnik && !kolekcija.javna) { document.querySelector("main").innerHTML = "<p>Ova kolekcija je privatna.</p>"; return; }

    document.getElementById("kol-naziv").textContent = kolekcija.naziv + (kolekcija.isWishlist ? " ⭐" : "");
    document.getElementById("kol-opis").textContent = kolekcija.opis || "";

    if (jeVlasnik) {
        document.getElementById("kol-owner-actions").innerHTML = `
            <label style="font-size:.85rem;"><input type="checkbox" id="toggle-public" ${kolekcija.javna ? "checked" : ""} /> Javna kolekcija</label>
        `;
        document.getElementById("toggle-public").addEventListener("change", (e) => {
            DB.update("listaKolekcija", kolId, { javna: e.target.checked });
            renderPublicLink();
        });
        renderPublicLink();
    }

    function renderPublicLink() {
        const box = document.getElementById("public-link-box");
        const fresh = DB.find("listaKolekcija", kolId);
        if (jeVlasnik && fresh.javna) {
            box.style.display = "block";
            box.textContent = `Javni link: ${location.origin}${Auth.resolveFromRoot("lista-kolekcija/details.html")}?id=${kolId}`;
        } else {
            box.style.display = "none";
        }
    }

    function items() {
        return DB.query("listaKolekcijaStavke", (s) => s.listaKolekcijaId === kolId).sort((a, b) => a.redoslijed - b.redoslijed);
    }

    function render() {
        const stavke = items();
        const el = document.getElementById("items-list");
        if (!stavke.length) { document.getElementById("empty-msg").style.display = "block"; el.innerHTML = ""; return; }
        el.innerHTML = stavke.map((s) => {
            const knjiga = DB.find("knjige", s.knjigaId);
            if (!knjiga) return "";
            return `<div class="item-row" draggable="${jeVlasnik}" data-stavka-id="${s.id}">
                ${jeVlasnik ? '<span class="drag-handle">⠿</span>' : ""}
                <img src="${Common.bookCoverUrl(knjiga.naslov)}" alt="" />
                <div class="info">
                    <a href="../knjiga/details.html?id=${knjiga.id}" style="font-weight:700;">${Common.escapeHtml(knjiga.naslov)}</a>
                    <div style="color:var(--sl-muted);font-size:.85rem;">${Common.escapeHtml(knjiga.autor)}</div>
                </div>
                ${jeVlasnik ? `<button class="btn btn-danger btn-sm" data-remove="${s.id}">Ukloni</button>` : ""}
            </div>`;
        }).join("");

        if (jeVlasnik) {
            el.querySelectorAll("[data-remove]").forEach((btn) => btn.addEventListener("click", () => {
                DB.remove("listaKolekcijaStavke", Number(btn.getAttribute("data-remove")));
                render();
            }));
            wireDragReorder(el);
        }
    }

    function wireDragReorder(container) {
        let dragged = null;
        container.querySelectorAll(".item-row").forEach((row) => {
            row.addEventListener("dragstart", () => { dragged = row; row.style.opacity = "0.5"; });
            row.addEventListener("dragend", () => { row.style.opacity = ""; });
            row.addEventListener("dragover", (e) => e.preventDefault());
            row.addEventListener("drop", (e) => {
                e.preventDefault();
                if (!dragged || dragged === row) return;
                const rows = [...container.querySelectorAll(".item-row")];
                const draggedIdx = rows.indexOf(dragged);
                const targetIdx = rows.indexOf(row);
                if (draggedIdx < targetIdx) row.after(dragged);
                else row.before(dragged);
                const newOrder = [...container.querySelectorAll(".item-row")].map((r) => Number(r.getAttribute("data-stavka-id")));
                newOrder.forEach((id, idx) => DB.update("listaKolekcijaStavke", id, { redoslijed: idx + 1 }));
            });
        });
    }

    render();
})();
