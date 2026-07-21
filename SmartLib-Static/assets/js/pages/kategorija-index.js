(function () {
    "use strict";
    if (!Auth.guard(["Bibliotekar", "Administrator"])) return;

    function bookCount(katId) {
        return DB.query("knjige", (k) => k.kategorijaId === katId).length;
    }

    function render() {
        const kategorije = DB.getAll("kategorije").sort((a, b) => a.naziv.localeCompare(b.naziv));
        const tbody = document.getElementById("kat-tbody");
        tbody.innerHTML = kategorije.map((k) => {
            const count = bookCount(k.id);
            return `
            <tr data-id="${k.id}">
                <td class="view-row">${Common.escapeHtml(k.naziv)}</td>
                <td class="view-row">${Common.escapeHtml(k.opis || "—")}</td>
                <td class="view-row">${count}</td>
                <td class="view-row">
                    <button class="btn btn-secondary btn-sm" data-action="edit">Uredi</button>
                    ${count > 0
                        ? `<button class="btn btn-danger btn-sm" title="Kategorija ima knjige" onclick="alert('Ne možete obrisati kategoriju koja sadrži knjige.')">Obriši</button>`
                        : `<button class="btn btn-danger btn-sm" data-action="delete">Obriši</button>`}
                </td>
                <td class="edit-row" colspan="4" style="display:none;">
                    <form class="edit-kat-form" style="display:flex;gap:.5rem;align-items:center;flex-wrap:wrap;">
                        <input type="text" class="edit-naziv" value="${Common.escapeHtml(k.naziv)}" required style="flex:1;min-width:140px;" />
                        <input type="text" class="edit-opis" value="${Common.escapeHtml(k.opis || "")}" style="flex:2;min-width:180px;" />
                        <button type="submit" class="btn btn-primary btn-sm">Sačuvaj</button>
                        <button type="button" class="btn btn-secondary btn-sm" data-action="cancel-edit">Otkaži</button>
                    </form>
                </td>
            </tr>`;
        }).join("");

        tbody.querySelectorAll("tr").forEach((row) => {
            const id = Number(row.getAttribute("data-id"));
            const viewCells = row.querySelectorAll(".view-row");
            const editCell = row.querySelector(".edit-row");
            row.querySelector("[data-action='edit']").addEventListener("click", () => {
                viewCells.forEach((c) => (c.style.display = "none"));
                editCell.style.display = "";
            });
            row.querySelector("[data-action='cancel-edit']").addEventListener("click", () => {
                viewCells.forEach((c) => (c.style.display = ""));
                editCell.style.display = "none";
            });
            const delBtn = row.querySelector("[data-action='delete']");
            if (delBtn) {
                delBtn.addEventListener("click", () => {
                    if (!confirm("Obrisati ovu kategoriju?")) return;
                    DB.remove("kategorije", id);
                    render();
                });
            }
            row.querySelector(".edit-kat-form").addEventListener("submit", (e) => {
                e.preventDefault();
                DB.update("kategorije", id, {
                    naziv: row.querySelector(".edit-naziv").value.trim(),
                    opis: row.querySelector(".edit-opis").value.trim(),
                });
                render();
            });
        });
    }

    const addCard = document.getElementById("add-kat-form-card");
    document.getElementById("add-kat-btn").addEventListener("click", () => {
        addCard.style.display = addCard.style.display === "none" ? "block" : "none";
    });
    document.getElementById("add-kat-form").addEventListener("submit", (e) => {
        e.preventDefault();
        const naziv = document.getElementById("new-kat-naziv").value.trim();
        if (DB.query("kategorije", (k) => k.naziv.toLowerCase() === naziv.toLowerCase()).length) {
            document.getElementById("alert-container").innerHTML = '<div class="alert alert-error">Kategorija s tim nazivom već postoji.</div>';
            return;
        }
        DB.insert("kategorije", { naziv, opis: document.getElementById("new-kat-opis").value.trim() });
        document.getElementById("new-kat-naziv").value = "";
        document.getElementById("new-kat-opis").value = "";
        addCard.style.display = "none";
        document.getElementById("alert-container").innerHTML = "";
        render();
    });

    render();
})();
