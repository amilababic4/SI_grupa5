(function () {
    "use strict";

    const isAdmin = Auth.isStaff();
    const MONTHS = ["Januar", "Februar", "Mart", "April", "Maj", "Juni", "Juli", "August", "Septembar", "Oktobar", "Novembar", "Decembar"];
    const DAYS = ["Pon", "Uto", "Sri", "Čet", "Pet", "Sub", "Ned"];
    const BS_MONTHS_LOWER = ["januar", "februar", "mart", "april", "maj", "juni", "juli", "august", "septembar", "oktobar", "novembar", "decembar"];

    const now = new Date();
    const TODAY_ISO = now.toISOString().slice(0, 10);
    let curYear = now.getFullYear();
    let curMonth = now.getMonth();
    let activeCatFilter = "all";
    let selectedDateStr = TODAY_ISO;

    function pad(n) { return String(n).padStart(2, "0"); }
    function isoToDisp(iso) { const p = (iso || "").split("-"); return p.length === 3 ? pad(+p[2]) + "/" + pad(+p[1]) + "/" + p[0] : ""; }
    function dispToIso(d) { const p = (d || "").split("/"); return p.length === 3 && p[2].length === 4 ? p[2] + "-" + pad(+p[1]) + "-" + pad(+p[0]) : ""; }
    function toDs(y, m, d) { return y + "-" + pad(m + 1) + "-" + pad(d); }
    function escH(s) { return Common.escapeHtml(s); }

    function loadEvents() {
        return DB.getAll("dogadjaji").map((d) => ({
            id: d.id, date: d.datum, name: d.naslov, time: d.sat || "", loc: d.lokacija || "",
            cat: (d.kategorija || "edukacija").toLowerCase(), opis: d.opis || "",
        }));
    }
    let EVENTS = loadEvents();

    function getEvs(ds) {
        return EVENTS.filter((e) => activeCatFilter === "all" ? e.date === ds : e.date === ds && e.cat === activeCatFilter);
    }

    function renderHeroStats() {
        const thisMonthCount = EVENTS.filter((e) => e.date.startsWith(`${curYear}-${pad(now.getMonth() + 1)}`) && now.getFullYear() === curYear).length;
        // Match original: counts based on *current real* month/year, not the navigated calendar month.
        const realMonthCount = DB.query("dogadjaji", (d) => {
            const dt = new Date(d.datum + "T00:00:00");
            return dt.getFullYear() === now.getFullYear() && dt.getMonth() === now.getMonth();
        }).length;
        const upcomingCount = DB.query("dogadjaji", (d) => d.datum >= TODAY_ISO).length;
        const daysInMonth = new Date(now.getFullYear(), now.getMonth() + 1, 0).getDate();
        const progressPct = Math.round((now.getDate() / daysInMonth) * 100);
        const monthName = BS_MONTHS_LOWER[now.getMonth()];
        const monthNameCap = monthName.charAt(0).toUpperCase() + monthName.slice(1);

        document.getElementById("stat-month").textContent = realMonthCount;
        document.getElementById("stat-month-label").textContent = "Događaja u " + monthNameCap;
        document.getElementById("stat-upcoming").textContent = upcomingCount;
        setTimeout(() => { document.getElementById("month-progress").style.width = progressPct + "%"; }, 400);
    }

    function renderCalendar() {
        document.getElementById("cal-month-title").textContent = MONTHS[curMonth] + " " + curYear;
        const firstDay = new Date(curYear, curMonth, 1);
        const daysInMonth = new Date(curYear, curMonth + 1, 0).getDate();
        let offset = firstDay.getDay() - 1;
        if (offset < 0) offset = 6;

        const grid = document.getElementById("cal-grid");
        grid.innerHTML = "";
        DAYS.forEach((d) => {
            const h = document.createElement("div");
            h.className = "cal-day-hdr";
            h.textContent = d;
            grid.appendChild(h);
        });
        for (let i = 0; i < offset; i++) {
            const blank = document.createElement("div");
            blank.className = "cal-cell cal-empty";
            grid.appendChild(blank);
        }
        const todayDate = new Date(now.getFullYear(), now.getMonth(), now.getDate());
        for (let d = 1; d <= daysInMonth; d++) {
            const ds = toDs(curYear, curMonth, d);
            const allEvs = EVENTS.filter((e) => e.date === ds);
            const isToday = ds === TODAY_ISO;
            const isPast = new Date(curYear, curMonth, d) < todayDate;
            const isSel = ds === selectedDateStr;

            const cell = document.createElement("div");
            cell.className = "cal-cell" + (isToday ? " is-today" : "") + (isSel && !isToday ? " is-selected" : "") + (isPast && !isToday ? " is-past" : "");
            cell.setAttribute("data-date", ds);
            const numSpan = document.createElement("span");
            numSpan.textContent = d;
            cell.appendChild(numSpan);
            if (allEvs.length) {
                const dotsEl = document.createElement("div");
                dotsEl.className = "cal-dots";
                allEvs.slice(0, 3).forEach((ev) => {
                    const dot = document.createElement("span");
                    dot.className = "cal-dot dot-" + ev.cat;
                    dotsEl.appendChild(dot);
                });
                cell.appendChild(dotsEl);
            }
            cell.addEventListener("click", () => selectDay(ds));
            grid.appendChild(cell);
        }
    }

    function selectDay(ds) {
        selectedDateStr = ds;
        document.querySelectorAll(".cal-cell").forEach((c) => {
            const isT = c.getAttribute("data-date") === TODAY_ISO;
            c.classList.remove("is-selected");
            if (c.getAttribute("data-date") === ds && !isT) c.classList.add("is-selected");
        });
        renderDayPanel(ds);
        const dateInput = document.getElementById("af-date");
        if (dateInput) dateInput.value = ds;
        const dateDisp = document.getElementById("af-date-display");
        if (dateDisp) dateDisp.value = isoToDisp(ds);
    }

    function renderDayPanel(ds) {
        const evs = getEvs(ds);
        const parts = ds.split("-");
        document.getElementById("day-panel-date").textContent = pad(+parts[2]) + "/" + pad(+parts[1]) + "/" + parts[0];
        const panel = document.getElementById("day-panel-events");
        if (!evs.length) {
            panel.innerHTML = '<div class="day-empty"><div class="day-empty-line"></div><p>Nema planiranih događaja za ovaj dan.</p></div>';
            return;
        }
        panel.innerHTML = evs.map((e) => {
            const adminBtns = isAdmin
                ? `<div class="day-event-admin"><button class="btn-ev-edit" data-edit="${e.id}">Uredi</button><button class="btn-ev-del" data-del="${e.id}">Obriši</button></div>`
                : "";
            return `<div class="day-event">
                <div class="day-event-time">${escH(e.time || "—")}</div>
                <div class="day-event-info">
                    <div class="day-event-name">${escH(e.name)}</div>
                    ${e.loc ? `<div class="day-event-loc">${escH(e.loc)}</div>` : ""}
                    <span class="day-event-cat cat-dot-${e.cat}">${escH(e.cat)}</span>
                    ${adminBtns}
                </div>
            </div>`;
        }).join("");
        panel.querySelectorAll("[data-edit]").forEach((b) => b.addEventListener("click", () => admOpen(Number(b.getAttribute("data-edit")))));
        panel.querySelectorAll("[data-del]").forEach((b) => b.addEventListener("click", () => admDelete(Number(b.getAttribute("data-del")))));
    }

    function renderUpcoming() {
        const upcoming = EVENTS.filter((e) => e.date >= TODAY_ISO && (activeCatFilter === "all" || e.cat === activeCatFilter)).sort((a, b) => a.date.localeCompare(b.date));
        const list = document.getElementById("upcoming-list");
        if (!upcoming.length) {
            list.innerHTML = '<div style="color:var(--ink-muted);font-size:.9rem;padding:1rem 0;">Nema predstojećih događaja za odabrani filter.</div>';
            return;
        }
        list.innerHTML = upcoming.map((e, i) => {
            const parts = e.date.split("-");
            const dl = pad(+parts[2]) + "/" + pad(+parts[1]) + "/" + parts[0];
            const adminActs = isAdmin
                ? `<div class="upcoming-admin-acts"><button class="btn-ev-edit" data-edit="${e.id}">Uredi</button><button class="btn-ev-del" data-del="${e.id}">Obriši</button></div>`
                : "";
            return `<div class="upcoming-row" style="animation:upRowIn .4s ease ${i * 0.06}s both">
                <span class="upnum">${pad(i + 1)}</span><span class="update">${dl}</span><span class="upname">${escH(e.name)}</span>
                <span class="upcat cat-dot-${e.cat}">${escH(e.cat)}</span><span class="uptime">${escH(e.time)}</span>${adminActs}
            </div>`;
        }).join("");
        list.querySelectorAll("[data-edit]").forEach((b) => b.addEventListener("click", () => admOpen(Number(b.getAttribute("data-edit")))));
        list.querySelectorAll("[data-del]").forEach((b) => b.addEventListener("click", () => admDelete(Number(b.getAttribute("data-del")))));
    }

    document.getElementById("cal-prev").addEventListener("click", () => { curMonth--; if (curMonth < 0) { curMonth = 11; curYear--; } renderCalendar(); });
    document.getElementById("cal-next").addEventListener("click", () => { curMonth++; if (curMonth > 11) { curMonth = 0; curYear++; } renderCalendar(); });

    document.querySelectorAll(".cal-filter").forEach((btn) => {
        btn.addEventListener("click", () => {
            document.querySelectorAll(".cal-filter").forEach((b) => b.classList.remove("active"));
            btn.classList.add("active");
            activeCatFilter = btn.getAttribute("data-cat-filter");
            renderCalendar();
            renderUpcoming();
            renderDayPanel(selectedDateStr);
        });
    });

    const revealObs = new IntersectionObserver((entries) => {
        entries.forEach((e) => { if (e.isIntersecting) { e.target.classList.add("in"); revealObs.unobserve(e.target); } });
    }, { threshold: 0.1 });
    document.querySelectorAll(".reveal").forEach((el) => revealObs.observe(el));

    const aStyle = document.createElement("style");
    aStyle.textContent = "@keyframes upRowIn{from{opacity:0;transform:translateX(-12px)}to{opacity:1;transform:none}}";
    document.head.appendChild(aStyle);

    // ── Admin CRUD (direct DB calls) ──────────────────────────────────
    const admOverlayEl = document.getElementById("adm-overlay");
    const admTitleEl = document.getElementById("adm-hdr-title-text");
    const admSubmitBtn = document.getElementById("adm-submit");
    const admToastEl = document.getElementById("adm-toast");

    function admShowToast(msg) {
        admToastEl.textContent = msg;
        admToastEl.classList.add("show");
        setTimeout(() => admToastEl.classList.remove("show"), 3000);
    }

    function admOpen(id) {
        document.getElementById("af-id").value = id;
        document.getElementById("af-name").classList.remove("af-error");
        if (id === 0) {
            admTitleEl.textContent = "Novi događaj";
            document.getElementById("af-name").value = "";
            document.getElementById("af-date").value = selectedDateStr;
            document.getElementById("af-date-display").value = isoToDisp(selectedDateStr);
            document.getElementById("af-time").value = "17:00";
            document.getElementById("af-loc").value = "";
            document.getElementById("af-cat").value = "Edukacija";
            document.getElementById("af-desc").value = "";
            admSubmitBtn.textContent = "Dodaj događaj";
        } else {
            const ev = EVENTS.find((e) => e.id === id);
            if (!ev) return;
            admTitleEl.textContent = "Uredi događaj";
            document.getElementById("af-name").value = ev.name || "";
            document.getElementById("af-date").value = ev.date || "";
            document.getElementById("af-date-display").value = isoToDisp(ev.date || "");
            document.getElementById("af-time").value = ev.time || "";
            document.getElementById("af-loc").value = ev.loc || "";
            const catCap = (ev.cat || "edukacija");
            document.getElementById("af-cat").value = catCap.charAt(0).toUpperCase() + catCap.slice(1);
            document.getElementById("af-desc").value = ev.opis || "";
            admSubmitBtn.textContent = "Sačuvaj izmjene";
        }
        admSubmitBtn.disabled = false;
        admOverlayEl.classList.add("open");
    }
    function admClose() { admOverlayEl.classList.remove("open"); }

    admSubmitBtn.addEventListener("click", () => {
        const id = parseInt(document.getElementById("af-id").value, 10) || 0;
        const naslov = document.getElementById("af-name").value.trim();
        if (!naslov) { document.getElementById("af-name").classList.add("af-error"); document.getElementById("af-name").focus(); return; }
        const dateVal = document.getElementById("af-date").value;
        if (!dateVal) { document.getElementById("af-date-display").classList.add("af-error"); document.getElementById("af-date-display").focus(); return; }

        const timeVal = document.getElementById("af-time").value.trim();
        const locVal = document.getElementById("af-loc").value.trim();
        const catVal = document.getElementById("af-cat").value;
        const descVal = document.getElementById("af-desc").value.trim();

        const data = { naslov, datum: dateVal, sat: timeVal, lokacija: locVal, kategorija: catVal, opis: descVal };
        if (id > 0) DB.update("dogadjaji", id, data);
        else { data.autorId = Auth.currentUser().id; DB.insert("dogadjaji", data); }

        EVENTS = loadEvents();
        admClose();
        admShowToast(id > 0 ? "Događaj uspješno ažuriran!" : "Događaj uspješno dodan!");
        renderCalendar();
        selectDay(selectedDateStr);
        renderUpcoming();
        renderHeroStats();
    });

    function admDelete(id) {
        if (!confirm("Sigurno obrisati ovaj događaj?")) return;
        DB.remove("dogadjaji", id);
        EVENTS = loadEvents();
        admShowToast("Događaj obrisan.");
        renderCalendar();
        selectDay(selectedDateStr);
        renderUpcoming();
        renderHeroStats();
    }

    document.getElementById("adm-fab").addEventListener("click", () => admOpen(0));
    const admDayBtn = document.getElementById("adm-day-btn");
    if (admDayBtn) admDayBtn.addEventListener("click", () => admOpen(0));
    document.getElementById("adm-close").addEventListener("click", admClose);
    document.getElementById("adm-cancel").addEventListener("click", admClose);
    admOverlayEl.addEventListener("click", admClose);
    document.getElementById("adm-drawer").addEventListener("click", (e) => e.stopPropagation());
    document.addEventListener("keydown", (e) => { if (e.key === "Escape" && admOverlayEl.classList.contains("open")) admClose(); });
    document.getElementById("af-name").addEventListener("input", function () { this.classList.remove("af-error"); });
    document.getElementById("af-date-display").addEventListener("input", function () {
        document.getElementById("af-date").value = dispToIso(this.value.trim());
        this.classList.remove("af-error");
    });

    renderHeroStats();
    renderCalendar();
    selectDay(selectedDateStr);
    renderUpcoming();
})();
