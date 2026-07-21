(function () {
    "use strict";
    if (!Auth.guard()) return;

    const user = Auth.currentUser();
    const jeClan = user.uloga === "Član";
    const isStaff = Auth.isStaff();

    function enrichBook(k) {
        const primjerci = DB.query("primjerci", (p) => p.knjigaId === k.id);
        const recenzije = DB.query("recenzije", (r) => r.knjigaId === k.id);
        const kategorija = DB.find("kategorije", k.kategorijaId);
        const brojDostupnih = primjerci.filter((p) => p.status === "dostupan").length;
        const prosjecnaOcjena = recenzije.length ? recenzije.reduce((s, r) => s + r.ocjena, 0) / recenzije.length : 0;
        let procitana = false;
        if (jeClan) {
            procitana = DB.query("zaduzenja", (z) => z.korisnikId === user.id && z.knjigaId === k.id && z.status === "zatvoreno").length > 0;
        }
        return Object.assign({}, k, {
            kategorijaNaziv: kategorija ? kategorija.naziv : null,
            brojPrimjeraka: primjerci.length,
            brojDostupnih,
            prosjecnaOcjena,
            brojRecenzija: recenzije.length,
            procitana,
        });
    }

    function allBooks() { return DB.getAll("knjige").map(enrichBook); }

    function populateFilterOptions() {
        const books = allBooks();
        const kategorije = DB.getAll("kategorije").sort((a, b) => a.naziv.localeCompare(b.naziv));
        const katSelect = document.getElementById("f-kategorija");
        kategorije.forEach((k) => katSelect.insertAdjacentHTML("beforeend", `<option value="${k.id}">${Common.escapeHtml(k.naziv)}</option>`));

        const izdavaci = [...new Set(books.map((b) => b.izdavac).filter(Boolean))].sort();
        const izdSelect = document.getElementById("f-izdavac");
        izdavaci.forEach((i) => izdSelect.insertAdjacentHTML("beforeend", `<option value="${Common.escapeHtml(i)}">${Common.escapeHtml(i)}</option>`));

        const godine = [...new Set(books.map((b) => b.godinaIzdanja).filter(Boolean))].sort((a, b) => b - a);
        const godSelect = document.getElementById("f-godina");
        godine.forEach((g) => godSelect.insertAdjacentHTML("beforeend", `<option value="${g}">${g}</option>`));
    }

    function currentFilters() {
        return {
            naslov: Common.qs("naslov", ""),
            autor: Common.qs("autor", ""),
            kategorijaId: Common.qs("kategorijaId", ""),
            izdavac: Common.qs("izdavac", ""),
            godinaIzdanja: Common.qs("godinaIzdanja", ""),
            samoNeprocitane: Common.qs("samoNeprocitane", "") === "true",
            page: Number(Common.qs("page", "1")),
            pageSize: Number(Common.qs("pageSize", "8")),
        };
    }

    function applyFiltersToForm(f) {
        document.getElementById("f-naslov").value = f.naslov;
        document.getElementById("f-autor").value = f.autor;
        document.getElementById("f-kategorija").value = f.kategorijaId;
        document.getElementById("f-izdavac").value = f.izdavac;
        document.getElementById("f-godina").value = f.godinaIzdanja;
        document.getElementById("f-neprocitane").checked = f.samoNeprocitane;
        document.getElementById("pageSizeSelect").value = String(f.pageSize);
        if (f.kategorijaId || f.izdavac || f.godinaIzdanja) {
            document.getElementById("advanced-filters-panel").classList.add("filters-open");
        }
    }

    function filteredBooks(f) {
        return allBooks().filter((b) => {
            if (f.naslov && !b.naslov.toLowerCase().includes(f.naslov.toLowerCase())) return false;
            if (f.autor && !b.autor.toLowerCase().includes(f.autor.toLowerCase())) return false;
            if (f.kategorijaId && String(b.kategorijaId) !== String(f.kategorijaId)) return false;
            if (f.izdavac && b.izdavac !== f.izdavac) return false;
            if (f.godinaIzdanja && String(b.godinaIzdanja) !== String(f.godinaIzdanja)) return false;
            if (f.samoNeprocitane && jeClan && b.procitana) return false;
            return true;
        });
    }

    function buildUrl(overrides) {
        const f = Object.assign(currentFilters(), overrides);
        const params = new URLSearchParams();
        if (f.naslov) params.set("naslov", f.naslov);
        if (f.autor) params.set("autor", f.autor);
        if (f.kategorijaId) params.set("kategorijaId", f.kategorijaId);
        if (f.izdavac) params.set("izdavac", f.izdavac);
        if (f.godinaIzdanja) params.set("godinaIzdanja", f.godinaIzdanja);
        if (f.samoNeprocitane) params.set("samoNeprocitane", "true");
        params.set("page", overrides.page || f.page || 1);
        params.set("pageSize", f.pageSize);
        return "index.html?" + params.toString();
    }

    function renderActiveFilters(f) {
        const bar = document.getElementById("active-filters-bar");
        const chips = [];
        if (f.naslov) chips.push(["Naslov: " + f.naslov, { naslov: "" }]);
        if (f.autor) chips.push(["Autor: " + f.autor, { autor: "" }]);
        if (f.kategorijaId) {
            const kat = DB.find("kategorije", Number(f.kategorijaId));
            chips.push(["Kategorija: " + (kat ? kat.naziv : "—"), { kategorijaId: "" }]);
        }
        if (f.izdavac) chips.push(["Izdavač: " + f.izdavac, { izdavac: "" }]);
        if (f.godinaIzdanja) chips.push(["Godina: " + f.godinaIzdanja, { godinaIzdanja: "" }]);
        if (f.samoNeprocitane) chips.push(["Samo nepročitane", { samoNeprocitane: "" }]);

        if (!chips.length) { bar.style.display = "none"; return; }
        bar.style.display = "flex";
        bar.innerHTML = '<span>Aktivni filteri:</span>' + chips.map(([label, override]) =>
            `<span class="filter-chip">${Common.escapeHtml(label)} <a href="${buildUrl(Object.assign({ page: 1 }, override))}" class="chip-remove">×</a></span>`
        ).join("");
    }

    function renderList(books) {
        const el = document.getElementById("katalog-list-layout");
        el.innerHTML = books.map((k) => `
            <a class="lb-list-item" href="details.html?id=${k.id}">
                <div class="lb-cover"><img src="${Common.bookCoverUrl(k.naslov)}" alt="${Common.escapeHtml(k.naslov)}" /></div>
                <div class="lb-body">
                    <h3 class="lb-title">${Common.escapeHtml(k.naslov)}</h3>
                    <span class="lb-author">${Common.escapeHtml(k.autor)}${k.godinaIzdanja ? ", " + k.godinaIzdanja : ""}</span>
                    <div class="lb-rating-row">
                        ${Common.starsHtml(k.prosjecnaOcjena, k.brojRecenzija)}
                        ${k.brojRecenzija > 0 ? `<span>${k.prosjecnaOcjena.toFixed(1)}</span> <span style="color:var(--sl-muted);">(${k.brojRecenzija})</span>` : `<span style="color:var(--sl-muted);">Nema recenzija</span>`}
                    </div>
                    ${k.opis ? `<p class="lb-description">${Common.escapeHtml(k.opis.length > 180 ? k.opis.slice(0, 180) + "…" : k.opis)}</p>` : ""}
                    <div class="lb-body-bottom">
                        <div class="lb-tags">
                            ${k.kategorijaNaziv ? `<span class="lb-tag">${Common.escapeHtml(k.kategorijaNaziv)}</span>` : ""}
                            ${jeClan && k.procitana ? `<span class="lb-tag" style="background:rgba(16,185,129,.12);color:#10b981;">Pročitano</span>` : ""}
                            <span class="lb-availability ${k.brojDostupnih > 0 ? "available" : "unavailable"}">${k.brojDostupnih > 0 ? "✓ Dostupno (" + k.brojDostupnih + ")" : "✗ Zauzeto"}</span>
                        </div>
                    </div>
                </div>
            </a>
        `).join("");
    }

    function renderGrid(books) {
        const el = document.getElementById("katalog-grid-layout");
        el.innerHTML = '<div class="katalog-picks">Naše preporuke</div>' + books.map((k) => `
            <a class="lb-grid-card" href="details.html?id=${k.id}">
                <div class="lb-grid-poster">
                    <img src="${Common.bookCoverUrl(k.naslov)}" alt="${Common.escapeHtml(k.naslov)}" />
                    <span class="lb-grid-avail ${k.brojDostupnih > 0 ? "available" : "unavailable"}">${k.brojDostupnih > 0 ? "Dostupno" : "Zauzeto"}</span>
                </div>
                <div class="lb-grid-info">
                    <span class="lb-grid-title">${Common.escapeHtml(k.naslov)}</span>
                    <span class="lb-grid-author">${Common.escapeHtml(k.autor)}</span>
                </div>
            </a>
        `).join("");
    }

    function render() {
        const f = currentFilters();
        applyFiltersToForm(f);
        renderActiveFilters(f);

        const filtered = filteredBooks(f);
        document.getElementById("katalog-meta").textContent = (f.naslov || f.autor || f.kategorijaId || f.izdavac || f.godinaIzdanja || f.samoNeprocitane)
            ? `Pronađeno: ${filtered.length} ${filtered.length === 1 ? "knjiga" : "knjiga"}`
            : `Ukupno knjiga u katalogu: ${filtered.length}`;

        const emptyEl = document.getElementById("katalog-empty");
        const listEl = document.getElementById("katalog-list-layout");
        const gridEl = document.getElementById("katalog-grid-layout");
        const pagEl = document.getElementById("katalog-pagination");

        if (!filtered.length) {
            emptyEl.style.display = "block";
            emptyEl.innerHTML = "<p>📚 Nema rezultata za odabrane filtere.</p>";
            listEl.innerHTML = ""; gridEl.innerHTML = ""; pagEl.innerHTML = "";
            return;
        }
        emptyEl.style.display = "none";

        const { items, page, totalPages } = DB.paginate(filtered, f.page, f.pageSize);
        renderList(items);
        renderGrid(items);
        pagEl.innerHTML = Common.paginationHtml(page, totalPages, (p) => buildUrl({ page: p }));
    }

    document.getElementById("filter-form").addEventListener("submit", (e) => {
        e.preventDefault();
        window.location.href = buildUrl({
            naslov: document.getElementById("f-naslov").value.trim(),
            autor: document.getElementById("f-autor").value.trim(),
            kategorijaId: document.getElementById("f-kategorija").value,
            izdavac: document.getElementById("f-izdavac").value,
            godinaIzdanja: document.getElementById("f-godina").value,
            samoNeprocitane: document.getElementById("f-neprocitane").checked,
            page: 1,
        });
    });
    document.getElementById("reset-filters-btn").addEventListener("click", () => { window.location.href = "index.html"; });
    document.getElementById("btn-advanced-toggle").addEventListener("click", () => {
        document.getElementById("advanced-filters-panel").classList.toggle("filters-open");
    });
    document.getElementById("pageSizeSelect").addEventListener("change", (e) => {
        window.location.href = buildUrl({ pageSize: Number(e.target.value), page: 1 });
    });

    const btnList = document.getElementById("btn-list-view");
    const btnGrid = document.getElementById("btn-grid-view");
    const listLayout = document.getElementById("katalog-list-layout");
    const gridLayout = document.getElementById("katalog-grid-layout");
    function showList() { btnList.classList.add("active"); btnGrid.classList.remove("active"); listLayout.classList.remove("hidden-layout"); gridLayout.classList.add("hidden-layout"); localStorage.setItem("katalogView", "list"); }
    function showGrid() { btnGrid.classList.add("active"); btnList.classList.remove("active"); gridLayout.classList.remove("hidden-layout"); listLayout.classList.add("hidden-layout"); localStorage.setItem("katalogView", "grid"); }
    btnList.addEventListener("click", showList);
    btnGrid.addEventListener("click", showGrid);

    Common.Flash.renderInto(document.getElementById("alert-container"));
    populateFilterOptions();
    render();
    if (localStorage.getItem("katalogView") === "grid") showGrid();
})();
