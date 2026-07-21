(function () {
    "use strict";
    if (!Auth.guard(["Bibliotekar", "Administrator"])) return;

    const selClan = document.getElementById("sel-clan");
    const selKnjiga = document.getElementById("sel-knjiga");
    const selPrimjerak = document.getElementById("sel-primjerak");
    const alertContainer = document.getElementById("alert-container");

    const defaultDate = new Date();
    defaultDate.setMonth(defaultDate.getMonth() + 2);
    document.getElementById("datum-vracanja").value = defaultDate.toISOString().slice(0, 10);

    DB.query("korisnici", (u) => u.uloga === "Član" && u.status === "aktivan").sort((a, b) => a.prezime.localeCompare(b.prezime)).forEach((u) => {
        selClan.insertAdjacentHTML("beforeend", `<option value="${u.id}">${Common.escapeHtml(u.ime + " " + u.prezime)} (${Common.escapeHtml(u.email)})</option>`);
    });

    selClan.addEventListener("change", () => {
        selKnjiga.innerHTML = '<option value="">— Odaberite knjigu —</option>';
        selPrimjerak.innerHTML = '<option value="">— Prvo odaberite knjigu —</option>';
        selPrimjerak.disabled = true;
        if (!selClan.value) { selKnjiga.disabled = true; return; }
        DB.getAll("knjige").filter((k) => DB.query("primjerci", (p) => p.knjigaId === k.id && p.status === "dostupan").length > 0)
            .sort((a, b) => a.naslov.localeCompare(b.naslov))
            .forEach((k) => selKnjiga.insertAdjacentHTML("beforeend", `<option value="${k.id}">${Common.escapeHtml(k.naslov)} — ${Common.escapeHtml(k.autor)}</option>`));
        selKnjiga.disabled = false;
    });

    selKnjiga.addEventListener("change", () => {
        selPrimjerak.innerHTML = '<option value="">— Odaberite primjerak —</option>';
        if (!selKnjiga.value) { selPrimjerak.disabled = true; return; }
        DB.query("primjerci", (p) => p.knjigaId === Number(selKnjiga.value) && p.status === "dostupan")
            .forEach((p) => selPrimjerak.insertAdjacentHTML("beforeend", `<option value="${p.id}">${Common.escapeHtml(p.inventarniBroj)}</option>`));
        selPrimjerak.disabled = false;
    });

    document.getElementById("zaduzi-form").addEventListener("submit", (e) => {
        e.preventDefault();
        const korisnikId = Number(selClan.value);
        const knjigaId = Number(selKnjiga.value);
        const primjerakId = Number(selPrimjerak.value);

        const kasneca = DB.query("zaduzenja", (z) => z.korisnikId === korisnikId && z.status === "zakašnjelo").length > 0;
        if (kasneca) {
            alertContainer.innerHTML = '<div class="alert alert-error">Ovaj član ima kašnjenje u vraćanju i ne može zadužiti novu knjigu.</div>';
            return;
        }

        DB.insert("zaduzenja", {
            korisnikId, knjigaId, primjerakId,
            datumZaduzivanja: new Date().toISOString().slice(0, 10),
            datumPlaniranogVracanja: document.getElementById("datum-vracanja").value,
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
