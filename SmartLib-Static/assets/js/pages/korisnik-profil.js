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
    const jeAdmin = Auth.isInRole("Administrator");
    const jeBibliotekarIliAdmin = Auth.isStaff();
    const targetIsAdmin = profil.uloga === "Administrator";
    const statusAktivan = profil.status === "aktivan";

    document.title = (jeMojProfil ? "Moj profil" : profil.ime + " " + profil.prezime) + " — SmartLib";
    document.getElementById("back-link").setAttribute("href", jeMojProfil ? "../knjiga/index.html" : "index.html");
    Common.Flash.renderInto(document.getElementById("alert-container"));

    document.getElementById("avatar-initials").textContent = (profil.ime[0] + profil.prezime[0]).toUpperCase();
    document.getElementById("profil-ime").textContent = profil.ime + " " + profil.prezime;
    document.getElementById("profil-uloga").textContent = profil.uloga;
    document.getElementById("grid-ime").textContent = profil.ime;
    document.getElementById("grid-prezime").textContent = profil.prezime;
    document.getElementById("grid-email").textContent = profil.email;
    document.getElementById("grid-uloga-badge").innerHTML = `<span class="katalog-badge">${Common.escapeHtml(profil.uloga)}</span>`;
    document.getElementById("grid-status").innerHTML = `<span class="${statusAktivan ? "status-dostupan" : "status-deaktiviran"}">${Common.escapeHtml(profil.status)}</span>`;
    document.getElementById("grid-datum").textContent = Common.formatDate(profil.datumKreiranja);

    // ── Header actions ────────────────────────────────────────────
    let actionsHtml = "";
    if (jeAdmin) actionsHtml += `<a href="uredi.html?id=${profil.id}" class="btn btn-primary">Uredi profil</a>`;
    if (jeMojProfil) actionsHtml += `<a href="../auth/change-password.html" class="btn btn-password"><svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="11" width="18" height="11" rx="2" ry="2" /><path d="M7 11V7a5 5 0 0 1 10 0v4" /></svg>Promjena lozinke</a>`;
    if (jeMojProfil && statusAktivan && !jeAdmin) actionsHtml += `<button type="button" class="btn btn-danger" id="deactivate-self-btn">Deaktiviraj nalog</button>`;
    if (!jeMojProfil && jeBibliotekarIliAdmin) {
        if (statusAktivan) { if (!targetIsAdmin) actionsHtml += `<button type="button" class="btn btn-danger" id="deactivate-btn">Deaktiviraj nalog</button>`; }
        else actionsHtml += `<button type="button" class="btn btn-primary" id="activate-btn">Reaktiviraj nalog</button>`;
    }
    document.getElementById("profile-actions").innerHTML = actionsHtml;

    const deactSelfBtn = document.getElementById("deactivate-self-btn");
    if (deactSelfBtn) deactSelfBtn.addEventListener("click", () => {
        if (!confirm("Da li ste sigurni da zelite deaktivirati nalog?")) return;
        DB.update("korisnici", profil.id, { status: "deaktiviran" });
        Auth.logout();
        window.location.href = Auth.resolveFromRoot("index.html");
    });
    const deactBtn = document.getElementById("deactivate-btn");
    if (deactBtn) deactBtn.addEventListener("click", () => {
        if (!confirm("Deaktivirati nalog korisnika?")) return;
        DB.update("korisnici", profil.id, { status: "deaktiviran" });
        window.location.reload();
    });
    const actBtn = document.getElementById("activate-btn");
    if (actBtn) actBtn.addEventListener("click", () => {
        if (!confirm("Reaktivirati nalog korisnika?")) return;
        DB.update("korisnici", profil.id, { status: "aktivan" });
        window.location.reload();
    });

    // ── Membership (non-Bibliotekar users only, mirrors Uloga.Id != 2) ──
    if (profil.uloga !== "Bibliotekar") {
        document.getElementById("membership-block").style.display = "block";
        const clanarina = DB.query("clanarine", (c) => c.korisnikId === profil.id).sort((a, b) => b.datumIsteka.localeCompare(a.datumIsteka))[0];
        const today = new Date().toISOString().slice(0, 10);
        const link = document.getElementById("membership-manage-link");
        if (jeBibliotekarIliAdmin && !jeMojProfil) {
            link.style.display = "";
            link.textContent = clanarina ? "Upravljanje članarinom" : "Evidentiraj članarinu";
            link.setAttribute("href", "../clanarina/upsert.html?korisnikId=" + profil.id);
        } else if (jeMojProfil && profil.uloga === "Član") {
            link.style.display = "";
            link.textContent = "Produži članarinu";
            link.setAttribute("href", "../clanarina/produzenje.html");
        }
        const body = document.getElementById("membership-body");
        if (clanarina) {
            const aktivna = clanarina.datumIsteka >= today;
            body.innerHTML = `<div class="profil-grid" style="margin-top: 0.5rem;">
                <div class="details-item"><span class="details-label">Datum početka</span><span>${Common.formatDate(clanarina.datumPocetka)}</span></div>
                <div class="details-item"><span class="details-label">Važi do</span><span>${Common.formatDate(clanarina.datumIsteka)}</span></div>
                <div class="details-item profil-grid-full"><span class="details-label">Status članarine</span>${aktivna ? '<span class="status-dostupan">aktivna</span>' : '<span class="status-deaktiviran">istekla</span>'}</div>
            </div>`;
        } else {
            body.innerHTML = `<p class="profil-clanarina-prazno">Članarina nije evidentirana.</p>`;
        }

        if (jeBibliotekarIliAdmin && !jeMojProfil) {
            document.getElementById("staff-links-block").style.display = "flex";
            document.getElementById("link-zaduzenja-aktivna").setAttribute("href", "../zaduzenje/zaduzenja-clana.html?korisnikId=" + profil.id);
            document.getElementById("link-zaduzenja-historija").setAttribute("href", "../zaduzenje/historija-clana.html?korisnikId=" + profil.id);
        }
    }

    // ── Achievements ──────────────────────────────────────────────
    if (profil.uloga === "Član") {
        const closedLoans = DB.query("zaduzenja", (z) => z.korisnikId === profil.id && z.status === "zatvoreno");
        const reservations = DB.query("rezervacije", (r) => r.korisnikId === profil.id);
        const achievements = [];
        if (closedLoans.length >= 1) achievements.push(["Prva pročitana knjiga", "Pročitali ste svoju prvu knjigu."]);
        if (closedLoans.length >= 5) achievements.push(["5 pročitanih knjiga", "Pročitali ste 5 knjiga!"]);
        if (reservations.length >= 1) achievements.push(["Prva rezervacija", "Napravili ste svoju prvu rezervaciju."]);
        const categories = new Set(closedLoans.map((z) => { const k = DB.find("knjige", z.knjigaId); return k ? k.kategorijaId : null; }));
        if (categories.size >= 2) achievements.push(["Promijenjen ukus", "Čitate knjige iz više različitih kategorija."]);
        if (achievements.length) {
            document.getElementById("achievements-block").style.display = "block";
            document.getElementById("achievements-list").innerHTML = achievements.map(([t, d]) =>
                `<div class="details-item"><span class="details-label">${Common.escapeHtml(t)}</span><span class="details-value">${Common.escapeHtml(d)}</span></div>`).join("");
        }
    }

    // ── Collections ───────────────────────────────────────────────
    if (profil.uloga === "Član") {
        const kolekcije = DB.query("listaKolekcija", (l) => l.korisnikId === profil.id).slice(0, 5);
        if (kolekcije.length) {
            document.getElementById("collections-block").style.display = "block";
            if (jeMojProfil) {
                const link = document.getElementById("collections-manage-link");
                link.style.display = "";
                link.setAttribute("href", "../lista-kolekcija/index.html");
            }
            document.getElementById("collections-list").innerHTML = kolekcije.map((k) =>
                `<div class="details-item"><span class="details-label">${Common.escapeHtml(k.naziv)}</span><span class="details-value"><a href="../lista-kolekcija/${jeMojProfil ? "details" : "details"}.html?id=${k.id}">${k.javna ? "Javna kolekcija" : "Privatna kolekcija"}</a></span></div>`
            ).join("");
        }
    }
})();
