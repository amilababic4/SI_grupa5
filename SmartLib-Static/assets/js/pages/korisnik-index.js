(function () {
    "use strict";
    if (!Auth.guard(["Bibliotekar", "Administrator"])) return;

    function render() {
        const q = document.getElementById("search-input").value.trim().toLowerCase();
        const showDeact = document.getElementById("show-deactivated").checked;
        const members = DB.query("korisnici", (u) => u.uloga === "Član")
            .filter((u) => showDeact || u.status === "aktivan")
            .filter((u) => !q || (u.ime + " " + u.prezime + " " + u.email).toLowerCase().includes(q))
            .sort((a, b) => a.prezime.localeCompare(b.prezime));
        document.getElementById("members-tbody").innerHTML = members.map((u) => `
            <tr style="cursor:pointer;" onclick="window.location.href='profil.html?id=${u.id}'">
                <td>${Common.escapeHtml(u.ime + " " + u.prezime)}</td>
                <td>${Common.escapeHtml(u.email)}</td>
                <td>${Common.statusBadgeHtml(u.status)}</td>
                <td>${Common.formatDate(u.datumKreiranja)}</td>
            </tr>
        `).join("");
    }

    document.getElementById("search-input").addEventListener("input", render);
    document.getElementById("show-deactivated").addEventListener("change", render);
    render();
})();
