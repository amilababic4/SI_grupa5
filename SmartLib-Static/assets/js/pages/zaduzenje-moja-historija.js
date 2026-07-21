(function () {
    "use strict";
    if (!Auth.guard()) return;
    const user = Auth.currentUser();

    const loans = DB.query("zaduzenja", (z) => z.korisnikId === user.id && z.status === "zatvoreno")
        .sort((a, b) => b.datumStvarnogVracanja.localeCompare(a.datumStvarnogVracanja));
    const tbody = document.getElementById("hist-tbody");

    if (!loans.length) {
        document.getElementById("empty-msg").style.display = "block";
        document.querySelector(".table-responsive").style.display = "none";
        return;
    }

    tbody.innerHTML = loans.map((z) => {
        const knjiga = DB.find("knjige", z.knjigaId);
        const hasReview = knjiga && DB.query("recenzije", (r) => r.knjigaId === knjiga.id && r.korisnikId === user.id).length > 0;
        return `<tr>
            <td><a href="../knjiga/details.html?id=${z.knjigaId}">${knjiga ? Common.escapeHtml(knjiga.naslov) : "—"}</a></td>
            <td>${Common.formatDate(z.datumZaduzivanja)}</td>
            <td>${Common.formatDate(z.datumStvarnogVracanja)}</td>
            <td>${hasReview
                ? '<span class="status-badge status-badge--zatvoreno">Recenzija ostavljena</span>'
                : `<a href="../recenzija/dodaj.html?knjigaId=${z.knjigaId}" class="btn btn-secondary btn-sm">Ocijeni knjigu</a>`}</td>
        </tr>`;
    }).join("");
})();
