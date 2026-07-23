(function () {
    "use strict";
    if (!Auth.guard(["Bibliotekar", "Administrator"])) return;

    const today = new Date().toISOString().slice(0, 10);
    document.getElementById("datum-vracanja").setAttribute("min", today);

    const clanData = DB.query("korisnici", (u) => u.uloga === "Član" && u.status === "aktivan")
        .sort((a, b) => a.prezime.localeCompare(b.prezime))
        .map((u) => ({ id: u.id, text: u.ime + " " + u.prezime + " (" + u.email + ")" }));

    const knjigaData = DB.getAll("knjige")
        .sort((a, b) => a.naslov.localeCompare(b.naslov))
        .map((k) => ({ id: k.id, text: k.naslov + " — " + k.autor }));

    const primjerakData = DB.query("primjerci", (p) => p.status === "dostupan")
        .map((p) => ({ id: p.id, knjigaId: p.knjigaId, text: p.inventarniBroj }));

    function makeCombo(rootEl, data, hiddenEl, onChange) {
        const input = rootEl.querySelector(".zad-combo-input");
        const list = rootEl.querySelector(".zad-combo-list");
        let selId = String(hiddenEl.value || "0");
        let selLabel = "";

        function labelById(id) {
            for (let i = 0; i < data.length; i++) {
                if (String(data[i].id) === String(id)) return data[i].text;
            }
            return "";
        }

        if (selId !== "0") {
            selLabel = labelById(selId);
            input.value = selLabel;
        }

        function renderList(query) {
            const q = query ? query.trim().toLowerCase() : "";
            const items = q ? data.filter((d) => d.text.toLowerCase().indexOf(q) !== -1) : data;

            list.innerHTML = "";

            if (!items.length) {
                const empty = document.createElement("li");
                empty.className = "zad-combo-empty";
                empty.textContent = q ? "Nema rezultata." : "Nema dostupnih stavki.";
                list.appendChild(empty);
                return;
            }

            items.forEach((item) => {
                const li = document.createElement("li");
                li.className = "zad-combo-item";
                li.setAttribute("role", "option");
                li.setAttribute("data-id", String(item.id));
                li.textContent = item.text;
                if (String(item.id) === selId) li.classList.add("is-selected");

                li.addEventListener("mousedown", (e) => {
                    e.preventDefault();
                    pick(String(item.id), item.text);
                });
                list.appendChild(li);
            });
        }

        function openList() {
            rootEl.classList.add("is-open");
            list.removeAttribute("hidden");
            const q = (selId !== "0" && input.value === selLabel) ? "" : input.value;
            renderList(q);
            const sel = list.querySelector(".is-selected");
            if (sel) sel.scrollIntoView({ block: "nearest" });
        }

        function closeList() {
            rootEl.classList.remove("is-open");
            list.setAttribute("hidden", "");
            input.value = selId !== "0" ? selLabel : "";
        }

        function pick(id, text) {
            selId = id;
            selLabel = text;
            hiddenEl.value = id;
            closeList();
            if (onChange) onChange(id);
        }

        input.addEventListener("focus", openList);

        input.addEventListener("input", function () {
            selId = "0";
            selLabel = "";
            hiddenEl.value = "0";
            rootEl.classList.add("is-open");
            list.removeAttribute("hidden");
            renderList(this.value);
        });

        input.addEventListener("blur", () => { setTimeout(closeList, 160); });

        input.addEventListener("keydown", (e) => {
            const items = Array.from(list.querySelectorAll(".zad-combo-item"));
            const idx = items.findIndex((li) => li.classList.contains("is-active"));

            if (e.key === "ArrowDown") {
                e.preventDefault();
                if (!rootEl.classList.contains("is-open")) openList();
                const next = idx < items.length - 1 ? idx + 1 : 0;
                items.forEach((li) => li.classList.remove("is-active"));
                if (items[next]) { items[next].classList.add("is-active"); items[next].scrollIntoView({ block: "nearest" }); }
            } else if (e.key === "ArrowUp") {
                e.preventDefault();
                const prev = idx > 0 ? idx - 1 : items.length - 1;
                items.forEach((li) => li.classList.remove("is-active"));
                if (items[prev]) { items[prev].classList.add("is-active"); items[prev].scrollIntoView({ block: "nearest" }); }
            } else if (e.key === "Enter") {
                e.preventDefault();
                const active = list.querySelector(".is-active");
                if (active) active.dispatchEvent(new MouseEvent("mousedown", { bubbles: true }));
            } else if (e.key === "Escape") {
                closeList();
            }
        });

        return {
            setData(newData) {
                data = newData;
                selId = "0";
                selLabel = "";
            },
        };
    }

    const hidKorisnik = document.getElementById("hid-korisnik");
    const hidKnjiga = document.getElementById("hid-knjiga");
    const hidPrimjerak = document.getElementById("hid-primjerak");
    hidKorisnik.value = 0;
    hidKnjiga.value = 0;
    hidPrimjerak.value = 0;

    const primjerakInputEl = document.getElementById("combo-primjerak-input");

    function primjerciFor(knjigaId) {
        return primjerakData.filter((p) => String(p.knjigaId) === String(knjigaId));
    }

    const comboP = makeCombo(document.getElementById("combo-primjerak"), primjerciFor(0), hidPrimjerak, null);

    function refreshPrimjerak(knjigaId) {
        const avail = primjerciFor(knjigaId);
        comboP.setData(avail);
        hidPrimjerak.value = "0";
        primjerakInputEl.value = "";
        primjerakInputEl.placeholder = String(knjigaId) === "0"
            ? "Prvo odaberite knjigu..."
            : avail.length ? "Odaberite primjerak..." : "Nema dostupnih primjeraka";
    }

    makeCombo(document.getElementById("combo-knjiga"), knjigaData, hidKnjiga, (id) => refreshPrimjerak(id));
    makeCombo(document.getElementById("combo-clan"), clanData, hidKorisnik, null);

    document.getElementById("zaduzi-form").addEventListener("submit", (e) => {
        e.preventDefault();
        const validationEl = document.getElementById("validation-summary");
        validationEl.style.display = "none";

        const korisnikId = Number(hidKorisnik.value);
        const knjigaId = Number(hidKnjiga.value);
        const primjerakId = Number(hidPrimjerak.value);

        if (!korisnikId || !knjigaId || !primjerakId) {
            validationEl.textContent = "Molimo odaberite člana, knjigu i primjerak sa liste.";
            validationEl.style.display = "";
            return;
        }

        const kasneca = DB.query("zaduzenja", (z) => z.korisnikId === korisnikId && z.status === "zakašnjelo").length > 0;
        if (kasneca) {
            validationEl.textContent = "Ovaj član ima kašnjenje u vraćanju i ne može zadužiti novu knjigu.";
            validationEl.style.display = "";
            return;
        }

        let datumPovratka = document.getElementById("datum-vracanja").value;
        if (!datumPovratka) {
            const d = new Date();
            d.setMonth(d.getMonth() + 2);
            datumPovratka = d.toISOString().slice(0, 10);
        }

        DB.insert("zaduzenja", {
            korisnikId, knjigaId, primjerakId,
            datumZaduzivanja: today,
            datumPlaniranogVracanja: datumPovratka,
            datumStvarnogVracanja: null,
            status: "aktivno",
        });
        DB.update("primjerci", primjerakId, { status: "zadužen" });

        const activeRez = DB.query("rezervacije", (r) => r.korisnikId === korisnikId && r.knjigaId === knjigaId && r.status === "aktivna")[0];
        if (activeRez) DB.update("rezervacije", activeRez.id, { status: "realizovana" });

        Common.Flash.set("success", "Knjiga je uspješno zadužena.");
        window.location.href = "index.html";
    });
})();
