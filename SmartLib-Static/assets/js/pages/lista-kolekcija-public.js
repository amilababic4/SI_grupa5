(function () {
    "use strict";

    function render(filter) {
        const kolekcije = DB.query("listaKolekcija", (l) => l.javna)
            .filter((k) => !filter || k.naziv.toLowerCase().includes(filter.toLowerCase()))
            .sort((a, b) => b.datumAzuriranja.localeCompare(a.datumAzuriranja));
        const grid = document.getElementById("kol-grid");
        if (!kolekcije.length) { document.getElementById("empty-msg").style.display = "block"; grid.innerHTML = ""; return; }
        document.getElementById("empty-msg").style.display = "none";
        grid.innerHTML = kolekcije.map((k) => {
            const vlasnik = DB.find("korisnici", k.korisnikId);
            const brojStavki = DB.query("listaKolekcijaStavke", (s) => s.listaKolekcijaId === k.id).length;
            return `<a class="kol-card" href="details.html?id=${k.id}">
                <h3 style="margin:0 0 .3rem;">${Common.escapeHtml(k.naziv)}</h3>
                <div style="color:var(--sl-muted);font-size:.82rem;">${brojStavki} ${brojStavki === 1 ? "knjiga" : "knjiga"}</div>
                <div style="color:var(--sl-muted);font-size:.82rem;margin-top:.3rem;">by ${vlasnik ? Common.escapeHtml(vlasnik.ime + " " + vlasnik.prezime) : "—"}</div>
            </a>`;
        }).join("");
    }

    document.getElementById("search-input").addEventListener("input", (e) => render(e.target.value));
    render("");
})();
