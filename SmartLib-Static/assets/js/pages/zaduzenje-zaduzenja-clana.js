(function () {
    "use strict";
    if (!Auth.guard(["Bibliotekar", "Administrator"])) return;

    const korisnikId = Number(Common.qs("korisnikId"));
    const korisnik = DB.find("korisnici", korisnikId);
    document.getElementById("back-link").setAttribute("href", "../korisnik/profil.html?id=" + korisnikId);
    document.getElementById("page-heading").textContent = "Aktivna zaduženja — " + (korisnik ? korisnik.ime + " " + korisnik.prezime : "");

    const today = new Date().toISOString().slice(0, 10);
    const loans = DB.query("zaduzenja", (z) => z.korisnikId === korisnikId && z.status !== "zatvoreno");
    document.getElementById("tbody").innerHTML = loans.map((z) => {
        const knjiga = DB.find("knjige", z.knjigaId);
        const kasni = z.datumPlaniranogVracanja < today;
        return `<tr>
            <td><a href="../knjiga/details.html?id=${z.knjigaId}">${knjiga ? Common.escapeHtml(knjiga.naslov) : "—"}</a></td>
            <td>${Common.formatDate(z.datumZaduzivanja)}</td>
            <td>${Common.formatDate(z.datumPlaniranogVracanja)}</td>
            <td>${Common.statusBadgeHtml(kasni ? "zakašnjelo" : "aktivno")}</td>
        </tr>`;
    }).join("") || `<tr><td colspan="4" style="text-align:center;color:var(--sl-muted);">Nema aktivnih zaduženja.</td></tr>`;
})();
