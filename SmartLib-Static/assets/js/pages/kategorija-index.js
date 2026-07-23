(function () {
    "use strict";
    if (!Auth.guard(["Bibliotekar", "Administrator"])) return;
    Common.Flash.renderInto(document.getElementById("alert-container"));

    function bookCount(katId) { return DB.query("knjige", (k) => k.kategorijaId === katId).length; }

    function render() {
        const kategorije = DB.getAll("kategorije").sort((a, b) => a.naziv.localeCompare(b.naziv));
        document.getElementById("kat-count").textContent = kategorije.length;

        if (!kategorije.length) {
            document.getElementById("kat-empty").style.display = "block";
            document.getElementById("kat-card-wrap").style.display = "none";
            return;
        }
        document.getElementById("kat-empty").style.display = "none";
        document.getElementById("kat-card-wrap").style.display = "block";

        const tbody = document.getElementById("kat-tbody");
        tbody.innerHTML = kategorije.map((k) => {
            const count = bookCount(k.id);
            return `
            <tr>
                <td data-label="Naziv"><strong>${Common.escapeHtml(k.naziv)}</strong></td>
                <td data-label="Opis"><span style="color:var(--sl-muted); font-size:0.92rem;">${k.opis ? Common.escapeHtml(k.opis) : "—"}</span></td>
                <td data-label="Knjiga" style="text-align:center;"><span class="katalog-badge">${count}</span></td>
                <td data-label="Akcije" class="katalog-actions">
                    <button type="button" class="btn btn-primary btn-sm" data-toggle-edit="${k.id}">Uredi</button>
                    ${count > 0
                        ? `<button type="button" class="btn btn-danger btn-sm" data-blocked-delete="${k.naziv.replace(/"/g, "&quot;")}:${count}">Obriši</button>`
                        : `<button type="button" class="btn btn-danger btn-sm" data-delete="${k.id}:${k.naziv.replace(/"/g, "&quot;")}">Obriši</button>`}
                </td>
            </tr>
            <tr id="edit-row-${k.id}" class="hidden">
                <td colspan="4" style="padding: 0.5rem 1rem 1rem;">
                    <div class="card kat-add-form" style="margin: 0; border-radius: var(--sl-radius-md);">
                        <h3 class="kat-form-title">Uredi kategoriju</h3>
                        <form class="edit-kat-form" data-id="${k.id}">
                            <div class="kat-form-row">
                                <div class="form-group" style="flex:1; margin-bottom:0;">
                                    <label>Naziv <span style="color:var(--sl-danger)">*</span></label>
                                    <input type="text" class="edit-naziv" value="${Common.escapeHtml(k.naziv)}" maxlength="100" required />
                                </div>
                                <div class="form-group" style="flex:2; margin-bottom:0;">
                                    <label>Opis (opcionalno)</label>
                                    <input type="text" class="edit-opis" value="${Common.escapeHtml(k.opis || "")}" maxlength="500" />
                                </div>
                                <div class="kat-form-actions">
                                    <button type="submit" class="btn btn-primary">Sačuvaj</button>
                                    <button type="button" class="btn btn-secondary" data-toggle-edit="${k.id}">Otkaži</button>
                                </div>
                            </div>
                        </form>
                    </div>
                </td>
            </tr>`;
        }).join("");

        tbody.querySelectorAll("[data-toggle-edit]").forEach((btn) => btn.addEventListener("click", () => toggleEditRow(Number(btn.getAttribute("data-toggle-edit")))));
        tbody.querySelectorAll("[data-blocked-delete]").forEach((btn) => btn.addEventListener("click", () => {
            const [naziv, count] = btn.getAttribute("data-blocked-delete").split(":");
            alert(`Kategorija '${naziv}' ima ${count} ${count === "1" ? "knjigu" : "knjige/knjiga"} i ne može biti obrisana.\n\nPremjestite sve knjige u drugu kategoriju pa pokušajte ponovo.`);
        }));
        tbody.querySelectorAll("[data-delete]").forEach((btn) => btn.addEventListener("click", () => {
            const [id, naziv] = btn.getAttribute("data-delete").split(":");
            if (!confirm(`Obrisati kategoriju '${naziv}'?\nOva akcija je nepovratna.`)) return;
            DB.remove("kategorije", Number(id));
            render();
        }));
        tbody.querySelectorAll(".edit-kat-form").forEach((form) => form.addEventListener("submit", (e) => {
            e.preventDefault();
            const id = Number(form.getAttribute("data-id"));
            DB.update("kategorije", id, {
                naziv: form.querySelector(".edit-naziv").value.trim(),
                opis: form.querySelector(".edit-opis").value.trim(),
            });
            render();
        }));
    }

    function toggleEditRow(id) {
        document.querySelectorAll("[id^='edit-row-']").forEach((row) => {
            if (row.id !== "edit-row-" + id) row.classList.add("hidden");
        });
        document.getElementById("edit-row-" + id).classList.toggle("hidden");
    }

    document.addEventListener("keydown", (e) => {
        if (e.key === "Escape") document.querySelectorAll("[id^='edit-row-']").forEach((row) => row.classList.add("hidden"));
    });

    document.getElementById("toggle-add-form").addEventListener("click", () => document.getElementById("forma-nova-kategorija").classList.toggle("hidden"));
    document.getElementById("cancel-add-kat").addEventListener("click", () => document.getElementById("forma-nova-kategorija").classList.add("hidden"));
    document.getElementById("empty-add-btn").addEventListener("click", () => {
        document.getElementById("forma-nova-kategorija").classList.remove("hidden");
        document.getElementById("naziv-novi").focus();
    });

    document.getElementById("add-kat-form").addEventListener("submit", (e) => {
        e.preventDefault();
        const naziv = document.getElementById("naziv-novi").value.trim();
        if (DB.query("kategorije", (k) => k.naziv.toLowerCase() === naziv.toLowerCase()).length) {
            document.getElementById("alert-container").innerHTML = '<div class="alert alert-error">Kategorija s tim nazivom već postoji.</div>';
            return;
        }
        DB.insert("kategorije", { naziv, opis: document.getElementById("opis-novi").value.trim() });
        document.getElementById("naziv-novi").value = "";
        document.getElementById("opis-novi").value = "";
        document.getElementById("forma-nova-kategorija").classList.add("hidden");
        document.getElementById("alert-container").innerHTML = "";
        render();
    });

    render();
})();
