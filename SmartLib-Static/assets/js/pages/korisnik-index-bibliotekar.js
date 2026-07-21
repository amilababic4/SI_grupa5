(function () {
    "use strict";
    if (!Auth.guard(["Administrator"])) return;
    Common.Flash.renderInto(document.getElementById("alert-container"));

    const list = DB.query("korisnici", (u) => u.uloga === "Bibliotekar").sort((a, b) => a.prezime.localeCompare(b.prezime));
    document.getElementById("biblio-tbody").innerHTML = list.map((u) => `
        <tr style="cursor:pointer;" onclick="window.location.href='profil.html?id=${u.id}'">
            <td>${Common.escapeHtml(u.ime + " " + u.prezime)}</td>
            <td>${Common.escapeHtml(u.email)}</td>
            <td>${Common.statusBadgeHtml(u.status)}</td>
            <td>${Common.formatDate(u.datumKreiranja)}</td>
        </tr>
    `).join("");
})();
