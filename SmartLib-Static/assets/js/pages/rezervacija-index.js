(function () {
    "use strict";
    if (!Auth.guard(["Bibliotekar", "Administrator"])) return;

    function render() {
        const q = document.getElementById("search-input").value.trim().toLowerCase();
        const rows = DB.query("rezervacije", (r) => r.status === "aktivna").map((r) => ({
            r, korisnik: DB.find("korisnici", r.korisnikId), knjiga: DB.find("knjige", r.knjigaId),
        })).filter(({ korisnik, knjiga }) =>
            !q || (korisnik && (korisnik.ime + " " + korisnik.prezime).toLowerCase().includes(q)) || (knjiga && knjiga.naslov.toLowerCase().includes(q))
        );

        document.getElementById("tbody").innerHTML = rows.map(({ r, korisnik, knjiga }) => {
            const dostupna = knjiga && DB.query("primjerci", (p) => p.knjigaId === knjiga.id && p.status === "dostupan").length > 0;
            return `<tr style="${dostupna ? "background:rgba(2,122,72,.06);" : ""}">
                <td>${korisnik ? Common.escapeHtml(korisnik.ime + " " + korisnik.prezime) : "—"}</td>
                <td>${korisnik ? Common.escapeHtml(korisnik.email) : "—"}</td>
                <td><a href="../knjiga/details.html?id=${r.knjigaId}">${knjiga ? Common.escapeHtml(knjiga.naslov) : "—"}</a></td>
                <td>${Common.formatDate(r.datumRezervacije)}</td>
                <td>${Common.formatDate(r.datumIsteka)}</td>
                <td>${dostupna ? '<span class="status-badge status-badge--aktivno">Spremna za zaduženje</span>' : '<span class="status-badge status-badge--zatvoreno">Čeka se</span>'}</td>
            </tr>`;
        }).join("") || `<tr><td colspan="6" style="text-align:center;color:var(--sl-muted);">Nema aktivnih rezervacija.</td></tr>`;
    }

    document.getElementById("search-input").addEventListener("input", render);
    render();
})();
