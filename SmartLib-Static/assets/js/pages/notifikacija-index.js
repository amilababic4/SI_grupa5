(function () {
    "use strict";
    if (!Auth.guard()) return;
    const user = Auth.currentUser();

    function render() {
        const list = DB.query("notifikacije", (n) => n.korisnikId === user.id).sort((a, b) => b.datumKreiranja.localeCompare(a.datumKreiranja));
        const el = document.getElementById("notif-list");
        if (!list.length) { document.getElementById("empty-msg").style.display = "block"; return; }
        el.innerHTML = list.map((n) => `
            <div class="notif-item ${n.procitano ? "" : "unread"}" data-id="${n.id}" data-link="${n.linkUrl || ""}">
                <div>
                    <div class="title">${Common.escapeHtml(n.naslov)}</div>
                    <div>${Common.escapeHtml(n.poruka)}</div>
                    <div class="meta">${Common.formatDate(n.datumKreiranja)} · ${Common.escapeHtml(n.tip)}</div>
                </div>
                ${!n.procitano ? `<button class="btn btn-secondary btn-sm" data-mark="${n.id}">Označi pročitano</button>` : ""}
            </div>
        `).join("");

        el.querySelectorAll(".notif-item").forEach((item) => {
            item.addEventListener("click", (e) => {
                if (e.target.closest("[data-mark]")) return;
                const id = Number(item.getAttribute("data-id"));
                DB.update("notifikacije", id, { procitano: true });
                const link = item.getAttribute("data-link");
                if (link) window.location.href = link;
                else render();
                SmartLibLayout.updateNotifBadge();
            });
            item.setAttribute("tabindex", "0");
            item.addEventListener("keydown", (e) => { if (e.key === "Enter" || e.key === " ") item.click(); });
        });
        el.querySelectorAll("[data-mark]").forEach((btn) => {
            btn.addEventListener("click", (e) => {
                e.stopPropagation();
                DB.update("notifikacije", Number(btn.getAttribute("data-mark")), { procitano: true });
                render();
                SmartLibLayout.updateNotifBadge();
            });
        });
    }

    document.getElementById("mark-all-btn").addEventListener("click", () => {
        DB.query("notifikacije", (n) => n.korisnikId === user.id && !n.procitano).forEach((n) => DB.update("notifikacije", n.id, { procitano: true }));
        render();
        SmartLibLayout.updateNotifBadge();
    });

    render();
})();
