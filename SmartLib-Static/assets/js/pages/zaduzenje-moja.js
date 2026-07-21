(function () {
    "use strict";
    if (!Auth.guard()) return;
    const user = Auth.currentUser();

    const today = new Date().toISOString().slice(0, 10);
    const soonThreshold = new Date(Date.now() + 3 * 86400000).toISOString().slice(0, 10);
    const loans = DB.query("zaduzenja", (z) => z.korisnikId === user.id && z.status !== "zatvoreno");

    document.getElementById("table-wrap").style.display = loans.length ? "" : "none";
    document.getElementById("empty-wrap").style.display = loans.length ? "none" : "";
    if (!loans.length) return;

    document.getElementById("moja-tbody").innerHTML = loans.map((z) => {
        const knjiga = DB.find("knjige", z.knjigaId);
        const primjerak = DB.find("primjerci", z.primjerakId);
        const kasni = z.datumPlaniranogVracanja < today;
        const uskoro = !kasni && z.datumPlaniranogVracanja <= soonThreshold;
        return `<tr class="${kasni ? "tr-kasni" : uskoro ? "tr-blizi" : ""}">
            <td data-label="Knjiga"><strong>${knjiga ? Common.escapeHtml(knjiga.naslov) : "—"}</strong></td>
            <td data-label="Inv. br." class="katalog-isbn">${primjerak ? Common.escapeHtml(primjerak.inventarniBroj) : "—"}</td>
            <td data-label="Zaduženo">${Common.formatDate(z.datumZaduzivanja)}</td>
            <td data-label="Rok vraćanja">${Common.formatDate(z.datumPlaniranogVracanja)}</td>
            <td data-label="Status">${
                kasni ? '<span class="status-kasni">Zakašnjelo</span>'
                : uskoro ? '<span class="status-blizi">Rok uskoro</span>'
                : '<span class="status-dostupan">Aktivno</span>'
            }</td>
        </tr>`;
    }).join("");
})();
