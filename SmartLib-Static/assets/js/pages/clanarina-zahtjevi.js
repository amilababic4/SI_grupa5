(function () {
    "use strict";
    if (!Auth.guard(["Bibliotekar", "Administrator"])) return;
    const staff = Auth.currentUser();

    function render() {
        const zahtjevi = DB.query("zahtjeviProduzenja", (z) => z.status === "na_cekanju").sort((a, b) => a.datumPodnosenja.localeCompare(b.datumPodnosenja));
        const el = document.getElementById("requests-list");
        if (!zahtjevi.length) { document.getElementById("empty-msg").style.display = "block"; el.innerHTML = ""; return; }
        el.innerHTML = zahtjevi.map((z) => {
            const korisnik = DB.find("korisnici", z.korisnikId);
            const clanarina = DB.query("clanarine", (c) => c.korisnikId === z.korisnikId).sort((a, b) => b.datumIsteka.localeCompare(a.datumIsteka))[0];
            const initials = korisnik ? (korisnik.ime[0] + korisnik.prezime[0]).toUpperCase() : "?";
            return `<div class="zahtjev-card" data-id="${z.id}">
                <strong>${initials} ${korisnik ? Common.escapeHtml(korisnik.ime + " " + korisnik.prezime) : "—"}</strong>
                <div class="stat-grid">
                    <span>Trajanje: ${z.trajanjeMjeseci} mjeseci</span>
                    <span>Trenutni datum isteka: ${clanarina ? Common.formatDate(clanarina.datumIsteka) : "—"}</span>
                    <span>Podneseno: ${Common.formatDate(z.datumPodnosenja)}</span>
                </div>
                ${z.napomena ? `<p style="font-size:.88rem;color:var(--sl-muted);">Napomena: ${Common.escapeHtml(z.napomena)}</p>` : ""}
                <div style="display:flex;gap:.5rem;">
                    <button class="btn btn-primary btn-sm" data-approve="${z.id}">Odobri</button>
                    <button class="btn btn-danger btn-sm" data-toggle-reject="${z.id}">Odbij</button>
                </div>
                <div class="reject-form" id="reject-form-${z.id}">
                    <textarea rows="2" placeholder="Razlog odbijanja" id="reject-reason-${z.id}"></textarea>
                    <button class="btn btn-danger btn-sm" data-confirm-reject="${z.id}" style="margin-top:.4rem;">Potvrdi odbijanje</button>
                </div>
            </div>`;
        }).join("");

        el.querySelectorAll("[data-approve]").forEach((btn) => btn.addEventListener("click", () => {
            const id = Number(btn.getAttribute("data-approve"));
            const z = DB.find("zahtjeviProduzenja", id);
            const clanarina = DB.query("clanarine", (c) => c.korisnikId === z.korisnikId).sort((a, b) => b.datumIsteka.localeCompare(a.datumIsteka))[0];
            const today = new Date().toISOString().slice(0, 10);
            const base = new Date((clanarina && clanarina.datumIsteka > today ? clanarina.datumIsteka : today) + "T00:00:00");
            base.setMonth(base.getMonth() + z.trajanjeMjeseci);
            const noviDatum = base.toISOString().slice(0, 10);
            if (clanarina) DB.update("clanarine", clanarina.id, { datumIsteka: noviDatum });
            else DB.insert("clanarine", { korisnikId: z.korisnikId, datumPocetka: today, datumIsteka: noviDatum });
            DB.update("zahtjeviProduzenja", id, { status: "odobreno", datumObrade: today, obradioKorisnikId: staff.id, noviDatumIsteka: noviDatum });
            DB.insert("notifikacije", { korisnikId: z.korisnikId, naslov: "Članarina produžena", poruka: `Vaša članarina je produžena do ${Common.formatDate(noviDatum)}.`, tip: "Sistem", linkUrl: null, procitano: false, datumKreiranja: today });
            render();
        }));
        el.querySelectorAll("[data-toggle-reject]").forEach((btn) => btn.addEventListener("click", () => {
            document.getElementById("reject-form-" + btn.getAttribute("data-toggle-reject")).classList.toggle("open");
        }));
        el.querySelectorAll("[data-confirm-reject]").forEach((btn) => btn.addEventListener("click", () => {
            const id = Number(btn.getAttribute("data-confirm-reject"));
            const razlog = document.getElementById("reject-reason-" + id).value.trim();
            const today = new Date().toISOString().slice(0, 10);
            const z = DB.find("zahtjeviProduzenja", id);
            DB.update("zahtjeviProduzenja", id, { status: "odbijeno", datumObrade: today, obradioKorisnikId: staff.id, razlogOdbijanja: razlog });
            DB.insert("notifikacije", { korisnikId: z.korisnikId, naslov: "Zahtjev za produženje odbijen", poruka: razlog || "Vaš zahtjev za produženje članarine je odbijen.", tip: "Sistem", linkUrl: null, procitano: false, datumKreiranja: today });
            render();
        }));
    }

    render();
})();
