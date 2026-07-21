(function () {
    "use strict";

    const CATEGORIES = ["Sve", "Obavještenje", "Događaj"];
    let activeCat = "Sve";

    function allNews() {
        return DB.getAll("vijesti").sort((a, b) => b.datumObjave.localeCompare(a.datumObjave));
    }

    function renderFilterBar() {
        const bar = document.getElementById("cat-filter-bar");
        bar.innerHTML = CATEGORIES.map((c) => `<button type="button" class="cat-chip ${c === activeCat ? "active" : ""}" data-cat="${Common.escapeHtml(c)}">${Common.escapeHtml(c)}</button>`).join("");
        bar.querySelectorAll(".cat-chip").forEach((btn) => {
            btn.addEventListener("click", () => {
                activeCat = btn.getAttribute("data-cat");
                renderFilterBar();
                renderGrid();
            });
        });
    }

    function renderGrid() {
        const grid = document.getElementById("vijest-grid");
        const items = allNews().filter((v) => activeCat === "Sve" || v.kategorija === activeCat);
        const user = Auth.currentUser();
        if (!items.length) {
            grid.innerHTML = '<p style="grid-column:1/-1;text-align:center;color:var(--sl-muted);">Nema vijesti u ovoj kategoriji.</p>';
            return;
        }
        grid.innerHTML = items.map((v) => `
            <div class="vijest-card" data-id="${v.id}">
                ${v.slikaUrl ? `<img src="${v.slikaUrl}" alt="${Common.escapeHtml(v.naslov)}" />` : ""}
                <div class="body">
                    <div class="cat">${Common.escapeHtml(v.kategorija)}</div>
                    <h3>${Common.escapeHtml(v.naslov)}</h3>
                    <div class="date">${Common.formatDate(v.datumObjave)}</div>
                </div>
            </div>
        `).join("");
        grid.querySelectorAll(".vijest-card").forEach((card) => {
            card.addEventListener("click", () => openReader(Number(card.getAttribute("data-id"))));
        });
    }

    function openReader(id) {
        const v = DB.find("vijesti", id);
        if (!v) return;
        document.getElementById("vijest-grid").style.display = "none";
        document.getElementById("cat-filter-bar").style.display = "none";
        const reader = document.getElementById("vijest-reader");
        reader.style.display = "block";
        const html = (typeof marked !== "undefined") ? marked.parse(v.sadrzaj) : `<p>${Common.escapeHtml(v.sadrzaj)}</p>`;
        document.getElementById("vijest-reader-content").innerHTML = `
            <div class="cat">${Common.escapeHtml(v.kategorija)}</div>
            <h1>${Common.escapeHtml(v.naslov)}</h1>
            <div class="date" style="margin-bottom:1rem;">${Common.formatDate(v.datumObjave)}</div>
            ${v.slikaUrl ? `<img src="${v.slikaUrl}" style="width:100%;border-radius:var(--sl-radius-md);margin-bottom:1rem;" />` : ""}
            <div class="markdown-body">${html}</div>
            ${Auth.isStaff() ? `<div style="margin-top:1.5rem;display:flex;gap:.5rem;">
                <button class="btn btn-secondary" id="reader-edit-btn">Uredi</button>
                <button class="btn btn-danger" id="reader-delete-btn">Obriši</button>
            </div>` : ""}
        `;
        if (Auth.isStaff()) {
            document.getElementById("reader-edit-btn").addEventListener("click", () => openModal(v));
            document.getElementById("reader-delete-btn").addEventListener("click", () => deleteVijest(v.id));
        }
    }

    document.getElementById("close-reader-btn").addEventListener("click", () => {
        document.getElementById("vijest-reader").style.display = "none";
        document.getElementById("vijest-grid").style.display = "grid";
        document.getElementById("cat-filter-bar").style.display = "flex";
    });

    // ── Admin CRUD modal ──────────────────────────────────────────────
    const modal = document.getElementById("vijest-modal");

    function openModal(v) {
        document.getElementById("vijest-modal-alert").innerHTML = "";
        document.getElementById("vijest-modal-title").textContent = v ? "Uredi vijest" : "Nova vijest";
        document.getElementById("vijest-id").value = v ? v.id : "";
        document.getElementById("vijest-naslov").value = v ? v.naslov : "";
        document.getElementById("vijest-kategorija").value = v ? v.kategorija : "Obavještenje";
        document.getElementById("vijest-slika").value = v ? (v.slikaUrl || "") : "";
        document.getElementById("vijest-sadrzaj").value = v ? v.sadrzaj : "";
        document.getElementById("vijest-delete-btn").style.display = v ? "" : "none";
        modal.hidden = false;
    }

    function closeModal() { modal.hidden = true; }

    function deleteVijest(id) {
        if (!confirm("Obrisati ovu vijest?")) return;
        DB.remove("vijesti", id);
        closeModal();
        document.getElementById("close-reader-btn").click();
        renderGrid();
    }

    const fab = document.getElementById("vijest-fab");
    fab.addEventListener("click", () => openModal(null));
    document.getElementById("vijest-cancel-btn").addEventListener("click", closeModal);
    document.getElementById("vijest-delete-btn").addEventListener("click", () => deleteVijest(Number(document.getElementById("vijest-id").value)));

    document.getElementById("vijest-form").addEventListener("submit", (e) => {
        e.preventDefault();
        const id = document.getElementById("vijest-id").value;
        const data = {
            naslov: document.getElementById("vijest-naslov").value.trim(),
            kategorija: document.getElementById("vijest-kategorija").value,
            slikaUrl: document.getElementById("vijest-slika").value.trim() || null,
            sadrzaj: document.getElementById("vijest-sadrzaj").value.trim(),
        };
        if (id) {
            DB.update("vijesti", Number(id), data);
        } else {
            data.datumObjave = new Date().toISOString().slice(0, 10);
            data.autorId = Auth.currentUser().id;
            DB.insert("vijesti", data);
        }
        closeModal();
        renderGrid();
    });

    renderFilterBar();
    renderGrid();

    const idParam = Common.qs("id");
    if (idParam) openReader(Number(idParam));
})();
