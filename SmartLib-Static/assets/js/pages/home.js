(function () {
    "use strict";

    const user = Auth.currentUser();

    function pick(arr, n) {
        const copy = arr.slice();
        const out = [];
        while (copy.length && out.length < n) {
            out.push(copy.splice(Math.floor(Math.random() * copy.length), 1)[0]);
        }
        return out;
    }

    const viewDetailsIcon = `<span class="view-details-icon"><svg width="40" height="40" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="11" cy="11" r="8"></circle><line x1="21" y1="21" x2="16.65" y2="16.65"></line></svg></span><span class="view-details-text">Detalji</span>`;

    function bookCardHtml(k) {
        return `<a data-nav-href="knjiga/details.html?id=${k.id}" class="book-card">
            <div class="book-cover-wrapper">
                <img src="${Common.bookCoverUrl(k.naslov)}" alt="${Common.escapeHtml(k.naslov)}" loading="lazy" />
                <div class="book-overlay">${viewDetailsIcon}</div>
            </div>
            <div class="book-info">
                <span class="book-title">${Common.escapeHtml(k.naslov)}</span>
                <span class="book-year">${k.godinaIzdanja || ""}.</span>
            </div>
        </a>`;
    }

    function resolveGrid(el) {
        el.querySelectorAll("[data-nav-href]").forEach((a) => a.setAttribute("href", Auth.resolveFromRoot(a.getAttribute("data-nav-href"))));
    }

    // ── Random / Recommended books ──────────────────────────────────
    const allBooks = DB.getAll("knjige");
    const randomGrid = document.getElementById("random-grid");
    const randomPicks = pick(allBooks, 4);
    randomGrid.innerHTML = randomPicks.map(bookCardHtml).join("");
    resolveGrid(randomGrid);

    if (user) {
        const closedLoans = DB.query("zaduzenja", (z) => z.korisnikId === user.id && z.status === "zatvoreno");
        const categories = [...new Set(closedLoans.map((z) => { const k = DB.find("knjige", z.knjigaId); return k ? k.kategorijaId : null; }).filter(Boolean))];
        let candidates = categories.length
            ? allBooks.filter((k) => categories.includes(k.kategorijaId) && !randomPicks.find((r) => r.id === k.id))
            : allBooks.filter((k) => !randomPicks.find((r) => r.id === k.id));
        if (!candidates.length) candidates = allBooks;
        const recommended = pick(candidates, 4);
        if (recommended.length) {
            const section = document.getElementById("recommended-section");
            section.style.display = "block";
            const grid = document.getElementById("recommended-grid");
            grid.innerHTML = recommended.map(bookCardHtml).join("");
            resolveGrid(grid);
        }
    }

    // ── Upcoming events ──────────────────────────────────────────────
    const BS_MONTHS = ["Jan", "Feb", "Mar", "Apr", "Maj", "Jun", "Jul", "Aug", "Sep", "Okt", "Nov", "Dec"];
    const todayIso = new Date().toISOString().slice(0, 10);
    const upcoming = DB.query("dogadjaji", (d) => d.datum >= todayIso).sort((a, b) => a.datum.localeCompare(b.datum)).slice(0, 4);
    const eventsList = document.getElementById("events-list");
    const delayClasses = ["d1", "d2", "d3", "d4"];

    if (!upcoming.length) {
        eventsList.innerHTML = `<div class="event-row reveal d1" style="opacity:.6;pointer-events:none;">
            <div class="event-date"><span class="event-date-day">—</span><span class="event-date-mon">—</span></div>
            <div class="event-content"><span class="event-tag">Info</span><div class="event-name">Nema predstojećih događaja</div><div class="event-meta">Pratite kalendar za nove najave</div></div>
            <div class="event-arrow"><svg viewBox="0 0 14 14" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M2 7h10M7 2l5 5-5 5" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/></svg></div>
        </div>`;
    } else {
        eventsList.innerHTML = upcoming.map((ev, i) => {
            const d = new Date(ev.datum + "T00:00:00");
            const meta = [ev.sat, ev.lokacija].filter(Boolean).join(" — ");
            return `<div class="event-row reveal ${delayClasses[i % delayClasses.length]}">
                <div class="event-date"><span class="event-date-day">${String(d.getDate()).padStart(2, "0")}</span><span class="event-date-mon">${BS_MONTHS[d.getMonth()]}</span></div>
                <div class="event-content">
                    <span class="event-tag">${Common.escapeHtml(ev.kategorija)}</span>
                    <div class="event-name">${Common.escapeHtml(ev.naslov)}</div>
                    ${meta ? `<div class="event-meta">${Common.escapeHtml(meta)}</div>` : ""}
                </div>
                <div class="event-arrow"><svg viewBox="0 0 14 14" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M2 7h10M7 2l5 5-5 5" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/></svg></div>
            </div>`;
        }).join("");
    }

    // ── News ──────────────────────────────────────────────────────────
    const HN_BG = { forum: "#1B3D8F", edukacija: "#060F3A", "događaji": "#173b63", djeca: "#0D3373" };
    function hnMediaBg(cat) { return HN_BG[(cat || "").toLowerCase()] || "#0D3373"; }
    function hnDate(iso) {
        const d = new Date(iso + "T00:00:00");
        return `${String(d.getDate()).padStart(2, "0")}/${String(d.getMonth() + 1).padStart(2, "0")}/${d.getFullYear()}`;
    }

    const recentVijesti = DB.getAll("vijesti").sort((a, b) => b.datumObjave.localeCompare(a.datumObjave)).slice(0, 4);
    const newsContent = document.getElementById("news-content");

    if (!recentVijesti.length) {
        newsContent.innerHTML = `<div class="hn-empty reveal">
            <svg viewBox="0 0 64 64" fill="none"><rect x="8" y="12" width="48" height="40" rx="6" stroke="currentColor" stroke-width="2"/><path d="M20 24h24M20 32h16" stroke="currentColor" stroke-width="2" stroke-linecap="round"/></svg>
            <p>Nema objavljenih vijesti.</p>
        </div>`;
    } else {
        const featured = recentVijesti[0];
        const rest = recentVijesti.slice(1);
        const featuredMedia = featured.slikaUrl
            ? `<img src="${featured.slikaUrl}" alt="${Common.escapeHtml(featured.naslov)}" />`
            : `<svg viewBox="0 0 560 320" preserveAspectRatio="xMidYMid slice" xmlns="http://www.w3.org/2000/svg" style="width:100%;height:100%">
                <defs><pattern id="hnp0" width="14" height="14" patternUnits="userSpaceOnUse" patternTransform="rotate(30)"><line x1="0" y1="0" x2="0" y2="14" stroke="#1B3D8F" stroke-width="2"/></pattern></defs>
                <rect width="560" height="320" fill="${hnMediaBg(featured.kategorija)}"/>
                <rect width="560" height="320" fill="url(#hnp0)" opacity="0.22"/>
                <ellipse cx="280" cy="160" rx="130" ry="90" fill="rgba(27,61,143,0.28)"/>
            </svg>`;
        const excerpt = featured.sadrzaj.length > 160 ? featured.sadrzaj.slice(0, 160) + "…" : featured.sadrzaj;

        const sideHtml = rest.map((v, i) => {
            const dCls = i === 0 ? "d1" : (i === 1 ? "d2" : "d3");
            const media = v.slikaUrl
                ? `<img src="${v.slikaUrl}" alt="${Common.escapeHtml(v.naslov)}" />`
                : `<svg viewBox="0 0 120 100" preserveAspectRatio="xMidYMid slice" xmlns="http://www.w3.org/2000/svg" style="width:100%;height:100%">
                    <defs><pattern id="hnps${i}" width="10" height="10" patternUnits="userSpaceOnUse" patternTransform="rotate(-40)"><line x1="0" y1="0" x2="0" y2="10" stroke="rgba(196,217,255,0.35)" stroke-width="1.5"/></pattern></defs>
                    <rect width="120" height="100" fill="${hnMediaBg(v.kategorija)}"/>
                    <rect width="120" height="100" fill="url(#hnps${i})" opacity="0.28"/>
                </svg>`;
            return `<a class="hn-card reveal ${dCls}" data-nav-href="vijest/index.html" style="text-decoration:none;">
                <div class="hn-card-media">${media}</div>
                <div class="hn-card-body">
                    <span class="hn-card-tag">${Common.escapeHtml(v.kategorija)}</span>
                    <div class="hn-card-title">${Common.escapeHtml(v.naslov)}</div>
                    <div class="hn-card-date">${hnDate(v.datumObjave)}</div>
                </div>
            </a>`;
        }).join("");

        newsContent.innerHTML = `<div class="hn-grid">
            <div class="hn-main reveal">
                <a class="hn-card" data-nav-href="vijest/index.html" style="text-decoration:none;">
                    <div class="hn-card-media">${featuredMedia}</div>
                    <div class="hn-card-body">
                        <span class="hn-card-tag">${Common.escapeHtml(featured.kategorija)}</span>
                        <div class="hn-card-title">${Common.escapeHtml(featured.naslov)}</div>
                        <p class="hn-card-excerpt">${Common.escapeHtml(excerpt)}</p>
                        <div class="hn-card-date">${hnDate(featured.datumObjave)}</div>
                    </div>
                </a>
            </div>
            <div class="hn-side">${sideHtml}</div>
        </div>`;
    }
    resolveGrid(newsContent);
    document.querySelectorAll("#events-list [data-nav-href], .news-link[data-nav-href]").forEach((a) => {
        a.setAttribute("href", Auth.resolveFromRoot(a.getAttribute("data-nav-href")));
    });

    // ── Scroll reveal, counters, FAQ accordion, sparkles (verbatim from original) ──
    const _obs = new IntersectionObserver((entries) => {
        entries.forEach((e) => { if (e.isIntersecting) { e.target.classList.add("in"); _obs.unobserve(e.target); } });
    }, { threshold: 0.1 });
    document.querySelectorAll(".reveal").forEach((el) => _obs.observe(el));

    function runCounter(el, target, dur) {
        let t0 = null;
        const step = (ts) => {
            if (!t0) t0 = ts;
            const p = Math.min((ts - t0) / dur, 1);
            el.textContent = Math.round((1 - Math.pow(1 - p, 3)) * target);
            if (p < 1) requestAnimationFrame(step); else el.textContent = target;
        };
        requestAnimationFrame(step);
    }
    const _ctrObs = new IntersectionObserver((entries) => {
        entries.forEach((e) => {
            if (e.isIntersecting) { runCounter(e.target, parseInt(e.target.dataset.target, 10), 1600); _ctrObs.unobserve(e.target); }
        });
    }, { threshold: 0.6 });
    document.querySelectorAll(".counter").forEach((el) => _ctrObs.observe(el));

    document.querySelectorAll(".faq-q").forEach((btn) => {
        btn.addEventListener("click", () => {
            const isOpen = btn.classList.contains("open");
            document.querySelectorAll(".faq-q").forEach((b) => {
                b.classList.remove("open");
                b.setAttribute("aria-expanded", "false");
                b.nextElementSibling.classList.remove("open");
            });
            if (!isOpen) {
                btn.classList.add("open");
                btn.setAttribute("aria-expanded", "true");
                btn.nextElementSibling.classList.add("open");
            }
        });
    });

    (function () {
        const style = document.createElement("style");
        style.textContent =
            "@keyframes sp-a{0%{transform:translate(0,0) scale(1);opacity:.85}25%{transform:translate(7px,-20px) scale(1.18);opacity:1}65%{transform:translate(-4px,-9px) scale(0.82);opacity:.5}100%{transform:translate(0,0) scale(1);opacity:.85}}" +
            "@keyframes sp-b{0%{transform:translate(0,0) scale(1);opacity:.6}35%{transform:translate(-9px,-25px) scale(0.75);opacity:.95}72%{transform:translate(6px,-11px) scale(1.15);opacity:.45}100%{transform:translate(0,0) scale(1);opacity:.6}}" +
            "@keyframes sp-c{0%{transform:translate(0,0) scale(1);opacity:.75}50%{transform:translate(5px,-30px) scale(0.68);opacity:.3}100%{transform:translate(0,0) scale(1);opacity:.75}}" +
            "@keyframes sp-d{0%{transform:translate(0,0) scale(1);opacity:.45}40%{transform:translate(-7px,-16px) scale(1.22);opacity:.85}78%{transform:translate(8px,-6px) scale(0.88);opacity:.25}100%{transform:translate(0,0) scale(1);opacity:.45}}";
        document.head.appendChild(style);

        const anims = ["sp-a", "sp-b", "sp-c", "sp-d"];
        const colors = [[90, 128, 200], [196, 217, 255], [255, 255, 255], [27, 61, 143], [150, 185, 255]];
        const cfg = [
            { sel: ".home-hero", n: 55, z: 3 },
            { sel: ".ticker-wrap", n: 14 },
            { sel: ".sec-reading", n: 34 },
            { sel: ".sec-features", n: 24 },
            { sel: ".sec-news", n: 28 },
            { sel: ".home-featured-books", n: 20 },
        ];
        cfg.forEach((item) => {
            const s = document.querySelector(item.sel);
            if (!s) return;
            s.style.position = "relative";
            s.style.overflow = "hidden";
            const L = document.createElement("div");
            L.setAttribute("aria-hidden", "true");
            L.style.cssText = "position:absolute;inset:0;pointer-events:none;z-index:" + (item.z || 0) + ";overflow:hidden;";
            s.insertBefore(L, s.firstChild);
            for (let i = 0; i < item.n; i++) {
                const el = document.createElement("span");
                const sz = (1.5 + Math.random() * 3.5).toFixed(1);
                const c = colors[Math.floor(Math.random() * colors.length)];
                const op = (0.18 + Math.random() * 0.44).toFixed(2);
                const glow = (parseFloat(sz) * 2.8).toFixed(1);
                const dur = (7 + Math.random() * 10).toFixed(1);
                const del = (-Math.random() * 10).toFixed(1);
                el.style.cssText = [
                    "display:block", "position:absolute", "border-radius:50%",
                    "width:" + sz + "px", "height:" + sz + "px",
                    "left:" + (Math.random() * 100).toFixed(1) + "%",
                    "top:" + (Math.random() * 100).toFixed(1) + "%",
                    "opacity:" + op,
                    "background:rgba(" + c[0] + "," + c[1] + "," + c[2] + "," + op + ")",
                    "box-shadow:0 0 " + glow + "px " + glow + "px rgba(" + c[0] + "," + c[1] + "," + c[2] + "," + (parseFloat(op) * 0.38).toFixed(2) + ")",
                    "animation:" + anims[Math.floor(Math.random() * 3)] + " " + dur + "s " + del + "s infinite ease-in-out",
                    "will-change:transform,opacity",
                ].join(";");
                L.appendChild(el);
            }
        });
    })();
})();
