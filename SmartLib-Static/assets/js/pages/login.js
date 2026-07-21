(function () {
    "use strict";

    if (Auth.isAuthenticated()) {
        window.location.href = Auth.resolveFromRoot("index.html");
        return;
    }

    const alertContainer = document.getElementById("alert-container");
    Common.Flash.renderInto(alertContainer);

    const successMsg = Common.qs("registered");
    if (successMsg) {
        const div = document.createElement("div");
        div.className = "alert alert-success";
        div.textContent = "Registracija uspješna! Prijavite se svojim novim nalogom.";
        alertContainer.appendChild(div);
    }

    document.getElementById("login-form").addEventListener("submit", (e) => {
        e.preventDefault();
        const email = document.getElementById("email").value.trim();
        const password = document.getElementById("password").value;
        const result = Auth.login(email, password);
        if (!result.ok) {
            alertContainer.innerHTML = `<div class="alert alert-error">${Common.escapeHtml(result.error)}</div>`;
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
