(function () {
    "use strict";
    if (!Auth.guard(["Bibliotekar", "Administrator"])) return;
    Common.Flash.renderInto(document.getElementById("alert-container"));

    const today = new Date().toISOString().slice(0, 10);
    const soonThreshold = new Date(Date.now() + 3 * 86400000).toISOString().slice(0, 10);

    function render() {
        const q = document.getElementById("search-input").value.trim().toLowerCase();
        const loans = DB.query("zaduzenja", (z) => z.status !== "zatvoreno").map((z) => {
            const korisnik = DB.find("korisnici", z.korisnikId);
            const knjiga = DB.find("knjige", z.knjigaId);
            return { z, korisnik, knjiga };
        }).filter(({ korisnik, knjiga }) =>
            !q || (korisnik && (korisnik.ime + " " + korisnik.prezime).toLowerCase().includes(q)) || (knjiga && knjiga.naslov.toLowerCase().includes(q))
        ).sort((a, b) => a.z.datumPlaniranogVracanja.localeCompare(b.z.datumPlaniranogVracanja));

        document.getElementById("loans-tbody").innerHTML = loans.map(({ z, korisnik, knjiga }) => {
            const kasni = z.datumPlaniranogVracanja < today;
            const uskoro = !kasni && z.datumPlaniranogVracanja <= soonThreshold;
            return `<tr style="${kasni ? "background:rgba(180,35,24,.06);" : uskoro ? "background:rgba(200,151,74,.08);" : ""}">
                <td><a href="../korisnik/profil.html?id=${z.korisnikId}">${korisnik ? Common.escapeHtml(korisnik.ime + " " + korisnik.prezime) : "—"}</a></td>
                <td><a href="../knjiga/details.html?id=${z.knjigaId}">${knjiga ? Common.escapeHtml(knjiga.naslov) : "—"}</a></td>
                <td>${Common.formatDate(z.datumZaduzivanja)}</td>
                <td>${Common.formatDate(z.datumPlaniranogVracanja)}</td>
                <td>${Common.statusBadgeHtml(kasni ? "zakašnjelo" : "aktivno")}</td>
                <td><a href="vrati-potvrda.html?id=${z.id}" class="btn btn-secondary btn-sm">Evidentiraj vraćanje</a></td>
            </tr>`;
        }).join("") || `<tr><td colspan="6" style="text-align:center;color:var(--sl-muted);">Nema aktivnih zaduženja.</td></tr>`;
    }

    document.getElementById("search-input").addEventListener("input", render);
    render();
})();
