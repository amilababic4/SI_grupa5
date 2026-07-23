/*
 * SmartLib-Static — shared UI helpers used across page scripts.
 */
(function (global) {
    "use strict";

    function qs(name, fallback) {
        const params = new URLSearchParams(window.location.search);
        const v = params.get(name);
        return v === null ? (fallback === undefined ? null : fallback) : v;
    }

    function escapeHtml(str) {
        if (str === null || str === undefined) return "";
        return String(str)
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#39;");
    }

    function formatDate(iso) {
        if (!iso) return "—";
        const d = new Date(iso + (iso.length <= 10 ? "T00:00:00" : ""));
        if (isNaN(d.getTime())) return iso;
        // Manual dd.mm.yyyy. formatting — avoids relying on locale/ICU data
        // (toLocaleDateString('bs-BA', ...) is unreliable across environments).
        const dd = String(d.getDate()).padStart(2, "0");
        const mm = String(d.getMonth() + 1).padStart(2, "0");
        return `${dd}.${mm}.${d.getFullYear()}.`;
    }

    // ── One-shot flash messages (replaces TempData["SuccessMessage"/"ErrorMessage"]) ──
    const FLASH_KEY = "smartlib:flash";
    const Flash = {
        set(type, message) {
            sessionStorage.setItem(FLASH_KEY, JSON.stringify({ type, message }));
        },
        renderInto(container) {
            if (!container) return;
            const raw = sessionStorage.getItem(FLASH_KEY);
            if (!raw) return;
            sessionStorage.removeItem(FLASH_KEY);
            const { type, message } = JSON.parse(raw);
            const div = document.createElement("div");
            div.className = "alert alert-" + (type === "error" ? "error" : "success");
            div.textContent = message;
            container.prepend(div);
        },
    };

    // ── Star rating renderer (matches .lb-stars / .star.full/.half/.empty classes) ──
    function starsHtml(prosjecnaOcjena, brojRecenzija) {
        let html = '<span class="lb-stars">';
        for (let i = 1; i <= 5; i++) {
            let cls = "empty";
            if (brojRecenzija > 0) {
                if (i <= Math.floor(prosjecnaOcjena)) cls = "full";
                else if (i - prosjecnaOcjena < 1 && i - prosjecnaOcjena > 0) cls = "half";
            }
            html += `<span class="star ${cls}">★</span>`;
        }
        html += "</span>";
        return html;
    }

    // ── Book cover placeholder (self-contained SVG data URI — no external API calls) ──
    const COVER_PALETTES = [
        ["#173b63", "#2563eb"], ["#5c1a2a", "#b42318"], ["#1e3a5f", "#3b82f6"],
        ["#2d4a3a", "#10b981"], ["#4a1515", "#ef4444"], ["#3d2b1f", "#b8860b"],
        ["#0f1e3d", "#6366f1"], ["#4a4a2a", "#ca8a04"],
    ];
    function bookCoverUrl(naslov) {
        let hash = 0;
        for (let i = 0; i < naslov.length; i++) hash = (hash * 31 + naslov.charCodeAt(i)) >>> 0;
        const [c1, c2] = COVER_PALETTES[hash % COVER_PALETTES.length];
        const initials = naslov.split(" ").filter(Boolean).slice(0, 2).map((w) => w[0].toUpperCase()).join("");
        const svg = `<svg xmlns="http://www.w3.org/2000/svg" width="240" height="360" viewBox="0 0 240 360">
            <defs><linearGradient id="g" x1="0" y1="0" x2="1" y2="1">
                <stop offset="0" stop-color="${c1}"/><stop offset="1" stop-color="${c2}"/>
            </linearGradient></defs>
            <rect width="240" height="360" fill="url(#g)"/>
            <rect x="10" y="10" width="220" height="340" fill="none" stroke="rgba(255,255,255,.35)" stroke-width="2"/>
            <text x="120" y="190" font-family="Georgia, serif" font-size="64" fill="rgba(255,255,255,.92)" text-anchor="middle">${escapeHtml(initials)}</text>
        </svg>`;
        return "data:image/svg+xml;charset=utf-8," + encodeURIComponent(svg);
    }

    // ── Pagination control renderer ──────────────────────────────────────────
    function paginationHtml(page, totalPages, hrefForPage) {
        if (totalPages <= 1) return "";
        let html = '<div class="pagination">';
        html += page > 1
            ? `<a href="${hrefForPage(page - 1)}" class="btn btn-secondary">← Prethodna</a>`
            : `<span class="btn btn-secondary pagination-disabled">← Prethodna</span>`;
        html += `<span class="pagination-info">Strana <strong>${page}</strong> od <strong>${totalPages}</strong></span>`;
        html += page < totalPages
            ? `<a href="${hrefForPage(page + 1)}" class="btn btn-secondary">Sljedeća →</a>`
            : `<span class="btn btn-secondary pagination-disabled">Sljedeća →</span>`;
        html += "</div>";
        return html;
    }

    // ── Loan/reservation/membership status badges (reuses site.css .status-badge
    //    modifiers, and the standalone .status-dostupan/.status-nedostupan/
    //    .status-zaduzen/.status-deaktiviran classes for copy (primjerak) status) ──
    function statusBadgeHtml(status) {
        const standalone = {
            dostupan: "Dostupno",
            "zadužen": "Zaduženo",
            deaktiviran: "Deaktivirano",
        };
        if (standalone[status]) {
            const cls = status === "dostupan" ? "status-dostupan" : status === "zadužen" ? "status-zaduzen" : "status-deaktiviran";
            return `<span class="${cls}">${standalone[status]}</span>`;
        }
        const map = {
            aktivno: ["Aktivno", "status-badge--aktivno"],
            zatvoreno: ["Zatvoreno", "status-badge--zatvoreno"],
            "zakašnjelo": ["Kasni", "status-badge--zakasnelo"],
            aktivna: ["Aktivna", "status-badge--aktivno"],
            realizovana: ["Realizovana", "status-badge--zatvoreno"],
            otkazana: ["Otkazana", "status-badge--zakasnelo"],
            istekla: ["Istekla", "status-badge--zakasnelo"],
            aktivan: ["Aktivan", "status-badge--aktivno"],
            na_cekanju: ["Na čekanju", "status-badge--zatvoreno"],
            odobreno: ["Odobreno", "status-badge--aktivno"],
            odbijeno: ["Odbijeno", "status-badge--zakasnelo"],
            otvorena: ["Otvorena", "status-badge--zakasnelo"],
            razrijesena: ["Razriješena", "status-badge--zatvoreno"],
        };
        const [label, cls] = map[status] || [status, "status-badge--zatvoreno"];
        return `<span class="status-badge ${cls}">${escapeHtml(label)}</span>`;
    }

    function requireEl(id) {
        const el = document.getElementById(id);
        return el;
    }

    global.Common = { qs, escapeHtml, formatDate, Flash, starsHtml, bookCoverUrl, paginationHtml, statusBadgeHtml, requireEl };
})(window);
