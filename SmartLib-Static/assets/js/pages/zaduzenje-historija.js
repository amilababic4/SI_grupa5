(function () {
    "use strict";
    if (!Auth.guard(["Bibliotekar", "Administrator"])) return;

    function render() {
        const q = document.getElementById("search-input").value.trim().toLowerCase();
        const loans = DB.query("zaduzenja", (z) => z.status === "zatvoreno").map((z) => ({
            z, korisnik: DB.find("korisnici", z.korisnikId), knjiga: DB.find("knjige", z.knjigaId),
        })).filter(({ korisnik }) => !q || (korisnik && (korisnik.ime + " " + korisnik.prezime).toLowerCase().includes(q)))
            .sort((a, b) => b.z.datumStvarnogVracanja.localeCompare(a.z.datumStvarnogVracanja));

        document.getElementById("hist-tbody").innerHTML = loans.map(({ z, korisnik, knjiga }) => `
            <tr>
                <td><a href="../korisnik/profil.html?id=${z.korisnikId}">${korisnik ? Common.escapeHtml(korisnik.ime + " " + korisnik.prezime) : "—"}</a></td>
                <td><a href="../knjiga/details.html?id=${z.knjigaId}">${knjiga ? Common.escapeHtml(knjiga.naslov) : "—"}</a></td>
                <td>${Common.formatDate(z.datumZaduzivanja)}</td>
                <td>${Common.formatDate(z.datumStvarnogVracanja)}</td>
            </tr>
        `).join("") || `<tr><td colspan="4" style="text-align:center;color:var(--sl-muted);">Nema zatvorenih zaduženja.</td></tr>`;
    }

    document.getElementById("search-input").addEventListener("input", render);
    render();
})();
