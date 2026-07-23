(function () {
    "use strict";
    if (!Auth.guard(["Bibliotekar", "Administrator"])) return;
    const user = Auth.currentUser();
    Common.Flash.renderInto(document.getElementById("alert-container"));

    function getEmailSetting() {
        const setting = DB.query("appPostavke", (s) => s.kljuc === "DistributerEmail")[0];
        return setting ? setting.vrijednost : "";
    }

    document.getElementById("distributer-email").value = getEmailSetting();

    function renderRecent() {
        const recent = DB.getAll("nabavkaZahtjevi").sort((a, b) => b.vrijemePodnosenja.localeCompare(a.vrijemePodnosenja)).slice(0, 5);
        document.getElementById("recent-requests").innerHTML = recent.length ? recent.map((n) => `
            <div class="nabavka-item">
                <strong>${Common.escapeHtml(n.nazivKnjige)}</strong> — ${Common.escapeHtml(n.autor)}
                <div style="font-size:.82rem;color:var(--sl-muted);">${Common.escapeHtml(n.izdavac || "")} · ${n.brojPrimjeraka} primjeraka · ${Common.formatDate(n.vrijemePodnosenja)}</div>
                <span class="status-badge ${n.emailPoslan ? "status-badge--aktivno" : "status-badge--zatvoreno"}">${n.emailPoslan ? "Email poslan" : "Email nije poslan"}</span>
            </div>
        `).join("") : "<p style='color:var(--sl-muted);'>Nema prethodnih zahtjeva.</p>";
    }

    document.getElementById("nabavka-form").addEventListener("submit", (e) => {
        e.preventDefault();
        DB.insert("nabavkaZahtjevi", {
            nazivKnjige: document.getElementById("naziv-knjige").value.trim(),
            autor: document.getElementById("autor").value.trim(),
            izdavac: document.getElementById("izdavac").value.trim(),
            brojPrimjeraka: Number(document.getElementById("broj-primjeraka").value) || 1,
            napomena: document.getElementById("napomena").value.trim(),
            vrijemePodnosenja: new Date().toISOString().slice(0, 10),
            emailPoslan: !!getEmailSetting(),
            podnosilacId: user.id,
        });
        document.getElementById("nabavka-form").reset();
        document.getElementById("broj-primjeraka").value = 1;
        Common.Flash.set("success", "Zahtjev za nabavku je poslan.");
        window.location.reload();
    });

    document.getElementById("email-form").addEventListener("submit", (e) => {
        e.preventDefault();
        const value = document.getElementById("distributer-email").value.trim();
        const existing = DB.query("appPostavke", (s) => s.kljuc === "DistributerEmail")[0];
        if (existing) DB.update("appPostavke", existing.id, { vrijednost: value });
        else DB.insert("appPostavke", { kljuc: "DistributerEmail", vrijednost: value });
        Common.Flash.set("success", "Email distributera je ažuriran.");
        window.location.reload();
    });

    renderRecent();
})();
