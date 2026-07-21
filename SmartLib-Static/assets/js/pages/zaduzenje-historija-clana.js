(function () {
    "use strict";
    if (!Auth.guard(["Bibliotekar", "Administrator"])) return;

    const korisnikId = Number(Common.qs("korisnikId"));
    const korisnik = DB.find("korisnici", korisnikId);
    document.getElementById("back-link").setAttribute("href", "../korisnik/profil.html?id=" + korisnikId);
    document.getElementById("page-heading").textContent = "Historija zaduženja — " + (korisnik ? korisnik.ime + " " + korisnik.prezime : "");

    const loans = DB.query("zaduzenja", (z) => z.korisnikId === korisnikId && z.status === "zatvoreno")
        .sort((a, b) => b.datumStvarnogVracanja.localeCompare(a.datumStvarnogVracanja));
    document.getElementById("tbody").innerHTML = loans.map((z) => {
        const knjiga = DB.find("knjige", z.knjigaId);
        return `<tr>
            <td><a href="../knjiga/details.html?id=${z.knjigaId}">${knjiga ? Common.escapeHtml(knjiga.naslov) : "—"}</a></td>
            <td>${Common.formatDate(z.datumZaduzivanja)}</td>
            <td>${Common.formatDate(z.datumStvarnogVracanja)}</td>
        </tr>`;
    }).join("") || `<tr><td colspan="3" style="text-align:center;color:var(--sl-muted);">Nema historije zaduženja.</td></tr>`;
})();
