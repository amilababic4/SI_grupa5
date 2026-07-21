/*
 * SmartLib-Static — shared layout behavior (nav, drawer, dark mode, animations wiring).
 * The header/drawer/footer HTML itself is inlined in every page (see any page's
 * <body> top/bottom); this script only drives visibility + interaction, mirroring
 * what the original _Layout.cshtml did server-side with @if(User.IsInRole(...)).
 */
(function () {
    "use strict";

    function applyAnimationFlag() {
        const user = Auth.currentUser();
        const isStaff = user && (user.uloga === "Bibliotekar" || user.uloga === "Administrator");
        document.body.setAttribute("data-disable-animations", isStaff ? "true" : "false");
    }

    function applyRoleVisibility() {
        const user = Auth.currentUser();
        const authed = !!user;

        document.querySelectorAll('[data-auth="in"]').forEach((el) => {
            el.style.display = authed ? "" : "none";
        });
        document.querySelectorAll('[data-auth="out"]').forEach((el) => {
            el.style.display = authed ? "none" : "";
        });
        document.querySelectorAll("[data-role]").forEach((el) => {
            const roles = el.getAttribute("data-role").split(",").map((r) => r.trim());
            const show = authed && roles.indexOf(user.uloga) !== -1;
            el.style.display = show ? "" : "none";
        });

        if (authed) {
            document.querySelectorAll('[data-user-name]').forEach((el) => { el.textContent = user.ime + " " + user.prezime; });
        }
    }

    function resolveNavLinks() {
        document.querySelectorAll("[data-nav-href]").forEach((a) => {
            a.setAttribute("href", Auth.resolveFromRoot(a.getAttribute("data-nav-href")));
        });
    }

    function updateNotifBadge() {
        const user = Auth.currentUser();
        const badge = document.querySelectorAll(".notif-badge");
        if (!user) { badge.forEach((b) => b.style.display = "none"); return; }
        const count = DB.query("notifikacije", (n) => n.korisnikId === user.id && !n.procitano).length;
        badge.forEach((b) => {
            if (count > 0) { b.textContent = String(count); b.style.display = ""; }
            else { b.style.display = "none"; }
        });
    }

    function wireLogout() {
        document.querySelectorAll("[data-action='logout']").forEach((btn) => {
            btn.addEventListener("click", (e) => {
                e.preventDefault();
                if (typeof SmartLibCache !== "undefined") SmartLibCache.clearAll();
                Auth.logout();
                window.location.href = Auth.resolveFromRoot("index.html");
            });
        });
    }

    function wireThemeToggle() {
        const root = document.documentElement;
        const btn = document.getElementById("theme-toggle");
        if (!btn) return;
        const updateIcon = () => {
            const dark = root.getAttribute("data-theme") === "dark";
            const sun = btn.querySelector(".icon-sun");
            const moon = btn.querySelector(".icon-moon");
            if (sun) sun.style.display = dark ? "block" : "none";
            if (moon) moon.style.display = dark ? "none" : "block";
        };
        updateIcon();
        btn.addEventListener("click", () => {
            const isDark = root.getAttribute("data-theme") === "dark";
            const next = isDark ? "light" : "dark";
            root.setAttribute("data-theme", next);
            localStorage.setItem("smartlib-theme", next);
            updateIcon();
        });
    }

    function wireDrawer() {
        const drawer = document.getElementById("nav-drawer");
        const overlay = document.getElementById("nav-drawer-overlay");
        const hamburger = document.getElementById("nav-hamburger");
        const closeBtn = document.getElementById("nav-drawer-close");
        if (!drawer) return;

        function open() {
            drawer.classList.add("open");
            if (overlay) overlay.classList.add("open");
            if (hamburger) hamburger.setAttribute("aria-expanded", "true");
            document.body.style.overflow = "hidden";
        }
        function close() {
            drawer.classList.remove("open");
            if (overlay) overlay.classList.remove("open");
            if (hamburger) hamburger.setAttribute("aria-expanded", "false");
            document.body.style.overflow = "";
        }
        if (hamburger) hamburger.addEventListener("click", open);
        if (closeBtn) closeBtn.addEventListener("click", close);
        if (overlay) overlay.addEventListener("click", close);
        drawer.querySelectorAll("a").forEach((a) => a.addEventListener("click", close));
        drawer.querySelectorAll("[data-action='logout']").forEach((b) => b.addEventListener("click", close));
        document.addEventListener("keydown", (e) => { if (e.key === "Escape") close(); });
    }

    function wirePasswordToggles() {
        document.querySelectorAll(".password-field").forEach((field) => {
            const input = field.querySelector("input");
            const toggle = field.querySelector(".password-toggle");
            if (!input || !toggle) return;
            toggle.addEventListener("click", () => {
                const isHidden = input.type === "password";
                input.type = isHidden ? "text" : "password";
                toggle.setAttribute("aria-pressed", isHidden ? "true" : "false");
            });
        });
    }

    function teleportModals() {
        document.querySelectorAll(
            ".sl-modal-overlay, .collection-create-overlay, .confirm-modal-overlay, .image-modal-overlay, .kat-modal-overlay"
        ).forEach((el) => {
            if (el.parentElement !== document.body) document.body.appendChild(el);
        });
    }

    // Welcome book-overlay animation: triggered once via sessionStorage flags
    // set right after a successful login (see auth/login page script), instead
    // of the original TempData round-trip.
    const CATALOG_QUOTES = [
        { h: "✦ VAŠA ČITAONICA ✦", f: "OTKRIJTE NOVE SVJETOVE" },
        { h: "✦ RIZNICA PRIČA ✦", f: "ZARONITE U STRANICE" },
        { h: "✦ DAŠAK INSPIRACIJE ✦", f: "BIRAJTE SVOJU AVANTURU" },
        { h: "✦ VAŠA BIBLIOTEKA ✦", f: "DOBRODOŠLI U RAJ" },
        { h: "✦ NOVA STRANICA ✦", f: "PRELISTAJTE KATALOG" },
    ];

    function maybeShowWelcomeOverlay() {
        const overlay = document.getElementById("book-overlay");
        if (!overlay) return;
        const shouldShow = sessionStorage.getItem("smartlib-show-welcome") === "true";
        if (!shouldShow) { overlay.remove(); return; }
        sessionStorage.removeItem("smartlib-show-welcome");

        const user = Auth.currentUser();
        const isStaff = user && (user.uloga === "Bibliotekar" || user.uloga === "Administrator");
        if (isStaff) { overlay.remove(); return; } // animations disabled for staff, per original behavior

        const name = (user ? user.ime : "Član");
        const pick = CATALOG_QUOTES[Math.floor(Math.random() * CATALOG_QUOTES.length)];
        const textEl = overlay.querySelector(".page-text");
        const headerEl = overlay.querySelector(".right-page .book-page-header");
        const footerEl = overlay.querySelector(".left-page .book-page-footer");
        const btnEl = overlay.querySelector("#close-overlay-btn");
        if (textEl) textEl.textContent = `Dobrodošli nazad, ${name}! Drago nam je što ste tu.`;
        if (headerEl) headerEl.textContent = pick.h;
        if (footerEl) footerEl.textContent = pick.f;
        if (btnEl) btnEl.textContent = "Uđite u biblioteku ➔";
        overlay.removeAttribute("hidden");
    }

    function init() {
        applyAnimationFlag();
        applyRoleVisibility();
        resolveNavLinks();
        updateNotifBadge();
        wireLogout();
        wireThemeToggle();
        wireDrawer();
        wirePasswordToggles();
        teleportModals();
        maybeShowWelcomeOverlay();
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", init);
    } else {
        init();
    }

    window.SmartLibLayout = { applyRoleVisibility, updateNotifBadge };
})();
