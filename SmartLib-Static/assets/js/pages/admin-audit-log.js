(function () {
    "use strict";
    if (!Auth.guard(["Administrator"])) return;

    const entries = DB.getAll("auditLog");
    const entitetSelect = document.getElementById("filter-entitet");
    const akcijaSelect = document.getElementById("filter-akcija");
    [...new Set(entries.map((e) => e.entitetTip))].sort().forEach((t) => entitetSelect.insertAdjacentHTML("beforeend", `<option value="${t}">${t}</option>`));
    [...new Set(entries.map((e) => e.akcija))].sort().forEach((a) => akcijaSelect.insertAdjacentHTML("beforeend", `<option value="${a}">${a}</option>`));

    window.__toggleDiff = function (id) {
        const row = document.getElementById("diff-" + id);
        row.classList.toggle("open");
    };

    function render() {
        const entitetFilter = entitetSelect.value;
        const akcijaFilter = akcijaSelect.value;
        const filtered = entries
            .filter((e) => !entitetFilter || e.entitetTip === entitetFilter)
            .filter((e) => !akcijaFilter || e.akcija === akcijaFilter)
            .sort((a, b) => b.datumAkcije.localeCompare(a.datumAkcije));

        document.getElementById("tbody").innerHTML = filtered.map((e) => {
            const korisnik = DB.find("korisnici", e.korisnikId);
            return `<tr>
                <td>${korisnik ? Common.escapeHtml(korisnik.ime + " " + korisnik.prezime) : "Sistem"}</td>
                <td>${Common.escapeHtml(e.akcija)}</td>
                <td>${Common.escapeHtml(e.entitetTip)}${e.entitetId ? " #" + e.entitetId : ""}</td>
                <td>${Common.formatDate(e.datumAkcije)}</td>
                <td><button class="btn btn-secondary btn-sm" onclick="window.__toggleDiff(${e.id})">Prikaži/Sakrij</button></td>
            </tr>
            <tr class="diff-row" id="diff-${e.id}">
                <td colspan="5">
                    <strong>Prije:</strong><pre>${Common.escapeHtml(e.vrijednostiPrije ? JSON.stringify(JSON.parse(e.vrijednostiPrije), null, 2) : "—")}</pre>
                    <strong>Nakon:</strong><pre>${Common.escapeHtml(e.vrijednostiNakon ? JSON.stringify(JSON.parse(e.vrijednostiNakon), null, 2) : "—")}</pre>
                </td>
            </tr>`;
        }).join("") || `<tr><td colspan="5" style="text-align:center;color:var(--sl-muted);">Nema zapisa.</td></tr>`;
    }

    entitetSelect.addEventListener("change", render);
    akcijaSelect.addEventListener("change", render);
    render();
})();
