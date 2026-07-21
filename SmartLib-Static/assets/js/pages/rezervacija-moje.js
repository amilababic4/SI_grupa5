(function () {
    "use strict";
    if (!Auth.guard(["Član"])) return;
    const user = Auth.currentUser();
    Common.Flash.renderInto(document.getElementById("alert-container"));

    function knjigaDostupna(knjigaId) {
        return DB.query("primjerci", (p) => p.knjigaId === knjigaId && p.status === "dostupan").length > 0;
    }

    function render() {
        const rezervacije = DB.query("rezervacije", (r) => r.korisnikId === user.id).sort((a, b) => b.datumRezervacije.localeCompare(a.datumRezervacije));
        if (!rezervacije.length) {
            document.getElementById("empty-msg").style.display = "block";
            document.querySelector(".table-responsive").style.display = "none";
            return;
        }
        let anyReady = false;
        document.getElementById("moje-rez-tbody").innerHTML = rezervacije.map((r) => {
            const knjiga = DB.find("knjige", r.knjigaId);
            const ready = r.status === "aktivna" && knjiga && knjigaDostupna(knjiga.id);
            if (ready) anyReady = true;
            return `<tr ${ready ? 'style="background:rgba(2,122,72,.06);"' : ""}>
                <td><a href="../knjiga/details.html?id=${r.knjigaId}">${knjiga ? Common.escapeHtml(knjiga.naslov) : "—"}</a></td>
                <td>${Common.formatDate(r.datumRezervacije)}</td>
                <td>${Common.formatDate(r.datumIsteka)}</td>
                <td>${ready ? '<span class="status-badge status-badge--aktivno">Spremna za zaduženje</span>' : Common.statusBadgeHtml(r.status)}</td>
                <td>${r.status === "aktivna" ? `<button class="btn btn-danger btn-sm" data-cancel="${r.id}">Otkaži</button>` : ""}</td>
            </tr>`;
        }).join("");
        document.getElementById("ready-banner").style.display = anyReady ? "block" : "none";

        document.querySelectorAll("[data-cancel]").forEach((btn) => {
            btn.addEventListener("click", () => {
                if (!confirm("Otkazati ovu rezervaciju?")) return;
                DB.update("rezervacije", Number(btn.getAttribute("data-cancel")), { status: "otkazana" });
                render();
            });
        });
    }

    render();
})();
