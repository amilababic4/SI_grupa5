(function () {
    "use strict";
    if (!Auth.guard(["Bibliotekar", "Administrator"])) return;
    const staff = Auth.currentUser();

    function render() {
        const prijave = DB.query("recenzijaPrijave", (p) => p.status === "otvorena");
        const tbody = document.getElementById("tbody");
        if (!prijave.length) { document.getElementById("empty-msg").style.display = "block"; tbody.innerHTML = ""; return; }
        tbody.innerHTML = prijave.map((p) => {
            const recenzija = DB.find("recenzije", p.recenzijaId);
            const knjiga = recenzija ? DB.find("knjige", recenzija.knjigaId) : null;
            const autor = recenzija ? DB.find("korisnici", recenzija.korisnikId) : null;
            const prijavio = DB.find("korisnici", p.prijavioKorisnikId);
            return `<tr>
                <td>${knjiga ? `<a href="../knjiga/details.html?id=${knjiga.id}">${Common.escapeHtml(knjiga.naslov)}</a>` : "—"}</td>
                <td>${autor ? Common.escapeHtml(autor.ime + " " + autor.prezime) : "—"}</td>
                <td>${recenzija ? recenzija.ocjena : "—"} ★</td>
                <td>${prijavio ? Common.escapeHtml(prijavio.ime + " " + prijavio.prezime) : "—"}</td>
                <td>${Common.escapeHtml(p.razlog || "—")}</td>
                <td style="display:flex;gap:.4rem;">
                    ${recenzija ? `<button class="btn btn-danger btn-sm" data-remove="${p.id}:${recenzija.id}">Ukloni recenziju</button>` : ""}
                    <button class="btn btn-secondary btn-sm" data-resolve="${p.id}">Razriješi</button>
                </td>
            </tr>`;
        }).join("");

        tbody.querySelectorAll("[data-resolve]").forEach((btn) => btn.addEventListener("click", () => {
            DB.update("recenzijaPrijave", Number(btn.getAttribute("data-resolve")), { status: "razrijesena", razrijesioKorisnikId: staff.id, datumRazrjesenja: new Date().toISOString().slice(0, 10) });
            render();
        }));
        tbody.querySelectorAll("[data-remove]").forEach((btn) => btn.addEventListener("click", () => {
            if (!confirm("Ukloniti ovu recenziju?")) return;
            const [prijavaId, recenzijaId] = btn.getAttribute("data-remove").split(":").map(Number);
            const recenzija = DB.find("recenzije", recenzijaId);
            DB.remove("recenzije", recenzijaId);
            DB.update("recenzijaPrijave", prijavaId, { status: "razrijesena", razrijesioKorisnikId: staff.id, datumRazrjesenja: new Date().toISOString().slice(0, 10) });
            if (recenzija) {
                const author = DB.find("korisnici", recenzija.korisnikId);
                if (author) {
                    const strikes = (author.brojUklonjenihSadrzaja || 0) + 1;
                    const patch = { brojUklonjenihSadrzaja: strikes };
                    if (strikes >= 3) patch.datumZabraneDo = new Date(Date.now() + 7 * 86400000).toISOString().slice(0, 10);
                    DB.update("korisnici", author.id, patch);
                }
            }
            render();
        }));
    }

    render();
})();
