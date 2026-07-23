(function () {
    "use strict";
    if (!Auth.guard(["Bibliotekar", "Administrator"])) return;
    Common.Flash.renderInto(document.getElementById("alert-container"));

    function render() {
        const q = document.getElementById("search-input").value.trim().toLowerCase();
        const showDeact = document.getElementById("show-deactivated").checked;
        const imaFilter = !!q || showDeact;
        const members = DB.query("korisnici", (u) => u.uloga === "Član")
            .filter((u) => showDeact || u.status === "aktivan")
            .filter((u) => !q || (u.ime + " " + u.prezime + " " + u.email).toLowerCase().includes(q))
            .sort((a, b) => a.prezime.localeCompare(b.prezime));

        document.getElementById("meta-line").innerHTML = imaFilter
            ? `Pronađeno: <strong>${members.length}</strong> ${members.length === 1 ? "član" : "članova"}`
            : `Ukupno aktivnih članova: <strong>${members.length}</strong>`;

        if (!members.length) {
            document.getElementById("table-wrap").style.display = "none";
            document.getElementById("empty-msg").style.display = "block";
            document.getElementById("empty-msg").textContent = imaFilter ? "Nema članova koji odgovaraju pretrazi." : "Trenutno nema registrovanih članova.";
            return;
        }
        document.getElementById("table-wrap").style.display = "";
        document.getElementById("empty-msg").style.display = "none";

        document.getElementById("members-tbody").innerHTML = members.map((u) => `
            <tr class="members-row-link" data-id="${u.id}" title="Pogledaj profil — ${Common.escapeHtml(u.ime + " " + u.prezime)}">
                <td data-label="Ime i prezime"><span class="members-name">${Common.escapeHtml(u.ime + " " + u.prezime)}</span></td>
                <td data-label="Email" class="members-email">${Common.escapeHtml(u.email)}</td>
                <td data-label="Status"><span class="members-status ${u.status === "aktivan" ? "members-status--aktivan" : "members-status--deaktiviran"}">${Common.escapeHtml(u.status)}</span></td>
                <td data-label="Kreiran">${Common.formatDate(u.datumKreiranja)}</td>
            </tr>
        `).join("");
        document.querySelectorAll(".members-row-link").forEach((row) => row.addEventListener("click", () => { window.location.href = "profil.html?id=" + row.getAttribute("data-id"); }));
    }

    document.getElementById("search-btn").addEventListener("click", render);
    document.getElementById("search-input").addEventListener("keydown", (e) => { if (e.key === "Enter") render(); });
    document.getElementById("show-deactivated").addEventListener("change", render);
    document.getElementById("reset-btn").addEventListener("click", () => {
        document.getElementById("search-input").value = "";
        document.getElementById("show-deactivated").checked = false;
        render();
    });

    render();
})();
