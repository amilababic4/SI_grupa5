(function () {
    "use strict";
    if (!Auth.guard(["Bibliotekar", "Administrator"])) return;

    const id = Number(Common.qs("id"));
    const zaduzenje = DB.find("zaduzenja", id);
    if (!zaduzenje) { document.querySelector("main").innerHTML = "<p>Zaduženje nije pronađeno.</p>"; return; }

    const korisnik = DB.find("korisnici", zaduzenje.korisnikId);
    const knjiga = DB.find("knjige", zaduzenje.knjigaId);
    const primjerak = DB.find("primjerci", zaduzenje.primjerakId);

    document.getElementById("back-link").setAttribute("href", "details.html?id=" + id);
    document.getElementById("cancel-link").setAttribute("href", "details.html?id=" + id);

    document.getElementById("v-knjiga").querySelector("strong").textContent = knjiga ? knjiga.naslov : "—";
    document.getElementById("v-invbroj").textContent = primjerak ? primjerak.inventarniBroj : "—";
    document.getElementById("v-clan").textContent = korisnik ? korisnik.ime + " " + korisnik.prezime : "—";
    document.getElementById("v-rok").textContent = Common.formatDate(zaduzenje.datumPlaniranogVracanja);

    document.getElementById("confirm-form").addEventListener("submit", (e) => {
        e.preventDefault();
        const today = new Date().toISOString().slice(0, 10);
        DB.update("zaduzenja", id, { status: "zatvoreno", datumStvarnogVracanja: today });
        DB.update("primjerci", zaduzenje.primjerakId, { status: "dostupan" });

        if (knjiga) {
            const rez = DB.query("rezervacije", (r) => r.knjigaId === knjiga.id && r.status === "aktivna")
                .sort((a, b) => a.datumRezervacije.localeCompare(b.datumRezervacije))[0];
            if (rez) {
                DB.update("rezervacije", rez.id, { status: "realizovana" });
                DB.insert("notifikacije", {
                    korisnikId: rez.korisnikId,
                    naslov: "Knjiga koju ste rezervisali je dostupna",
                    poruka: `'${knjiga.naslov}' je sada dostupna za preuzimanje.`,
                    tip: "Rezervacija",
                    linkUrl: "../rezervacija/moje.html",
                    procitano: false,
                    datumKreiranja: today,
                });
            }
        }

        Common.Flash.set("success", "Vraćanje je evidentirano.");
        window.location.href = "index.html";
    });
})();
