/*
 * SmartLib-Static — session/auth simulation (no real server).
 * Session = { korisnikId } stored in localStorage under "smartlib:session".
 */
(function (global) {
    "use strict";

    const SESSION_KEY = "smartlib:session";

    function getSession() {
        const raw = localStorage.getItem(SESSION_KEY);
        return raw ? JSON.parse(raw) : null;
    }

    function currentUser() {
        const session = getSession();
        if (!session) return null;
        return DB.find("korisnici", session.korisnikId);
    }

    function isAuthenticated() {
        return currentUser() !== null;
    }

    function isInRole(role) {
        const user = currentUser();
        return !!user && user.uloga === role;
    }

    function isStaff() {
        return isInRole("Bibliotekar") || isInRole("Administrator");
    }

    function login(email, password) {
        const user = DB.query("korisnici", (u) => u.email.toLowerCase() === String(email).toLowerCase() && u.lozinka === password)[0];
        if (!user) return { ok: false, error: "Pogrešan email ili lozinka." };
        if (user.status === "deaktiviran") return { ok: false, error: "Nalog je deaktiviran. Obratite se administratoru." };
        localStorage.setItem(SESSION_KEY, JSON.stringify({ korisnikId: user.id }));
        return { ok: true, user: user };
    }

    function logout() {
        localStorage.removeItem(SESSION_KEY);
    }

    function register(data) {
        const exists = DB.query("korisnici", (u) => u.email.toLowerCase() === data.email.toLowerCase())[0];
        if (exists) return { ok: false, error: "Korisnik s ovim emailom već postoji." };
        const user = DB.insert("korisnici", {
            ime: data.ime,
            prezime: data.prezime,
            email: data.email,
            lozinka: data.lozinka,
            uloga: data.uloga || "Član",
            status: "aktivan",
            datumKreiranja: new Date().toISOString().slice(0, 10),
            brojUklonjenihSadrzaja: 0,
            datumZabraneDo: null,
            listaZeljaJavna: false,
        });
        DB.insert("clanarine", {
            korisnikId: user.id,
            datumPocetka: new Date().toISOString().slice(0, 10),
            datumIsteka: new Date(new Date().setFullYear(new Date().getFullYear() + 1)).toISOString().slice(0, 10),
        });
        return { ok: true, user: user };
    }

    function changePassword(trenutna, nova) {
        const user = currentUser();
        if (!user) return { ok: false, error: "Niste prijavljeni." };
        if (user.lozinka !== trenutna) return { ok: false, error: "Trenutna lozinka nije tačna." };
        DB.update("korisnici", user.id, { lozinka: nova });
        return { ok: true };
    }

    // Simulated password reset — since there's no email server, the "reset link"
    // is represented purely client-side by finding the user and letting them set
    // a new password directly on the reset page (token check is skipped in this demo).
    function resetPassword(email, novaLozinka) {
        const user = DB.query("korisnici", (u) => u.email.toLowerCase() === String(email).toLowerCase())[0];
        if (!user) return { ok: false, error: "Nije pronađen korisnik s tim emailom." };
        DB.update("korisnici", user.id, { lozinka: novaLozinka });
        return { ok: true };
    }

    // Redirects to login (or shows an inline message) if requirements aren't met.
    // roles: optional array of allowed roles; omit for "any authenticated user".
    function guard(roles) {
        const user = currentUser();
        if (!user) {
            const returnUrl = encodeURIComponent(location.pathname.split("/").pop() + location.search);
            location.href = resolveFromRoot("auth/login.html") + "?returnUrl=" + returnUrl;
            return false;
        }
        if (roles && roles.length && roles.indexOf(user.uloga) === -1) {
            document.body.innerHTML = '<div style="max-width:640px;margin:4rem auto;text-align:center;font-family:sans-serif;">' +
                '<h2>Nemate pristup ovoj stranici</h2><p>Ova stranica je dostupna samo ulogama: ' + roles.join(", ") + '.</p>' +
                '<a href="' + resolveFromRoot("index.html") + '">Povratak na početnu</a></div>';
            return false;
        }
        return true;
    }

    // Computes a relative path back to the site root from the current page,
    // based on how many folder levels deep the current page is (0 = root).
    function resolveFromRoot(target) {
        const depth = location.pathname.replace(/\/index\.html$/, "/").split("/").filter(Boolean).length;
        // Pages live either at root (index.html) or one level deep (e.g. /knjiga/index.html).
        // Every page in this build is at most 1 level deep, so a single "../" prefix suffices
        // when not already at root. Root-level pages need no prefix.
        const atRoot = !/\/[a-z-]+\/[a-z-]+\.html$/i.test(location.pathname);
        return (atRoot ? "" : "../") + target;
    }

    global.Auth = {
        getSession,
        currentUser,
        isAuthenticated,
        isInRole,
        isStaff,
        login,
        logout,
        register,
        changePassword,
        resetPassword,
        guard,
        resolveFromRoot,
    };
})(window);
