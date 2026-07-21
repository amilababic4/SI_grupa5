(function () {
    "use strict";
    if (!Auth.guard()) return;

    const me = Auth.currentUser();
    const idParam = Common.qs("id");
    const viewingOther = idParam && Number(idParam) !== me.id;
    if (viewingOther && !Auth.isStaff()) { window.location.href = Auth.resolveFromRoot("index.html"); return; }
    const profil = viewingOther ? DB.find("korisnici", Number(idParam)) : me;
    if (!profil) { document.querySelector("main").innerHTML = "<p>Korisnik nije pronađen.</p>"; return; }

    const jeMojProfil = profil.id === me.id;

    Common.Flash.renderInto(document.getElementById("alert-container"));

    document.getElementById("avatar-initials").textContent = (profil.ime[0] + profil.prezime[0]).toUpperCase();
    document.getElementById("profil-ime").textContent = profil.ime + " " + profil.prezime;
    document.getElementById("profil-email").textContent = profil.email;
    document.getElementById("profil-role-status").innerHTML = `<span class="status-badge status-badge--zatvoreno">${Common.escapeHtml(profil.uloga)}</span> ` + Common.statusBadgeHtml(profil.status);

    const actions = document.getElementById("profile-actions");
    let actionsHtml = "";
    if (jeMojProfil) {
        actionsHtml += `<a href="../auth/change-password.html" class="btn btn-secondary">Promjena lozinke</a>`;
        if (!Auth.isStaff()) actionsHtml += `<button class="btn btn-danger" id="deactivate-self-btn">Deaktiviraj nalog</button>`;
    }
    if (Auth.isStaff() && (viewingOther || Auth.isInRole("Administrator"))) {
        actionsHtml += `<a href="uredi.html?id=${profil.id}" class="btn btn-secondary">Uredi profil</a>`;
    }
    if (Auth.isStaff() && viewingOther && profil.uloga === "Član") {
        actionsHtml += profil.status === "aktivan"
            ? `<button class="btn btn-danger" id="deactivate-btn">Deaktiviraj</button>`
            : `<button class="btn btn-primary" id="activate-btn">Aktiviraj</button>`;
    }
    actions.innerHTML = actionsHtml;

    if (document.getElementById("deactivate-self-btn")) {
        document.getElementById("deactivate-self-btn").addEventListener("click", () => {
            if (!confirm("Da li ste sigurni da želite deaktivirati svoj nalog?")) return;
            DB.update("korisnici", profil.id, { status: "deaktiviran" });
            Auth.logout();
            window.location.href = Auth.resolveFromRoot("index.html");
        });
    }
    if (document.getElementById("deactivate-btn")) {
        document.getElementById("deactivate-btn").addEventListener("click", () => {
            DB.update("korisnici", profil.id, { status: "deaktiviran" });
            window.location.reload();
        });
    }
    if (document.getElementById("activate-btn")) {
        document.getElementById("activate-btn").addEventListener("click", () => {
            DB.update("korisnici", profil.id, { status: "aktivan" });
            window.location.reload();
        });
    }

    // ── Membership (members only) ──────────────────────────────────
    if (profil.uloga === "Član") {
        const clanarina = DB.query("clanarine", (c) => c.korisnikId === profil.id).sort((a, b) => b.datumIsteka.localeCompare(a.datumIsteka))[0];
        document.getElementById("membership-block").style.display = "block";
        const today = new Date().toISOString().slice(0, 10);
        const grid = document.getElementById("membership-grid");
        if (clanarina) {
            const aktivna = clanarina.datumIsteka >= today;
            grid.innerHTML = `
                <div><div class="label">Status</div><div class="value">${aktivna ? "Aktivna" : "Istekla"}</div></div>
                <div><div class="label">Početak</div><div class="value">${Common.formatDate(clanarina.datumPocetka)}</div></div>
                <div><div class="label">Ističe</div><div class="value">${Common.formatDate(clanarina.datumIsteka)}</div></div>
            `;
            if (jeMojProfil) grid.insertAdjacentHTML("beforeend", `<div style="grid-column:1/-1;"><a href="../clanarina/produzenje.html" class="btn btn-secondary btn-sm">Produži članarinu</a></div>`);
            else if (Auth.isStaff()) grid.insertAdjacentHTML("beforeend", `<div style="grid-column:1/-1;"><a href="../clanarina/upsert.html?korisnikId=${profil.id}" class="btn btn-secondary btn-sm">Uredi članarinu</a></div>`);
        } else {
            grid.innerHTML = `<div class="value">Nema evidentirane članarine.</div>`;
        }
    }

    // ── Staff quick links ───────────────────────────────────────────
    if (Auth.isStaff() && viewingOther) {
        document.getElementById("staff-links-block").style.display = "block";
        document.getElementById("staff-links").innerHTML = `
            <a href="../zaduzenje/zaduzenja-clana.html?korisnikId=${profil.id}" class="btn btn-secondary btn-sm">Aktivna zaduženja</a>
            <a href="../zaduzenje/historija-clana.html?korisnikId=${profil.id}" class="btn btn-secondary btn-sm">Historija zaduženja</a>
        `;
    }

    // ── Achievements (own profile, members only) ────────────────────
    if (profil.uloga === "Član") {
        const closedLoans = DB.query("zaduzenja", (z) => z.korisnikId === profil.id && z.status === "zatvoreno");
        const reservations = DB.query("rezervacije", (r) => r.korisnikId === profil.id);
        const achievements = [];
        if (closedLoans.length >= 1) achievements.push(["Prva pročitana knjiga", "Pročitali ste svoju prvu knjigu."]);
        if (closedLoans.length >= 5) achievements.push(["5 pročitanih knjiga", "Pročitali ste 5 knjiga!"]);
        if (reservations.length >= 1) achievements.push(["Prva rezervacija", "Napravili ste svoju prvu rezervaciju."]);
        const categories = new Set(closedLoans.map((z) => { const k = DB.find("knjige", z.knjigaId); return k ? k.kategorijaId : null; }));
        if (categories.size >= 2) achievements.push(["Promijenjen ukus", "Čitate knjige iz više različitih kategorija."]);
        document.getElementById("achievements-block").style.display = "block";
        const list = document.getElementById("achievements-list");
        list.innerHTML = achievements.length
            ? achievements.map(([t, d]) => `<span class="achievement-chip" title="${Common.escapeHtml(d)}">🏆 ${Common.escapeHtml(t)}</span>`).join("")
            : "<p style='color:var(--sl-muted);'>Još nema osvojenih postignuća.</p>";
    }

    // ── Collections preview ──────────────────────────────────────────
    if (profil.uloga === "Član" && jeMojProfil) {
        const kolekcije = DB.query("listaKolekcija", (l) => l.korisnikId === profil.id).slice(0, 5);
        document.getElementById("collections-block").style.display = "block";
        document.getElementById("collections-preview").innerHTML = kolekcije.length
            ? kolekcije.map((k) => `<a href="../lista-kolekcija/details.html?id=${k.id}">${Common.escapeHtml(k.naziv)}</a>`).join("")
            : "<p style='color:var(--sl-muted);'>Nemate kreiranih kolekcija.</p>";
    }
})();
