(function () {
    "use strict";
    if (!Auth.guard(["Bibliotekar", "Administrator"])) return;

    const primjerakId = Number(Common.qs("id"));
    const primjerak = DB.find("primjerci", primjerakId);
    if (!primjerak) { document.querySelector("main").innerHTML = "<p>Primjerak nije pronađen.</p>"; return; }
    const knjiga = DB.find("knjige", primjerak.knjigaId);

    document.getElementById("back-link").setAttribute("href", "../knjiga/details.html?id=" + primjerak.knjigaId);
    document.getElementById("page-heading").textContent = "Zaduženja primjerka " + primjerak.inventarniBroj;

    const loans = DB.query("zaduzenja", (z) => z.primjerakId === primjerakId).sort((a, b) => b.datumZaduzivanja.localeCompare(a.datumZaduzivanja));
    const active = loans.filter((z) => z.status !== "zatvoreno").length;
    const closed = loans.filter((z) => z.status === "zatvoreno").length;
    document.getElementById("summary").textContent = `Knjiga: ${knjiga ? knjiga.naslov : "—"} · Aktivna: ${active} · Zatvorena: ${closed}`;

    const today = new Date().toISOString().slice(0, 10);
    document.getElementById("tbody").innerHTML = loans.map((z) => {
        const korisnik = DB.find("korisnici", z.korisnikId);
        const kasni = z.status !== "zatvoreno" && z.datumPlaniranogVracanja < today;
        return `<tr>
            <td><a href="../korisnik/profil.html?id=${z.korisnikId}">${korisnik ? Common.escapeHtml(korisnik.ime + " " + korisnik.prezime) : "—"}</a></td>
            <td>${Common.formatDate(z.datumZaduzivanja)}</td>
            <td>${z.status === "zatvoreno" ? Common.formatDate(z.datumStvarnogVracanja) : Common.formatDate(z.datumPlaniranogVracanja)}</td>
            <td>${Common.statusBadgeHtml(z.status === "zatvoreno" ? "zatvoreno" : kasni ? "zakašnjelo" : "aktivno")}</td>
        </tr>`;
    }).join("") || `<tr><td colspan="4" style="text-align:center;color:var(--sl-muted);">Nema zaduženja za ovaj primjerak.</td></tr>`;
})();
