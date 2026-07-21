(function () {
    "use strict";

    const DOW = ["Pon", "Uto", "Sri", "Čet", "Pet", "Sub", "Ned"];
    const MONTHS = ["Januar", "Februar", "Mart", "April", "Maj", "Juni", "Juli", "Avgust", "Septembar", "Oktobar", "Novembar", "Decembar"];
    const today = new Date();
    let viewYear = today.getFullYear();
    let viewMonth = today.getMonth(); // 0-based

    function pad(n) { return String(n).padStart(2, "0"); }
    function isoDate(y, m, d) { return `${y}-${pad(m + 1)}-${pad(d)}`; }
    const todayIso = isoDate(today.getFullYear(), today.getMonth(), today.getDate());

    function eventsOn(iso) {
        return DB.query("dogadjaji", (d) => d.datum === iso);
    }

    function renderGrid() {
        document.getElementById("kal-month-title").textContent = `${MONTHS[viewMonth]} ${viewYear}`;
        const grid = document.getElementById("kal-grid");
        let html = DOW.map((d) => `<div class="kal-dow">${d}</div>`).join("");

        const firstOfMonth = new Date(viewYear, viewMonth, 1);
        let startOffset = firstOfMonth.getDay() - 1; // Monday-first
        if (startOffset < 0) startOffset = 6;
        const daysInMonth = new Date(viewYear, viewMonth + 1, 0).getDate();

        for (let i = 0; i < startOffset; i++) html += `<div class="kal-cell empty"></div>`;
        for (let day = 1; day <= daysInMonth; day++) {
            const iso = isoDate(viewYear, viewMonth, day);
            const evs = eventsOn(iso);
            const isToday = iso === todayIso;
            html += `<div class="kal-cell ${isToday ? "today" : ""}" data-date="${iso}">
                <div class="d-num">${day}</div>
                ${evs.map(() => '<span class="ev-dot"></span>').join("")}
            </div>`;
        }
        grid.innerHTML = html;
        grid.querySelectorAll(".kal-cell:not(.empty)").forEach((cell) => {
            cell.addEventListener("click", () => showDayDetail(cell.getAttribute("data-date")));
        });
    }

    function showDayDetail(iso) {
        const evs = eventsOn(iso);
        const el = document.getElementById("kal-day-detail");
        if (!evs.length) {
            el.innerHTML = `<div class="card"><strong>${Common.formatDate(iso)}</strong><p style="color:var(--sl-muted);margin-top:.5rem;">Nema događaja ovog dana.</p></div>`;
            return;
        }
        el.innerHTML = `<div class="card"><strong>${Common.formatDate(iso)}</strong>` + evs.map((e) => `
            <div style="margin-top:.75rem;padding-top:.75rem;border-top:1px solid var(--sl-border);">
                <div style="font-weight:700;">${Common.escapeHtml(e.naslov)}</div>
                <div style="font-size:.85rem;color:var(--sl-muted);">${e.sat ? e.sat + " · " : ""}${Common.escapeHtml(e.lokacija || "")} · ${Common.escapeHtml(e.kategorija)}</div>
                <p style="margin-top:.4rem;">${Common.escapeHtml(e.opis || "")}</p>
                ${Auth.isStaff() ? `<div style="display:flex;gap:.5rem;margin-top:.5rem;">
                    <button class="btn btn-secondary btn-sm" onclick="window.__kalEdit(${e.id})">Uredi</button>
                    <button class="btn btn-danger btn-sm" onclick="window.__kalDelete(${e.id})">Obriši</button>
                </div>` : ""}
            </div>
        `).join("") + `</div>`;
    }

    function renderUpcoming() {
        const list = DB.query("dogadjaji", (d) => d.datum >= todayIso).sort((a, b) => a.datum.localeCompare(b.datum)).slice(0, 8);
        const el = document.getElementById("kal-upcoming-list");
        if (!list.length) { el.innerHTML = "<p style='color:var(--sl-muted);'>Nema predstojećih događaja.</p>"; return; }
        el.innerHTML = list.map((e) => `
            <div class="card">
                <strong>${Common.escapeHtml(e.naslov)}</strong>
                <div style="font-size:.82rem;color:var(--sl-muted);margin-top:.3rem;">${Common.formatDate(e.datum)}${e.sat ? " · " + e.sat : ""}</div>
                <div style="font-size:.82rem;color:var(--sl-muted);">${Common.escapeHtml(e.lokacija || "")}</div>
            </div>
        `).join("");
    }

    document.getElementById("kal-prev").addEventListener("click", () => {
        viewMonth--; if (viewMonth < 0) { viewMonth = 11; viewYear--; }
        renderGrid();
    });
    document.getElementById("kal-next").addEventListener("click", () => {
        viewMonth++; if (viewMonth > 11) { viewMonth = 0; viewYear++; }
        renderGrid();
    });

    // ── Admin CRUD ──────────────────────────────────────────────────
    const modal = document.getElementById("kal-modal");
    function openModal(ev) {
        document.getElementById("kal-modal-title").textContent = ev ? "Uredi događaj" : "Novi događaj";
        document.getElementById("kal-id").value = ev ? ev.id : "";
        document.getElementById("kal-naslov").value = ev ? ev.naslov : "";
        document.getElementById("kal-datum").value = ev ? ev.datum : todayIso;
        document.getElementById("kal-sat").value = ev ? (ev.sat || "") : "";
        document.getElementById("kal-lokacija").value = ev ? (ev.lokacija || "") : "";
        document.getElementById("kal-kategorija").value = ev ? ev.kategorija : "Edukacija";
        document.getElementById("kal-opis").value = ev ? (ev.opis || "") : "";
        document.getElementById("kal-delete-btn").style.display = ev ? "" : "none";
        modal.hidden = false;
    }
    function closeModal() { modal.hidden = true; }

    window.__kalEdit = (id) => openModal(DB.find("dogadjaji", id));
    window.__kalDelete = (id) => {
        if (!confirm("Obrisati ovaj događaj?")) return;
        DB.remove("dogadjaji", id);
        renderGrid();
        renderUpcoming();
        document.getElementById("kal-day-detail").innerHTML = "";
    };

    document.getElementById("kal-fab").addEventListener("click", () => openModal(null));
    document.getElementById("kal-cancel-btn").addEventListener("click", closeModal);
    document.getElementById("kal-delete-btn").addEventListener("click", () => { window.__kalDelete(Number(document.getElementById("kal-id").value)); closeModal(); });

    document.getElementById("kal-form").addEventListener("submit", (e) => {
        e.preventDefault();
        const id = document.getElementById("kal-id").value;
        const data = {
            naslov: document.getElementById("kal-naslov").value.trim(),
            datum: document.getElementById("kal-datum").value,
            sat: document.getElementById("kal-sat").value || null,
            lokacija: document.getElementById("kal-lokacija").value.trim(),
            kategorija: document.getElementById("kal-kategorija").value,
            opis: document.getElementById("kal-opis").value.trim(),
        };
        if (id) {
            DB.update("dogadjaji", Number(id), data);
        } else {
            data.autorId = Auth.currentUser().id;
            DB.insert("dogadjaji", data);
        }
        closeModal();
        renderGrid();
        renderUpcoming();
    });

    renderGrid();
    renderUpcoming();
})();
