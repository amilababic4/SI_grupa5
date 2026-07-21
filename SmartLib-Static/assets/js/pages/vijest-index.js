(function () {
    "use strict";

    const isAdmin = Auth.isStaff();
    const MEDIA_BG = { forum: "#1B3D8F", edukacija: "#060F3A", "događaji": "#173b63", djeca: "#0D3373" };
    const TAG_CLS = { Forum: "tc-forum", Edukacija: "tc-edukacija", "Događaji": "tc-događaji", Djeca: "tc-djeca" };

    function fmtDate(iso) {
        const d = new Date(iso + "T00:00:00");
        return `${String(d.getDate()).padStart(2, "0")}/${String(d.getMonth() + 1).padStart(2, "0")}/${d.getFullYear()}`;
    }
    function escH(s) { return Common.escapeHtml(s); }
    function mdPlain(md) {
        return String(md || "")
            .replace(/[#>*_`~-]/g, "")
            .replace(/\[(.*?)\]\(.*?\)/g, "$1")
            .replace(/\s+/g, " ")
            .trim();
    }
    function mdHtml(md) {
        return typeof marked !== "undefined" ? marked.parse(md || "") : `<p>${escH(md)}</p>`;
    }

    function loadVijesti() {
        return DB.getAll("vijesti").sort((a, b) => b.datumObjave.localeCompare(a.datumObjave));
    }

    let vijestList = loadVijesti();
    let _ARTS = {};

    function refreshArts() {
        _ARTS = {};
        vijestList.forEach((v) => {
            _ARTS[v.id] = {
                id: v.id, naslov: v.naslov, sadrzaj: v.sadrzaj, sadrzajHtml: mdHtml(v.sadrzaj),
                kategorija: v.kategorija, slikaUrl: v.slikaUrl, datum: fmtDate(v.datumObjave), datumIso: v.datumObjave,
            };
        });
    }

    function render() {
        vijestList = loadVijesti();
        refreshArts();

        const featured = vijestList[0];
        const gridCards = vijestList.slice(1);
        const categories = [...new Set(vijestList.map((v) => v.kategorija))].sort();
        const catCounts = {};
        vijestList.forEach((v) => { catCounts[v.kategorija] = (catCounts[v.kategorija] || 0) + 1; });

        document.getElementById("stat-total").textContent = vijestList.length;
        document.getElementById("stat-cats").textContent = categories.length;
        document.getElementById("stat-latest").textContent = vijestList.length ? fmtDate(vijestList[0].datumObjave) : "—";

        document.getElementById("filter-bar").innerHTML =
            `<button class="filter-btn active" data-filter="all">Sve <span class="filter-count">${vijestList.length}</span></button>` +
            categories.map((cat) => `<button class="filter-btn" data-filter="${cat.toLowerCase()}">${escH(cat)} <span class="filter-count">${catCounts[cat]}</span></button>`).join("");

        const content = document.getElementById("news-content");
        if (!vijestList.length) {
            content.innerHTML = `<div class="vijesti-empty reveal">
                <svg viewBox="0 0 64 64" fill="none" xmlns="http://www.w3.org/2000/svg"><rect x="8" y="12" width="48" height="40" rx="6" stroke="currentColor" stroke-width="2"/><path d="M20 24h24M20 32h16" stroke="currentColor" stroke-width="2" stroke-linecap="round"/></svg>
                <h3>Nema objavljenih vijesti</h3><p>Vijesti će biti prikazane čim budu objavljene.</p>
            </div>`;
            return;
        }

        let html = "";
        if (featured) {
            const bg = MEDIA_BG[featured.kategorija.toLowerCase()] || "#0D3373";
            const media = featured.slikaUrl
                ? `<img src="${featured.slikaUrl}" alt="${escH(featured.naslov)}" />`
                : `<svg viewBox="0 0 560 460" preserveAspectRatio="xMidYMid slice" xmlns="http://www.w3.org/2000/svg" style="width:100%;height:100%">
                    <defs><pattern id="fp1" width="14" height="14" patternUnits="userSpaceOnUse" patternTransform="rotate(30)"><line x1="0" y1="0" x2="0" y2="14" stroke="#1B3D8F" stroke-width="2"/></pattern></defs>
                    <rect width="560" height="460" fill="${bg}"/><rect width="560" height="460" fill="url(#fp1)" opacity="0.22"/>
                    <ellipse cx="280" cy="230" rx="140" ry="110" fill="rgba(27,61,143,0.3)"/></svg>`;
            const featPlain = mdPlain(featured.sadrzaj);
            html += `<article class="featured-article reveal" data-cat="${featured.kategorija.toLowerCase()}" data-open="${featured.id}">
                <div class="feat-media">${media}<div class="feat-badges"><span class="feat-badge">Istaknuto</span></div></div>
                <div class="feat-body">
                    <div class="feat-cat">${escH(featured.kategorija)}</div>
                    <h2 class="feat-title">${escH(featured.naslov)}</h2>
                    <p class="feat-excerpt">${escH(featPlain.length > 200 ? featPlain.slice(0, 200) + "…" : featPlain)}</p>
                    <div class="feat-footer">
                        <div class="feat-meta"><span>${fmtDate(featured.datumObjave)}</span></div>
                        <button class="feat-link" data-open-stop="${featured.id}">Čitaj više<svg viewBox="0 0 14 14" fill="none"><path d="M2 7h10M7 2l5 5-5 5" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/></svg></button>
                    </div>
                    ${isAdmin ? `<div class="card-admin-bar" data-stop="1"><button class="btn-card-edit" data-edit="${featured.id}">Uredi</button><button class="btn-card-delete" data-delete="${featured.id}">Obriši</button></div>` : ""}
                </div>
            </article>`;
        }

        if (gridCards.length) {
            html += `<div class="section-label"><span class="section-label-text">Sve vijesti</span></div><div class="news-grid">`;
            gridCards.forEach((v, i) => {
                const delayClass = i % 3 === 0 ? "d1" : (i % 3 === 1 ? "d2" : "d3");
                const bg = MEDIA_BG[v.kategorija.toLowerCase()] || "#0D3373";
                const media = v.slikaUrl
                    ? `<img src="${v.slikaUrl}" alt="${escH(v.naslov)}" />`
                    : `<svg viewBox="0 0 360 196" xmlns="http://www.w3.org/2000/svg" style="width:100%;height:100%">
                        <defs><pattern id="nc${v.id}" width="10" height="10" patternUnits="userSpaceOnUse" patternTransform="rotate(-40)"><line x1="0" y1="0" x2="0" y2="10" stroke="rgba(196,217,255,0.4)" stroke-width="1.5"/></pattern></defs>
                        <rect width="360" height="196" fill="${bg}"/><rect width="360" height="196" fill="url(#nc${v.id})" opacity="0.28"/></svg>`;
                const cardPlain = mdPlain(v.sadrzaj);
                html += `<article class="news-card reveal ${delayClass}" data-cat="${v.kategorija.toLowerCase()}" data-open="${v.id}">
                    <div class="news-card-media">${media}<span class="nc-tag ${TAG_CLS[v.kategorija] || "tc-obavještenje"}">${escH(v.kategorija)}</span></div>
                    <div class="news-card-body">
                        <div class="nc-title">${escH(v.naslov)}</div>
                        <p class="nc-excerpt">${escH(cardPlain.length > 120 ? cardPlain.slice(0, 120) + "…" : cardPlain)}</p>
                        <div class="nc-footer"><span class="nc-date">${fmtDate(v.datumObjave)}</span></div>
                        ${isAdmin ? `<div class="card-admin-bar" data-stop="1"><button class="btn-card-edit" data-edit="${v.id}">Uredi</button><button class="btn-card-delete" data-delete="${v.id}">Obriši</button></div>` : ""}
                    </div>
                </article>`;
            });
            html += `</div>`;
        }
        content.innerHTML = html;
        wireCardEvents();
        wireFilters();
        applyReveal();
    }

    function wireCardEvents() {
        document.querySelectorAll("[data-open]").forEach((el) => el.addEventListener("click", () => openArticle(Number(el.getAttribute("data-open")))));
        document.querySelectorAll("[data-open-stop]").forEach((el) => el.addEventListener("click", (e) => { e.stopPropagation(); openArticle(Number(el.getAttribute("data-open-stop"))); }));
        document.querySelectorAll("[data-stop]").forEach((el) => el.addEventListener("click", (e) => e.stopPropagation()));
        document.querySelectorAll("[data-edit]").forEach((el) => el.addEventListener("click", (e) => { e.stopPropagation(); admOpen(Number(el.getAttribute("data-edit"))); }));
        document.querySelectorAll("[data-delete]").forEach((el) => el.addEventListener("click", (e) => { e.stopPropagation(); admDelete(Number(el.getAttribute("data-delete"))); }));
    }

    function wireFilters() {
        document.querySelectorAll(".filter-btn").forEach((btn) => {
            btn.addEventListener("click", () => {
                document.querySelectorAll(".filter-btn").forEach((b) => b.classList.remove("active"));
                btn.classList.add("active");
                const f = btn.getAttribute("data-filter");
                document.querySelectorAll("[data-cat]").forEach((card) => {
                    card.classList.toggle("filtered-out", f !== "all" && card.getAttribute("data-cat") !== f);
                });
            });
        });
    }

    function applyReveal() {
        const obs = new IntersectionObserver((entries) => {
            entries.forEach((e) => { if (e.isIntersecting) { e.target.classList.add("in"); obs.unobserve(e.target); } });
        }, { threshold: 0.1 });
        document.querySelectorAll(".reveal").forEach((el) => obs.observe(el));
    }

    function openArticle(id) {
        const a = _ARTS[id];
        if (!a) return;
        const bg = MEDIA_BG[a.kategorija.toLowerCase()] || "#0D3373";
        const pill = document.getElementById("nd-pill");
        pill.textContent = a.kategorija;
        pill.className = "nd-pill " + (TAG_CLS[a.kategorija] || "tc-obavještenje");
        document.getElementById("nd-meta").innerHTML = `<span>${escH(a.datum)}</span>`;
        document.getElementById("nd-title").textContent = a.naslov;
        document.getElementById("nd-article").innerHTML = a.sadrzajHtml || "";
        document.getElementById("nd-media").innerHTML = a.slikaUrl
            ? `<img src="${escH(a.slikaUrl)}" alt="${escH(a.naslov)}"/>`
            : `<svg viewBox="0 0 700 240" preserveAspectRatio="xMidYMid slice" xmlns="http://www.w3.org/2000/svg" style="width:100%;height:100%">
                <defs><pattern id="vmp${id}" width="12" height="12" patternUnits="userSpaceOnUse" patternTransform="rotate(30)"><line x1="0" y1="0" x2="0" y2="12" stroke="rgba(255,255,255,.18)" stroke-width="1.5"/></pattern></defs>
                <rect width="700" height="240" fill="${bg}"/><rect width="700" height="240" fill="url(#vmp${id})"/></svg>`;
        document.getElementById("nd-modal").classList.add("open");
        const nb = document.querySelector(".nd-body"); if (nb) nb.scrollTop = 0;
    }
    function closeArticle() { document.getElementById("nd-modal").classList.remove("open"); }
    document.getElementById("nd-close").addEventListener("click", closeArticle);
    document.getElementById("nd-back").addEventListener("click", closeArticle);
    document.getElementById("nd-backdrop").addEventListener("click", closeArticle);

    // ── Admin CRUD (direct DB calls instead of fetch) ─────────────────
    const admOverlayEl = document.getElementById("adm-overlay");
    const admTitleEl = document.getElementById("adm-hdr-title-text");
    const admSubmitBtn = document.getElementById("adm-submit");
    const admToastEl = document.getElementById("adm-toast");

    function pad(n) { return String(n).padStart(2, "0"); }
    function isoToDisp(iso) { const p = (iso || "").split("-"); return p.length === 3 ? pad(+p[2]) + "/" + pad(+p[1]) + "/" + p[0] : ""; }
    function dispToIso(d) { const p = (d || "").split("/"); return p.length === 3 && p[2].length === 4 ? p[2] + "-" + pad(+p[1]) + "-" + pad(+p[0]) : ""; }

    function admShowToast(msg) {
        admToastEl.textContent = msg;
        admToastEl.classList.add("show");
        setTimeout(() => admToastEl.classList.remove("show"), 3000);
    }

    window.admOpen = function (id) {
        document.getElementById("af-id").value = id;
        document.getElementById("af-title").classList.remove("af-error");
        document.getElementById("af-content").classList.remove("af-error");
        if (id === 0) {
            admTitleEl.textContent = "Nova vijest";
            document.getElementById("af-title").value = "";
            document.getElementById("af-cat").value = "Obavještenje";
            const td = new Date().toISOString().slice(0, 10);
            document.getElementById("af-date").value = td;
            document.getElementById("af-date-display").value = isoToDisp(td);
            document.getElementById("af-content").value = "";
            document.getElementById("af-img").value = "";
            admSubmitBtn.textContent = "Objavi vijest";
        } else {
            const a = _ARTS[id];
            if (!a) return;
            admTitleEl.textContent = "Uredi vijest";
            document.getElementById("af-title").value = a.naslov || "";
            document.getElementById("af-cat").value = a.kategorija || "Obavještenje";
            document.getElementById("af-date").value = a.datumIso || "";
            document.getElementById("af-date-display").value = isoToDisp(a.datumIso || "");
            document.getElementById("af-content").value = a.sadrzaj || "";
            document.getElementById("af-img").value = a.slikaUrl || "";
            admSubmitBtn.textContent = "Sačuvaj izmjene";
        }
        admSubmitBtn.disabled = false;
        admOverlayEl.classList.add("open");
    };
    function admClose() { admOverlayEl.classList.remove("open"); }

    admSubmitBtn.addEventListener("click", () => {
        const id = parseInt(document.getElementById("af-id").value, 10) || 0;
        const naslov = document.getElementById("af-title").value.trim();
        const sadrzaj = document.getElementById("af-content").value.trim();
        let valid = true;
        if (!naslov) { document.getElementById("af-title").classList.add("af-error"); document.getElementById("af-title").focus(); valid = false; }
        if (!sadrzaj) { document.getElementById("af-content").classList.add("af-error"); if (valid) document.getElementById("af-content").focus(); valid = false; }
        if (!valid) return;

        const data = {
            naslov, sadrzaj,
            kategorija: document.getElementById("af-cat").value,
            datumObjave: document.getElementById("af-date").value,
            slikaUrl: document.getElementById("af-img").value.trim() || null,
        };
        if (id > 0) DB.update("vijesti", id, data);
        else { data.autorId = Auth.currentUser().id; DB.insert("vijesti", data); }

        admClose();
        admShowToast(id > 0 ? "Vijest uspješno ažurirana!" : "Vijest uspješno objavljena!");
        render();
    });

    window.admDelete = function (id) {
        if (!confirm("Sigurno obrisati ovu vijest?")) return;
        DB.remove("vijesti", id);
        admShowToast("Vijest obrisana.");
        render();
    };

    document.getElementById("adm-close").addEventListener("click", admClose);
    document.getElementById("adm-cancel").addEventListener("click", admClose);
    admOverlayEl.addEventListener("click", admClose);
    document.getElementById("adm-drawer").addEventListener("click", (e) => e.stopPropagation());
    document.addEventListener("keydown", (e) => {
        if (e.key === "Escape") {
            if (document.getElementById("nd-modal").classList.contains("open")) closeArticle();
            if (admOverlayEl.classList.contains("open")) admClose();
        }
    });
    document.getElementById("af-title").addEventListener("input", function () { this.classList.remove("af-error"); });
    document.getElementById("af-content").addEventListener("input", function () { this.classList.remove("af-error"); });
    document.getElementById("af-date-display").addEventListener("input", function () { document.getElementById("af-date").value = dispToIso(this.value.trim()); });
    document.getElementById("adm-fab").addEventListener("click", () => admOpen(0));

    render();
})();
