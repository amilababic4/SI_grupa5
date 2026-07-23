(function () {
    "use strict";

    if (Auth.isAuthenticated()) {
        window.location.href = Auth.resolveFromRoot("index.html");
        return;
    }

    const alertContainer = document.getElementById("alert-container");

    const successMsg = Common.qs("registered");
    if (successMsg) {
        alertContainer.innerHTML = `<div class="validation-message" style="margin-bottom: 1rem;">Registracija uspješna! Prijavite se svojim novim nalogom.</div>`;
    }

    document.getElementById("login-form").addEventListener("submit", (e) => {
        e.preventDefault();
        const email = document.getElementById("email").value.trim();
        const password = document.getElementById("password").value;
        const result = Auth.login(email, password);
        if (!result.ok) {
            alertContainer.innerHTML = `<div class="validation-message" style="margin-bottom: 1rem;">${Common.escapeHtml(result.error)}</div>`;
            return;
        }
        sessionStorage.setItem("smartlib-show-welcome", "true");
        const returnUrl = Common.qs("returnUrl");
        const isStaff = result.user.uloga === "Bibliotekar" || result.user.uloga === "Administrator";
        if (returnUrl) {
            window.location.href = Auth.resolveFromRoot(decodeURIComponent(returnUrl));
        } else if (isStaff) {
            window.location.href = Auth.resolveFromRoot("korisnik/index.html");
        } else {
            window.location.href = Auth.resolveFromRoot("index.html");
        }
    });
})();
