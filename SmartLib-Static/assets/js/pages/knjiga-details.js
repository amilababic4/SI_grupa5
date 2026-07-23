(function () {
    "use strict";
    if (!Auth.guard()) return;

    const user = Auth.currentUser();
    const jeClan = user.uloga === "Član";
    const jeOsoblje = Auth.isStaff();
    const bookId = Number(Common.qs("id"));
    const knjiga = DB.find("knjige", bookId);

    if (!knjiga) {
        document.querySelector("main").innerHTML = "<p>Knjiga nije pronađena.</p>";
        return;
    }

    Common.Flash.renderInto(document.getElementById("alert-container"));

    function primjerci() { return DB.query("primjerci", (p) => p.knjigaId === bookId); }
    function recenzije() { return DB.query("recenzije", (r) => r.knjigaId === bookId); }

    function renderHeader() {
        const kategorija = DB.find("kategorije", knjiga.kategorijaId);
        const primj = primjerci();
        const dostupno = primj.filter((p) => p.status === "dostupan").length;

        document.title = knjiga.naslov + " — SmartLib";
        document.getElementById("mainCoverImage").src = Common.bookCoverUrl(knjiga.naslov);
        document.getElementById("mainCoverImage").alt = knjiga.naslov;
        document.getElementById("book-naslov").textContent = knjiga.naslov;
        document.getElementById("book-autor").textContent = knjiga.autor;

        if (jeClan) {
            const procitana = DB.query("zaduzenja", (z) => z.korisnikId === user.id && z.knjigaId === bookId && z.status === "zatvoreno").length > 0;
            if (procitana) document.getElementById("read-badge").style.display = "";
        }

        document.getElementById("book-opis").textContent = knjiga.opis || "";
        document.getElementById("book-opis").style.display = knjiga.opis ? "" : "none";
        document.getElementById("book-isbn").textContent = knjiga.isbn;
        document.getElementById("book-kategorija").innerHTML = kategorija ? `<span class="katalog-badge">${Common.escapeHtml(kategorija.naziv)}</span>` : "—";
        document.getElementById("book-izdavac").textContent = knjiga.izdavac || "—";
        document.getElementById("book-godina").textContent = knjiga.godinaIzdanja || "—";
        document.getElementById("book-broj-primjeraka").textContent = primj.length;
        document.getElementById("book-dostupno").innerHTML = `<span class="${dostupno > 0 ? "status-dostupan" : "status-nedostupan"}">${dostupno}</span>`;

        document.getElementById("cover-wrapper").addEventListener("click", () => {
            document.getElementById("modalImage").src = Common.bookCoverUrl(knjiga.naslov);
            document.getElementById("imageModal").style.display = "flex";
        });

        document.getElementById("edit-link").setAttribute("href", "edit.html?id=" + bookId);
        document.getElementById("delete-form").addEventListener("submit", (e) => {
            e.preventDefault();
            if (!confirm("Jeste li sigurni da želite obrisati knjigu: " + knjiga.naslov + "?")) return;
            primjerci().forEach((p) => DB.remove("primjerci", p.id));
            recenzije().forEach((r) => DB.remove("recenzije", r.id));
            DB.remove("knjige", bookId);
            Common.Flash.set("success", "Knjiga je obrisana.");
            window.location.href = "index.html";
        });

        if (jeClan) setupMemberActions(dostupno);
    }

    function setupMemberActions(dostupno) {
        const slot = document.getElementById("reserve-slot");
        const activeRez = DB.query("rezervacije", (r) => r.korisnikId === user.id && r.knjigaId === bookId && r.status === "aktivna")[0];
        const kasneca = DB.query("zaduzenja", (z) => z.korisnikId === user.id && z.status === "zakašnjelo").length > 0;

        if (dostupno === 0) {
            if (activeRez) {
                slot.innerHTML = `<button class="btn btn-secondary" disabled title="Već imate aktivnu rezervaciju za ovu knjigu">Rezervisano</button>`;
            } else if (kasneca) {
                slot.innerHTML = `<button class="btn btn-secondary" disabled title="Imate zakasnijela zaduženja — kontaktirajte biblioteku">Rezervacija blokirana</button>
                    <p style="color:#ef4444;font-size:0.82rem;margin-top:0.4rem;">⚠️ Imate zakasnijela zaduženja. Rezervacija nije moguća.</p>`;
            } else {
                slot.innerHTML = `<button type="button" class="btn btn-primary" id="reserve-btn">Rezerviši</button>`;
                document.getElementById("reserve-btn").addEventListener("click", () => {
                    if (!confirm("Rezervisati knjigu: " + knjiga.naslov + "?")) return;
                    DB.insert("rezervacije", {
                        korisnikId: user.id, knjigaId: bookId,
                        datumRezervacije: new Date().toISOString().slice(0, 10),
                        datumIsteka: new Date(Date.now() + 7 * 86400000).toISOString().slice(0, 10),
                        status: "aktivna",
                    });
                    Common.Flash.set("success", "Knjiga je uspješno rezervisana.");
                    window.location.reload();
                });
            }
        }

        document.getElementById("open-collection-modal-btn").addEventListener("click", openCollectionModal);
    }

    function showToast(msg, isError) {
        const toast = document.getElementById("collection-toast");
        toast.textContent = msg;
        toast.classList.toggle("is-error", !!isError);
        toast.classList.add("show");
        setTimeout(() => toast.classList.remove("show"), 2800);
    }

    function openCollectionModal() {
        const kolekcije = DB.query("listaKolekcija", (l) => l.korisnikId === user.id);
        const list = document.getElementById("collection-modal-list");
        if (!kolekcije.length) {
            list.innerHTML = "<p class='empty-note'>Nemate kolekcija. Kreirajte novu ispod.</p>";
        } else {
            list.innerHTML = kolekcije.map((k) => {
                const already = DB.query("listaKolekcijaStavke", (s) => s.listaKolekcijaId === k.id && s.knjigaId === bookId).length > 0;
                const isWishlist = k.naziv === "Lista želja";
                return `<form class="sl-modal-collection-row ${isWishlist ? "wishlist-row" : ""}" data-kat-id="${k.id}">
                    <div class="sl-modal-collection-info">
                        <span class="sl-modal-collection-name">${Common.escapeHtml(k.naziv)}</span>
                        <span class="visibility-pill ${k.javna ? "is-public" : "is-private"}">${k.javna ? "JAVNA" : "PRIVATNA"}</span>
                        ${isWishlist ? `<span class="wishlist-chip">Podrazumijevano</span>` : ""}
                    </div>
                    <button type="submit" class="btn ${isWishlist ? "btn-primary" : "btn-secondary"} btn-sm" ${already ? "disabled" : ""}>${already ? "Dodano" : (isWishlist ? "Dodaj u listu želja" : "Dodaj")}</button>
                </form>`;
            }).join("");
            list.querySelectorAll("form[data-kat-id]").forEach((form) => {
                form.addEventListener("submit", (e) => {
                    e.preventDefault();
                    const listaId = Number(form.getAttribute("data-kat-id"));
                    const already = DB.query("listaKolekcijaStavke", (s) => s.listaKolekcijaId === listaId && s.knjigaId === bookId).length > 0;
                    const kol = DB.find("listaKolekcija", listaId);
                    const listName = kol ? kol.naziv : "kolekciju";
                    if (already) { showToast(`Knjiga je već u kolekciji "${listName}".`, true); return; }
                    const redoslijed = DB.query("listaKolekcijaStavke", (s) => s.listaKolekcijaId === listaId).length + 1;
                    DB.insert("listaKolekcijaStavke", { listaKolekcijaId: listaId, knjigaId: bookId, redoslijed, datumDodavanja: new Date().toISOString().slice(0, 10) });
                    showToast(`Dodano u kolekciju "${listName}".`);
                    openCollectionModal();
                });
            });
        }
        document.getElementById("collectionModal").style.display = "flex";
    }
    document.getElementById("close-collection-modal").addEventListener("click", () => { document.getElementById("collectionModal").style.display = "none"; });
    document.getElementById("collectionModal").addEventListener("click", (e) => { if (e.target.id === "collectionModal") e.target.style.display = "none"; });

    document.getElementById("open-create-collection-btn").addEventListener("click", () => {
        document.getElementById("collectionModal").style.display = "none";
        document.getElementById("collectionCreateModal").style.display = "flex";
    });
    document.getElementById("close-create-collection-btn").addEventListener("click", () => { document.getElementById("collectionCreateModal").style.display = "none"; });
    document.getElementById("collectionCreateModal").addEventListener("click", (e) => { if (e.target.id === "collectionCreateModal") e.target.style.display = "none"; });
    document.getElementById("create-collection-form").addEventListener("submit", (e) => {
        e.preventDefault();
        const naziv = document.getElementById("NewNaziv").value.trim();
        const kol = DB.insert("listaKolekcija", {
            korisnikId: user.id, naziv, opis: document.getElementById("NewOpis").value.trim(),
            javna: document.getElementById("NewJavna").checked,
            datumKreiranja: new Date().toISOString().slice(0, 10), datumAzuriranja: new Date().toISOString().slice(0, 10), isWishlist: false,
        });
        DB.insert("listaKolekcijaStavke", { listaKolekcijaId: kol.id, knjigaId: bookId, redoslijed: 1, datumDodavanja: new Date().toISOString().slice(0, 10) });
        document.getElementById("collectionCreateModal").style.display = "none";
        showToast(`Dodano u kolekciju "${naziv}".`);
    });

    document.getElementById("close-image-modal").addEventListener("click", () => { document.getElementById("imageModal").style.display = "none"; });
    document.getElementById("imageModal").addEventListener("click", (e) => { if (e.target.id === "imageModal") e.target.style.display = "none"; });
    document.addEventListener("keydown", (e) => {
        if (e.key === "Escape") {
            document.getElementById("imageModal").style.display = "none";
            document.getElementById("collectionModal").style.display = "none";
            document.getElementById("collectionCreateModal").style.display = "none";
        }
    });

    function renderPrimjerci() {
        const primj = primjerci();
        document.getElementById("primjerci-count").textContent = primj.length;
        if (!primj.length) {
            document.getElementById("primjerci-empty").style.display = "block";
            document.getElementById("primjerci-table-wrap").style.display = "none";
            document.getElementById("primjerci-legenda").style.display = "none";
            return;
        }
        document.getElementById("primjerci-empty").style.display = "none";
        document.getElementById("primjerci-table-wrap").style.display = "";
        document.getElementById("primjerci-legenda").style.display = "flex";

        document.getElementById("primjerci-tbody").innerHTML = primj.map((p) => {
            const jeDeaktiviran = p.status === "deaktiviran";
            const clickable = jeOsoblje && !jeDeaktiviran;
            const rowCls = jeDeaktiviran ? "row-deaktiviran" : (clickable ? "members-row-link" : "");
            const invCls = jeDeaktiviran ? "inv-broj inv-broj--deaktiviran" : "inv-broj members-name";
            const akcije = jeOsoblje
                ? (jeDeaktiviran ? `<td data-label="Akcije" class="katalog-actions"><span class="deaktiviran-label">Deaktiviran</span></td>`
                    : `<td data-label="Akcije" class="katalog-actions" data-stop="1"><button class="btn btn-danger btn-sm" data-deact="${p.id}">Deaktiviraj</button></td>`)
                : "";
            return `<tr class="${rowCls}" ${clickable ? `data-goto="${p.id}"` : ""}>
                <td data-label="Inventarni broj"><span class="${invCls}">${Common.escapeHtml(p.inventarniBroj)}</span></td>
                <td data-label="Status">${Common.statusBadgeHtml(p.status)}</td>
                <td data-label="Datum nabave">${Common.formatDate(p.datumNabave)}</td>
                ${akcije}
            </tr>`;
        }).join("");

        document.getElementById("primjerci-tbody").querySelectorAll("[data-goto]").forEach((row) => {
            row.addEventListener("click", () => { window.location.href = "../zaduzenje/zaduzenja-primjerka.html?id=" + row.getAttribute("data-goto"); });
        });
        document.getElementById("primjerci-tbody").querySelectorAll("[data-stop]").forEach((td) => td.addEventListener("click", (e) => e.stopPropagation()));
        document.getElementById("primjerci-tbody").querySelectorAll("[data-deact]").forEach((btn) => {
            btn.addEventListener("click", () => {
                const id = Number(btn.getAttribute("data-deact"));
                const p = DB.find("primjerci", id);
                if (p.status === "zadužen") { alert("Ne možete deaktivirati primjerak koji je trenutno zadužen."); return; }
                if (!confirm("Deaktivirati primjerak " + p.inventarniBroj + "?")) return;
                DB.update("primjerci", id, { status: "deaktiviran" });
                renderPrimjerci();
                renderHeader();
            });
        });
    }

    document.getElementById("btn-dodaj-primjerak").addEventListener("click", () => document.getElementById("forma-dodaj-primjerak").classList.toggle("hidden"));
    document.getElementById("cancel-add-primjerak").addEventListener("click", () => document.getElementById("forma-dodaj-primjerak").classList.add("hidden"));
    document.getElementById("add-primjerak-form").addEventListener("submit", (e) => {
        e.preventDefault();
        const n = Number(document.getElementById("brojNovih").value) || 1;
        const existing = primjerci();
        let seq = existing.length + 1;
        for (let i = 0; i < n; i++) {
            DB.insert("primjerci", { knjigaId: bookId, inventarniBroj: `INV-${bookId}-${seq}`, status: "dostupan", datumNabave: new Date().toISOString().slice(0, 10) });
            seq++;
        }
        document.getElementById("forma-dodaj-primjerak").classList.add("hidden");
        renderPrimjerci();
        renderHeader();
    });

    function renderReviews() {
        const rec = recenzije();
        const prosjek = rec.length ? rec.reduce((s, r) => s + r.ocjena, 0) / rec.length : 0;
        const brojZaduzenja = DB.query("zaduzenja", (z) => z.knjigaId === bookId).length;
        document.getElementById("reviews-meta").innerHTML = `
            ${rec.length ? `<div class="reviews-rating">${prosjek.toFixed(1)} ⭐ <span>(${rec.length} recenzija)</span></div>` : ""}
            <span class="reviews-popularity">Popularnost: ${brojZaduzenja}</span>`;
        document.getElementById("add-review-link").setAttribute("href", "../recenzija/dodaj.html?knjigaId=" + bookId);

        const el = document.getElementById("reviews-list");
        if (!rec.length) { el.innerHTML = "<div style='text-align:center;color:var(--text-muted);padding:2rem 0;'><p>Ova knjiga još nema recenzija.</p></div>"; return; }

        el.innerHTML = `<div style="display:flex;flex-direction:column;gap:1.5rem;">` + rec.slice().reverse().map((r) => {
            const autor = DB.find("korisnici", r.korisnikId);
            const reported = DB.query("recenzijaPrijave", (p) => p.recenzijaId === r.id && p.prijavioKorisnikId === user.id).length > 0;
            return `<div style="background: var(--bg-alt); padding: 1.25rem; border-radius: 8px; border: 1px solid var(--border-color);">
                <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 0.75rem;">
                    <div>
                        <strong style="color: var(--text-main);">${autor ? Common.escapeHtml(autor.ime + " " + autor.prezime) : ""}</strong>
                        <div style="color: var(--text-muted); font-size: 0.85rem;">${Common.formatDate(r.datumKreiranja)}</div>
                    </div>
                    <div style="display: flex; align-items: center; gap: 0.75rem;">
                        <div style="color: #fbbf24; font-weight: bold;">${r.ocjena} / 5 ⭐</div>
                        ${jeOsoblje ? `<button class="btn btn-danger btn-sm" data-del-review="${r.id}" title="Ukloni neadekvatnu recenziju">🗑 Ukloni</button>` : ""}
                        ${!jeOsoblje ? (reported ? `<span class="status-blizi" title="Recenzija je već prijavljena">Prijavljeno</span>` : `<button class="btn btn-outline btn-sm" data-report-review="${r.id}" title="Prijavi recenziju">🚩 Prijavi</button>`) : ""}
                    </div>
                </div>
                ${r.komentar ? `<p style="margin: 0; color: var(--text-secondary); line-height: 1.5;">${Common.escapeHtml(r.komentar)}</p>` : ""}
            </div>`;
        }).join("") + `</div>`;

        el.querySelectorAll("[data-del-review]").forEach((btn) => btn.addEventListener("click", () => {
            if (!confirm("Ukloniti ovu recenziju?")) return;
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
        }));
        el.querySelectorAll("[data-report-review]").forEach((btn) => btn.addEventListener("click", () => {
            if (!confirm("Prijaviti ovu recenziju kao neadekvatnu?")) return;
            const rId = Number(btn.getAttribute("data-report-review"));
            DB.insert("recenzijaPrijave", { recenzijaId: rId, prijavioKorisnikId: user.id, razlog: "Neadekvatan sadržaj", datumKreiranja: new Date().toISOString().slice(0, 10), status: "otvorena", razrijesioKorisnikId: null, datumRazrjesenja: null });
            renderReviews();
        }));
    }

    renderHeader();
    renderPrimjerci();
    renderReviews();
})();
