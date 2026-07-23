(function () {
    "use strict";
    if (!Auth.guard(["Član"])) return;
    const user = Auth.currentUser();

    function render() {
        const kolekcije = DB.query("listaKolekcija", (l) => l.korisnikId === user.id).sort((a, b) => b.datumAzuriranja.localeCompare(a.datumAzuriranja));
        const grid = document.getElementById("kol-grid");
        if (!kolekcije.length) { document.getElementById("empty-msg").style.display = "block"; return; }
        grid.innerHTML = kolekcije.map((k) => {
            const brojStavki = DB.query("listaKolekcijaStavke", (s) => s.listaKolekcijaId === k.id).length;
            return `<a class="kol-card" href="details.html?id=${k.id}">
                <h3>${Common.escapeHtml(k.naziv)}${k.isWishlist ? " ⭐" : ""}</h3>
                <div class="meta">${brojStavki} ${brojStavki === 1 ? "knjiga" : "knjiga"} · ${k.javna ? "Javna" : "Privatna"}</div>
                ${k.opis ? `<p style="margin-top:.5rem;font-size:.88rem;color:var(--sl-muted);">${Common.escapeHtml(k.opis)}</p>` : ""}
            </a>`;
        }).join("");
    }

    const modal = document.getElementById("new-kol-modal");
    document.getElementById("new-kol-btn").addEventListener("click", () => { modal.hidden = false; });
    document.getElementById("new-kol-cancel").addEventListener("click", () => { modal.hidden = true; });
    document.getElementById("new-kol-form").addEventListener("submit", (e) => {
        e.preventDefault();
        DB.insert("listaKolekcija", {
            korisnikId: user.id,
            naziv: document.getElementById("new-kol-naziv").value.trim(),
            opis: document.getElementById("new-kol-opis").value.trim(),
            javna: document.getElementById("new-kol-javna").checked,
            datumKreiranja: new Date().toISOString().slice(0, 10),
            datumAzuriranja: new Date().toISOString().slice(0, 10),
            isWishlist: false,
        });
        modal.hidden = true;
        document.getElementById("new-kol-form").reset();
        render();
    });

    render();
})();
