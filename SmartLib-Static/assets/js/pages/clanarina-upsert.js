(function () {
    "use strict";
    if (!Auth.guard(["Bibliotekar", "Administrator"])) return;

    const korisnikId = Number(Common.qs("korisnikId"));
    const korisnik = DB.find("korisnici", korisnikId);
    if (!korisnik) { document.querySelector("main").innerHTML = "<p>Korisnik nije pronađen.</p>"; return; }

    document.getElementById("back-link").setAttribute("href", "../korisnik/profil.html?id=" + korisnikId);
    document.getElementById("page-heading").textContent = "Uredi članarinu — " + korisnik.ime + " " + korisnik.prezime;

    const existing = DB.query("clanarine", (c) => c.korisnikId === korisnikId).sort((a, b) => b.datumIsteka.localeCompare(a.datumIsteka))[0];
    const today = new Date().toISOString().slice(0, 10);
    const nextYear = new Date(); nextYear.setFullYear(nextYear.getFullYear() + 1);

    document.getElementById("datum-pocetka").value = existing ? existing.datumPocetka : today;
    document.getElementById("datum-isteka").value = existing ? existing.datumIsteka : nextYear.toISOString().slice(0, 10);

    document.getElementById("upsert-form").addEventListener("submit", (e) => {
        e.preventDefault();
        const patch = {
            korisnikId,
            datumPocetka: document.getElementById("datum-pocetka").value,
            datumIsteka: document.getElementById("datum-isteka").value,
        };
        if (existing) DB.update("clanarine", existing.id, patch);
        else DB.insert("clanarine", patch);

        if (korisnik.status === "deaktiviran") DB.update("korisnici", korisnikId, { status: "aktivan" });

        Common.Flash.set("success", "Članarina je sačuvana.");
        window.location.href = "../korisnik/profil.html?id=" + korisnikId;
    });
})();
