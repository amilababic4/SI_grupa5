(function () {
    "use strict";
    const savedEmail = sessionStorage.getItem("smartlib-reset-email");
    if (savedEmail) document.getElementById("email").value = savedEmail;

    document.getElementById("reset-form").addEventListener("submit", (e) => {
        e.preventDefault();
        const email = document.getElementById("email").value.trim();
        const p1 = document.getElementById("new-password").value;
        const p2 = document.getElementById("confirm-password").value;
        const alertContainer = document.getElementById("alert-container");
        if (p1 !== p2) {
            alertContainer.innerHTML = '<div class="validation-message" style="margin-bottom:1rem;">Lozinke se ne podudaraju.</div>';
            return;
        }
        const result = Auth.resetPassword(email, p1);
        if (!result.ok) {
            alertContainer.innerHTML = `<div class="validation-message" style="margin-bottom:1rem;">${Common.escapeHtml(result.error)}</div>`;
            return;
        }
        sessionStorage.removeItem("smartlib-reset-email");
        window.location.href = Auth.resolveFromRoot("auth/reset-password-confirmation.html");
    });
})();
