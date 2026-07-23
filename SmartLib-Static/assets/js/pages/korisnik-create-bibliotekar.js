(function () {
    "use strict";
    if (!Auth.guard(["Administrator"])) return;

    document.getElementById("create-biblio-form").addEventListener("submit", (e) => {
        e.preventDefault();
        const alertContainer = document.getElementById("alert-container");
        const p1 = document.getElementById("password").value;
        const p2 = document.getElementById("confirm-password").value;
        if (p1 !== p2) { alertContainer.innerHTML = '<div class="alert alert-error">Lozinke se ne podudaraju.</div>'; return; }
        const email = document.getElementById("email").value.trim();
        if (DB.query("korisnici", (u) => u.email.toLowerCase() === email.toLowerCase()).length) {
            alertContainer.innerHTML = '<div class="alert alert-error">Korisnik s ovim emailom već postoji.</div>';
            return;
        }
        DB.insert("korisnici", {
            ime: document.getElementById("ime").value.trim(),
            prezime: document.getElementById("prezime").value.trim(),
            email,
            lozinka: p1,
            uloga: "Bibliotekar",
            status: "aktivan",
            datumKreiranja: new Date().toISOString().slice(0, 10),
            brojUklonjenihSadrzaja: 0,
            datumZabraneDo: null,
            listaZeljaJavna: false,
        });
        Common.Flash.set("success", "Bibliotekar je uspješno kreiran.");
        window.location.href = "index-bibliotekar.html";
    });
})();
