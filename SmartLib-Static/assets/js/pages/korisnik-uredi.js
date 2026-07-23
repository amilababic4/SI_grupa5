(function () {
    "use strict";
    if (!Auth.guard(["Administrator"])) return;

    const id = Number(Common.qs("id"));
    const korisnik = DB.find("korisnici", id);
    if (!korisnik) { document.querySelector("main").innerHTML = "<p>Korisnik nije pronađen.</p>"; return; }

    document.getElementById("back-link").setAttribute("href", "profil.html?id=" + id);
    document.getElementById("cancel-link").setAttribute("href", "profil.html?id=" + id);
    document.getElementById("avatar-initials").textContent = (korisnik.ime[0] + korisnik.prezime[0]).toUpperCase();
    document.getElementById("id-badge").textContent = "#" + korisnik.id;
    document.getElementById("header-ime").textContent = korisnik.ime + " " + korisnik.prezime;
    document.getElementById("header-uloga").textContent = korisnik.uloga;
    document.getElementById("ime").value = korisnik.ime;
    document.getElementById("prezime").value = korisnik.prezime;

    const isAdminAccount = korisnik.uloga === "Administrator";
    const section = document.getElementById("role-status-section");
    if (isAdminAccount) {
        section.innerHTML = `
            <div class="profil-grid">
                <div class="details-item"><span class="details-label">Uloga</span><span class="katalog-badge">${Common.escapeHtml(korisnik.uloga)}</span></div>
                <div class="details-item"><span class="details-label">Status računa</span><span class="status-dostupan">aktivan</span></div>
            </div>
            <p class="uredi-hint" style="margin-top:0.65rem;">⚠️ Uloga i status administratora ne mogu se mijenjati.</p>`;
    } else {
        section.innerHTML = `
            <div class="profil-grid">
                <div class="form-group details-item" style="display:flex; flex-direction:column; gap:0.35rem;">
                    <label for="uloga" class="details-label">Uloga</label>
                    <select id="uloga"><option value="Član">Član</option><option value="Bibliotekar">Bibliotekar</option></select>
                </div>
                <div class="form-group details-item" style="display:flex; flex-direction:column; gap:0.35rem;">
                    <label for="status" class="details-label">Status računa</label>
                    <select id="status"><option value="aktivan">Aktivan</option><option value="deaktiviran">Deaktiviran</option></select>
                </div>
            </div>`;
        document.getElementById("uloga").value = korisnik.uloga;
        document.getElementById("status").value = korisnik.status;
    }

    document.getElementById("edit-form").addEventListener("submit", (e) => {
        e.preventDefault();
        const patch = {
            ime: document.getElementById("ime").value.trim(),
            prezime: document.getElementById("prezime").value.trim(),
        };
        if (!isAdminAccount) {
            patch.uloga = document.getElementById("uloga").value;
            patch.status = document.getElementById("status").value;
        }
        const newPass = document.getElementById("new-password").value;
        const confirmPass = document.getElementById("confirm-password").value;
        if (newPass || confirmPass) {
            if (newPass !== confirmPass) {
                document.getElementById("alert-container").innerHTML = '<div class="alert alert-error">Lozinke se ne podudaraju.</div>';
                return;
            }
            patch.lozinka = newPass;
        }
        DB.update("korisnici", id, patch);
        Common.Flash.set("success", "Korisnik je ažuriran.");
        window.location.href = "profil.html?id=" + id;
    });
})();
