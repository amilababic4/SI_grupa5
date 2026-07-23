(function () {
    "use strict";
    document.getElementById("forgot-form").addEventListener("submit", (e) => {
        e.preventDefault();
        const email = document.getElementById("email").value.trim();
        sessionStorage.setItem("smartlib-reset-email", email);
        window.location.href = Auth.resolveFromRoot("auth/forgot-password-confirmation.html");
    });
})();
