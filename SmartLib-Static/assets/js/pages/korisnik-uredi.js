(function () {
    "use strict";
    if (!Auth.guard(["Administrator"])) return;

    const id = Number(Common.qs("id"));
    const korisnik = DB.find("korisnici", id);
    if (!korisnik) { document.querySelector("main").innerHTML = "<p>Korisnik nije pronađen.</p>"; return; }

    document.getElementById("back-link").setAttribute("href", "profil.html?id=" + id);
    document.getElementById("ime").value = korisnik.ime;
    document.getElementById("prezime").value = korisnik.prezime;
    document.getElementById("email").value = korisnik.email;
    document.getElementById("uloga").value = korisnik.uloga;
    document.getElementById("status").value = korisnik.status;

    const isAdminAccount = korisnik.uloga === "Administrator";
    if (isAdminAccount) {
        document.getElementById("uloga").disabled = true;
        document.getElementById("status").disabled = true;
        document.getElementById("admin-note").style.display = "block";
    }

    document.getElementById("edit-form").addEventListener("submit", (e) => {
        e.preventDefault();
        const patch = {
            ime: document.getElementById("ime").value.trim(),
            prezime: document.getElementById("prezime").value.trim(),
            email: document.getElementById("email").value.trim(),
        };
        if (!isAdminAccount) {
            patch.uloga = document.getElementById("uloga").value;
            patch.status = document.getElementById("status").value;
        }
        const newPass = document.getElementById("new-password").value;
        if (newPass) patch.lozinka = newPass;
        DB.update("korisnici", id, patch);
        Common.Flash.set("success", "Korisnik je ažuriran.");
        window.location.href = "profil.html?id=" + id;
    });
})();
