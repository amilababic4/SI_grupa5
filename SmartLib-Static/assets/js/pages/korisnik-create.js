(function () {
    "use strict";
    if (Auth.isAuthenticated()) { window.location.href = Auth.resolveFromRoot("index.html"); return; }

    document.getElementById("create-form").addEventListener("submit", (e) => {
        e.preventDefault();
        const alertContainer = document.getElementById("alert-container");
        const p1 = document.getElementById("password").value;
        const p2 = document.getElementById("confirm-password").value;
        if (p1 !== p2) {
            alertContainer.innerHTML = '<div class="alert alert-error">Lozinke se ne podudaraju.</div>';
            return;
        }
        const result = Auth.register({
            ime: document.getElementById("ime").value.trim(),
            prezime: document.getElementById("prezime").value.trim(),
            email: document.getElementById("email").value.trim(),
            lozinka: p1,
            uloga: "Član",
        });
        if (!result.ok) {
            alertContainer.innerHTML = `<div class="alert alert-error">${Common.escapeHtml(result.error)}</div>`;
            return;
        }
        window.location.href = Auth.resolveFromRoot("auth/login.html") + "?registered=1";
    });
})();
