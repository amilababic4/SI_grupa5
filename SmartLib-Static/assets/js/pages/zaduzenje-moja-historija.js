(function () {
    "use strict";
    if (!Auth.guard()) return;
    const user = Auth.currentUser();
    Common.Flash.renderInto(document.getElementById("alert-container"));

    const threeYearsAgo = new Date();
    threeYearsAgo.setFullYear(threeYearsAgo.getFullYear() - 3);
    const cutoff = threeYearsAgo.toISOString().slice(0, 10);

    const loans = DB.query("zaduzenja", (z) => z.korisnikId === user.id && z.status === "zatvoreno" && z.datumStvarnogVracanja >= cutoff)
        .sort((a, b) => b.datumStvarnogVracanja.localeCompare(a.datumStvarnogVracanja));

    document.getElementById("meta-line").innerHTML = `Zatvorena zaduženja iz <strong>posljednje 3 godine</strong> · ukupno: <strong>${loans.length}</strong>`;
    document.getElementById("table-wrap").style.display = loans.length ? "" : "none";
    document.getElementById("empty-wrap").style.display = loans.length ? "none" : "";
    if (!loans.length) return;

    document.getElementById("hist-tbody").innerHTML = loans.map((z) => {
        const knjiga = DB.find("knjige", z.knjigaId);
        const primjerak = DB.find("primjerci", z.primjerakId);
        const hasReview = knjiga && DB.query("recenzije", (r) => r.knjigaId === knjiga.id && r.korisnikId === user.id).length > 0;
        return `<tr>
            <td data-label="Knjiga"><strong>${knjiga ? Common.escapeHtml(knjiga.naslov) : "—"}</strong></td>
            <td data-label="Inv. br." class="katalog-isbn">${primjerak ? Common.escapeHtml(primjerak.inventarniBroj) : "—"}</td>
            <td data-label="Zaduženo">${Common.formatDate(z.datumZaduzivanja)}</td>
            <td data-label="Rok vraćanja">${Common.formatDate(z.datumPlaniranogVracanja)}</td>
            <td data-label="Vraćeno">${z.datumStvarnogVracanja
                ? `<span class="historija-vraceno">${Common.formatDate(z.datumStvarnogVracanja)}</span>`
                : '<span class="historija-nepoznato">—</span>'}</td>
            <td data-label="Akcija">${
                !z.datumStvarnogVracanja ? '<span class="historija-nepoznato">—</span>'
                : hasReview ? '<span class="katalog-badge" style="background: rgba(245,158,11,0.12); color: #f59e0b;">Recenzija ostavljena</span>'
                : `<a href="../recenzija/dodaj.html?knjigaId=${z.knjigaId}" class="btn btn-primary btn-sm">Ocijeni knjigu</a>`
            }</td>
        </tr>`;
    }).join("");
})();
