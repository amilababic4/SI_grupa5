(function () {
    "use strict";
    if (!Auth.guard()) return;

    document.getElementById("change-password-form").addEventListener("submit", (e) => {
        e.preventDefault();
        const cur = document.getElementById("current-password").value;
        const p1 = document.getElementById("new-password").value;
        const p2 = document.getElementById("confirm-password").value;
        const alertContainer = document.getElementById("alert-container");
        if (p1 !== p2) {
            alertContainer.innerHTML = '<div class="alert alert-error">Nove lozinke se ne podudaraju.</div>';
            return;
        }
        const result = Auth.changePassword(cur, p1);
        if (!result.ok) {
            alertContainer.innerHTML = `<div class="alert alert-error">${Common.escapeHtml(result.error)}</div>`;
            return;
        }
        Common.Flash.set("success", "Lozinka je uspješno promijenjena.");
        window.location.href = Auth.resolveFromRoot("korisnik/profil.html");
    });
})();
