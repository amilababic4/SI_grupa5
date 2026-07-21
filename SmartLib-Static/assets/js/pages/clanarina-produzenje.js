(function () {
    "use strict";
    if (!Auth.guard(["Član"])) return;
    const user = Auth.currentUser();
    Common.Flash.renderInto(document.getElementById("alert-container"));

    const today = new Date().toISOString().slice(0, 10);
    const clanarina = DB.query("clanarine", (c) => c.korisnikId === user.id).sort((a, b) => b.datumIsteka.localeCompare(a.datumIsteka))[0];
    const aktivna = clanarina && clanarina.datumIsteka >= today;

    document.getElementById("status-card").innerHTML = clanarina
        ? `<p>Status članarine: ${aktivna ? '<span class="status-badge status-badge--aktivno">Aktivna</span>' : '<span class="status-badge status-badge--zakasnelo">Istekla</span>'}</p>
           <p style="color:var(--sl-muted);font-size:.9rem;">Ističe: ${Common.formatDate(clanarina.datumIsteka)}</p>`
        : `<p>Nemate evidentiranu članarinu.</p>`;

    const myRequests = DB.query("zahtjeviProduzenja", (z) => z.korisnikId === user.id).sort((a, b) => b.datumPodnosenja.localeCompare(a.datumPodnosenja));
    const hasPending = myRequests.some((z) => z.status === "na_cekanju");

    let selectedMonths = null;
    document.querySelectorAll(".duration-tile").forEach((tile) => {
        tile.addEventListener("click", () => {
            document.querySelectorAll(".duration-tile").forEach((t) => t.classList.remove("selected"));
            tile.classList.add("selected");
            selectedMonths = Number(tile.getAttribute("data-months"));
            const baseDate = new Date((aktivna ? clanarina.datumIsteka : today) + "T00:00:00");
            baseDate.setMonth(baseDate.getMonth() + selectedMonths);
            const previewBox = document.getElementById("preview-box");
            previewBox.style.display = "block";
            previewBox.textContent = `Novi datum isteka (procijenjeno): ${Common.formatDate(baseDate.toISOString().slice(0, 10))}`;
        });
    });

    if (hasPending) {
        document.getElementById("request-card").innerHTML = "<p style='color:var(--sl-muted);'>Već imate zahtjev na čekanju. Sačekajte obradu prije podnošenja novog.</p>";
    } else {
        document.getElementById("renew-form").addEventListener("submit", (e) => {
            e.preventDefault();
            if (!selectedMonths) { alert("Odaberite trajanje produženja."); return; }
            DB.insert("zahtjeviProduzenja", {
                korisnikId: user.id,
                trajanjeMjeseci: selectedMonths,
                napomena: document.getElementById("napomena").value.trim(),
                status: "na_cekanju",
                datumPodnosenja: today,
                datumObrade: null,
                obradioKorisnikId: null,
                razlogOdbijanja: null,
                noviDatumIsteka: null,
            });
            Common.Flash.set("success", "Zahtjev za produženje je podnesen.");
            window.location.reload();
        });
    }

    document.getElementById("history-list").innerHTML = myRequests.length
        ? myRequests.map((z) => `<div class="history-item">
            <span>${z.trajanjeMjeseci} mjeseci · ${Common.formatDate(z.datumPodnosenja)}</span>
            ${Common.statusBadgeHtml(z.status)}
        </div>`).join("")
        : "<p style='color:var(--sl-muted);'>Nema prethodnih zahtjeva.</p>";
})();
