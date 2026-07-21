(function () {
    "use strict";
    if (!Auth.guard(["Bibliotekar", "Administrator"])) return;

    const korisnikId = Number(Common.qs("korisnikId"));
    const korisnik = DB.find("korisnici", korisnikId);
    document.getElementById("back-link").setAttribute("href", "../korisnik/profil.html?id=" + korisnikId);
    document.getElementById("clan-badge").textContent = korisnik ? korisnik.ime + " " + korisnik.prezime : "Korisnik";

    const threeYearsAgo = new Date();
    threeYearsAgo.setFullYear(threeYearsAgo.getFullYear() - 3);
    const cutoff = threeYearsAgo.toISOString().slice(0, 10);

    const loans = DB.query("zaduzenja", (z) => z.korisnikId === korisnikId && z.status === "zatvoreno" && z.datumStvarnogVracanja >= cutoff)
        .sort((a, b) => b.datumStvarnogVracanja.localeCompare(a.datumStvarnogVracanja));

    document.getElementById("meta-line").innerHTML = `Zatvorena zaduženja iz <strong>posljednje 3 godine</strong> · ukupno: <strong>${loans.length}</strong>`;
    document.getElementById("table-wrap").style.display = loans.length ? "" : "none";
    document.getElementById("empty-wrap").style.display = loans.length ? "none" : "";
    if (!loans.length) return;

    document.getElementById("tbody").innerHTML = loans.map((z) => {
        const knjiga = DB.find("knjige", z.knjigaId);
        const primjerak = DB.find("primjerci", z.primjerakId);
        return `<tr>
            <td data-label="Knjiga"><strong>${knjiga ? Common.escapeHtml(knjiga.naslov) : "—"}</strong></td>
            <td data-label="Inv. br." class="katalog-isbn">${primjerak ? Common.escapeHtml(primjerak.inventarniBroj) : "—"}</td>
            <td data-label="Zaduženo">${Common.formatDate(z.datumZaduzivanja)}</td>
            <td data-label="Rok vraćanja">${Common.formatDate(z.datumPlaniranogVracanja)}</td>
            <td data-label="Vraćeno">${z.datumStvarnogVracanja
                ? `<span class="historija-vraceno">${Common.formatDate(z.datumStvarnogVracanja)}</span>`
                : '<span class="historija-nepoznato">—</span>'}</td>
            <td class="katalog-actions">
                <a href="details.html?id=${z.id}&returnUrl=HistorijaClana&korisnikId=${korisnikId}" class="btn btn-secondary btn-sm">Detalji</a>
            </td>
        </tr>`;
    }).join("");
})();
