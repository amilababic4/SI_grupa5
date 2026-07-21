(function () {
    "use strict";
    if (!Auth.guard()) return;
    const user = Auth.currentUser();
    Common.Flash.renderInto(document.getElementById("alert-container"));

    const today = new Date().toISOString().slice(0, 10);
    const loans = DB.query("zaduzenja", (z) => z.korisnikId === user.id && z.status !== "zatvoreno");
    const tbody = document.getElementById("moja-tbody");

    if (!loans.length) {
        document.getElementById("empty-msg").style.display = "block";
        document.querySelector(".table-responsive").style.display = "none";
        return;
    }

    tbody.innerHTML = loans.map((z) => {
        const knjiga = DB.find("knjige", z.knjigaId);
        const kasni = z.datumPlaniranogVracanja < today;
        const uskoro = !kasni && z.datumPlaniranogVracanja <= new Date(Date.now() + 3 * 86400000).toISOString().slice(0, 10);
        const status = kasni ? "zakašnjelo" : "aktivno";
        return `<tr>
            <td><a href="../knjiga/details.html?id=${z.knjigaId}">${knjiga ? Common.escapeHtml(knjiga.naslov) : "—"}</a></td>
            <td>${Common.formatDate(z.datumZaduzivanja)}</td>
            <td>${Common.formatDate(z.datumPlaniranogVracanja)}</td>
            <td>${Common.statusBadgeHtml(status)}${uskoro ? ' <span style="color:var(--sl-muted);font-size:.8rem;">(rok se bliži)</span>' : ""}</td>
        </tr>`;
    }).join("");
})();
