(function () {
    "use strict";
    if (!Auth.guard()) return;

    const user = Auth.currentUser();
    const jeClan = user.uloga === "Član";
    const bookId = Number(Common.qs("id"));
    const knjiga = DB.find("knjige", bookId);

    if (!knjiga) {
        document.querySelector("main").innerHTML = "<p>Knjiga nije pronađena.</p>";
        return;
    }

    function primjerci() { return DB.query("primjerci", (p) => p.knjigaId === bookId); }
    function recenzije() { return DB.query("recenzije", (r) => r.knjigaId === bookId); }

    function renderHeader() {
        const kategorija = DB.find("kategorije", knjiga.kategorijaId);
        const primj = primjerci();
        const rec = recenzije();
        const prosjek = rec.length ? rec.reduce((s, r) => s + r.ocjena, 0) / rec.length : 0;
        const dostupno = primj.filter((p) => p.status === "dostupan").length;

        document.title = knjiga.naslov + " — SmartLib";
        document.getElementById("book-cover-img").src = Common.bookCoverUrl(knjiga.naslov);
        document.getElementById("book-cover-img").alt = knjiga.naslov;
        document.getElementById("book-naslov").textContent = knjiga.naslov;
        document.getElementById("book-autor").textContent = knjiga.autor + (knjiga.godinaIzdanja ? ", " + knjiga.godinaIzdanja : "");
        document.getElementById("book-rating").innerHTML = Common.starsHtml(prosjek, rec.length) +
            (rec.length ? ` <strong>${prosjek.toFixed(1)}</strong> <span style="color:var(--sl-muted);">(${rec.length} recenzija)</span>` : ` <span style="color:var(--sl-muted);">Nema recenzija</span>`);
        document.getElementById("book-isbn").textContent = knjiga.isbn;
        document.getElementById("book-kategorija").textContent = kategorija ? kategorija.naziv : "—";
        document.getElementById("book-izdavac").textContent = knjiga.izdavac || "—";
        document.getElementById("book-godina").textContent = knjiga.godinaIzdanja || "—";
        document.getElementById("book-primjerci-count").textContent = primj.length;
        document.getElementById("book-dostupno").textContent = dostupno;
        document.getElementById("book-opis").textContent = knjiga.opis || "";

        document.getElementById("book-cover-img").addEventListener("click", () => {
            document.getElementById("image-modal-img").src = Common.bookCoverUrl(knjiga.naslov);
            document.getElementById("image-modal").hidden = false;
        });

        document.getElementById("edit-book-btn").setAttribute("href", "edit.html?id=" + bookId);
        document.getElementById("delete-book-btn").addEventListener("click", () => {
            if (!confirm("Obrisati ovu knjigu i sve njene primjerke?")) return;
            primjerci().forEach((p) => DB.remove("primjerci", p.id));
            DB.remove("knjige", bookId);
            Common.Flash.set("success", "Knjiga je obrisana.");
            window.location.href = "index.html";
        });

        if (jeClan) setupMemberActions(dostupno);
    }

    function setupMemberActions(dostupno) {
        const reserveBtn = document.getElementById("reserve-btn");
        const activeRez = DB.query("rezervacije", (r) => r.korisnikId === user.id && r.knjigaId === bookId && r.status === "aktivna")[0];
        const kasneca = DB.query("zaduzenja", (z) => z.korisnikId === user.id && z.status === "zakašnjelo").length > 0;

        if (dostupno > 0) {
            reserveBtn.textContent = "Dostupno je za zaduženje u biblioteci";
            reserveBtn.disabled = true;
        } else if (activeRez) {
            reserveBtn.textContent = "Već rezervisano";
            reserveBtn.disabled = true;
        } else if (kasneca) {
            reserveBtn.textContent = "Imate kašnjenje — rezervacija onemogućena";
            reserveBtn.disabled = true;
        } else {
            reserveBtn.addEventListener("click", () => {
                DB.insert("rezervacije", {
                    korisnikId: user.id,
                    knjigaId: bookId,
                    datumRezervacije: new Date().toISOString().slice(0, 10),
                    datumIsteka: new Date(Date.now() + 7 * 86400000).toISOString().slice(0, 10),
                    status: "aktivna",
                });
                Common.Flash.set("success", "Knjiga je uspješno rezervisana.");
                window.location.reload();
            });
        }

        document.getElementById("collection-btn").addEventListener("click", openCollectionModal);
    }

    function openCollectionModal() {
        const kolekcije = DB.query("listaKolekcija", (l) => l.korisnikId === user.id);
        const list = document.getElementById("collection-modal-list");
        if (!kolekcije.length) {
            list.innerHTML = "<p>Nemate kreiranih kolekcija. Kreirajte jednu na stranici Kolekcije.</p>";
        } else {
            list.innerHTML = kolekcije.map((k) => {
                const already = DB.query("listaKolekcijaStavke", (s) => s.listaKolekcijaId === k.id && s.knjigaId === bookId).length > 0;
                return `<div style="display:flex;justify-content:space-between;align-items:center;padding:.5rem 0;border-bottom:1px solid var(--sl-border);">
                    <span>${Common.escapeHtml(k.naziv)}</span>
                    <button class="btn btn-sm ${already ? "btn-secondary" : "btn-primary"}" data-kat-id="${k.id}" ${already ? "disabled" : ""}>${already ? "Dodano" : "Dodaj"}</button>
                </div>`;
            }).join("");
            list.querySelectorAll("button[data-kat-id]").forEach((btn) => {
                btn.addEventListener("click", () => {
                    const listaId = Number(btn.getAttribute("data-kat-id"));
                    const redoslijed = DB.query("listaKolekcijaStavke", (s) => s.listaKolekcijaId === listaId).length + 1;
                    DB.insert("listaKolekcijaStavke", { listaKolekcijaId: listaId, knjigaId: bookId, redoslijed, datumDodavanja: new Date().toISOString().slice(0, 10) });
                    btn.textContent = "Dodano";
                    btn.disabled = true;
                    btn.classList.replace("btn-primary", "btn-secondary");
                    showToast("Dodano u kolekciju.");
                });
            });
        }
        document.getElementById("collection-modal").hidden = false;
    }
    document.getElementById("collection-modal-close").addEventListener("click", () => { document.getElementById("collection-modal").hidden = true; });
    document.getElementById("image-modal").addEventListener("click", () => { document.getElementById("image-modal").hidden = true; });

    function showToast(msg) {
        const t = document.createElement("div");
        t.className = "sl-toast";
        t.textContent = msg;
        document.body.appendChild(t);
        setTimeout(() => t.remove(), 2500);
    }

    function renderPrimjerci() {
        const tbody = document.getElementById("primjerci-tbody");
        tbody.innerHTML = primjerci().map((p) => `
            <tr>
                <td><a href="../zaduzenje/zaduzenja-primjerka.html?id=${p.id}">${Common.escapeHtml(p.inventarniBroj)}</a></td>
                <td>${Common.statusBadgeHtml(p.status)}</td>
                <td>${Common.formatDate(p.datumNabave)}</td>
                <td>${p.status !== "deaktiviran" ? `<button class="btn btn-secondary btn-sm" data-deact="${p.id}">Deaktiviraj</button>` : ""}</td>
            </tr>
        `).join("");
        tbody.querySelectorAll("[data-deact]").forEach((btn) => {
            btn.addEventListener("click", () => {
                const id = Number(btn.getAttribute("data-deact"));
                const p = DB.find("primjerci", id);
                if (p.status === "zadužen") { alert("Ne možete deaktivirati primjerak koji je trenutno zadužen."); return; }
                DB.update("primjerci", id, { status: "deaktiviran" });
                renderPrimjerci();
                renderHeader();
            });
        });
    }

    document.getElementById("add-primjerak-form").addEventListener("submit", (e) => {
        e.preventDefault();
        const n = Number(document.getElementById("broj-novih").value) || 1;
        const existing = primjerci();
        let seq = existing.length + 1;
        for (let i = 0; i < n; i++) {
            DB.insert("primjerci", { knjigaId: bookId, inventarniBroj: `INV-${bookId}-${seq}`, status: "dostupan", datumNabave: new Date().toISOString().slice(0, 10) });
            seq++;
        }
        renderPrimjerci();
        renderHeader();
    });

    function renderReviews() {
        const rec = recenzije();
        const el = document.getElementById("reviews-list");
        if (!rec.length) { el.innerHTML = "<p style='color:var(--sl-muted);'>Još nema recenzija za ovu knjigu.</p>"; return; }
        el.innerHTML = rec.slice().reverse().map((r) => {
            const autor = DB.find("korisnici", r.korisnikId);
            return `<div class="review-item">
                <div style="display:flex;justify-content:space-between;">
                    <strong>${autor ? Common.escapeHtml(autor.ime + " " + autor.prezime) : "Korisnik"}</strong>
                    <span style="color:var(--sl-muted);font-size:.85rem;">${Common.formatDate(r.datumKreiranja)}</span>
                </div>
                ${Common.starsHtml(r.ocjena, 1)}
                <p style="margin-top:.4rem;">${Common.escapeHtml(r.komentar || "")}</p>
                <div style="display:flex;gap:.5rem;margin-top:.4rem;">
                    ${Auth.isStaff() ? `<button class="btn btn-danger btn-sm" data-del-review="${r.id}">Obriši</button>` : ""}
                    ${jeClan && r.korisnikId !== user.id ? `<button class="btn btn-secondary btn-sm" data-report-review="${r.id}">Prijavi</button>` : ""}
                </div>
            </div>`;
        }).join("");
        el.querySelectorAll("[data-del-review]").forEach((btn) => btn.addEventListener("click", () => {
            if (!confirm("Obrisati ovu recenziju?")) return;
            const rId = Number(btn.getAttribute("data-del-review"));
            const review = DB.find("recenzije", rId);
            DB.remove("recenzije", rId);
            const author = DB.find("korisnici", review.korisnikId);
            if (author) {
                const strikes = (author.brojUklonjenihSadrzaja || 0) + 1;
                const patch = { brojUklonjenihSadrzaja: strikes };
                if (strikes >= 3) patch.datumZabraneDo = new Date(Date.now() + 7 * 86400000).toISOString().slice(0, 10);
                DB.update("korisnici", author.id, patch);
            }
            renderReviews();
            renderHeader();
        }));
        el.querySelectorAll("[data-report-review]").forEach((btn) => btn.addEventListener("click", () => {
            const rId = Number(btn.getAttribute("data-report-review"));
            if (DB.query("recenzijaPrijave", (p) => p.recenzijaId === rId && p.prijavioKorisnikId === user.id).length) {
                alert("Već ste prijavili ovu recenziju.");
                return;
            }
            const razlog = prompt("Razlog prijave:");
            if (razlog === null) return;
            DB.insert("recenzijaPrijave", { recenzijaId: rId, prijavioKorisnikId: user.id, razlog, datumKreiranja: new Date().toISOString().slice(0, 10), status: "otvorena", razrijesioKorisnikId: null, datumRazrjesenja: null });
            showToast("Recenzija je prijavljena.");
        }));
    }

    Common.Flash.renderInto(document.getElementById("alert-container"));
    renderHeader();
    renderPrimjerci();
    renderReviews();
})();
