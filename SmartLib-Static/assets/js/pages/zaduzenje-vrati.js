(function () {
    "use strict";
    if (!Auth.guard(["Bibliotekar", "Administrator"])) return;

    const id = Number(Common.qs("id"));
    const zaduzenje = DB.find("zaduzenja", id);
    if (!zaduzenje) { document.querySelector("main").innerHTML = "<p>Zaduženje nije pronađeno.</p>"; return; }

    const korisnik = DB.find("korisnici", zaduzenje.korisnikId);
    const knjiga = DB.find("knjige", zaduzenje.knjigaId);
    const primjerak = DB.find("primjerci", zaduzenje.primjerakId);

    document.getElementById("details-grid").innerHTML = `
        <div><div class="label">Član</div><div class="value">${korisnik ? Common.escapeHtml(korisnik.ime + " " + korisnik.prezime) : "—"}</div></div>
        <div><div class="label">Knjiga</div><div class="value">${knjiga ? Common.escapeHtml(knjiga.naslov) : "—"}</div></div>
        <div><div class="label">Inv. broj</div><div class="value">${primjerak ? Common.escapeHtml(primjerak.inventarniBroj) : "—"}</div></div>
        <div><div class="label">Datum zaduživanja</div><div class="value">${Common.formatDate(zaduzenje.datumZaduzivanja)}</div></div>
        <div><div class="label">Rok vraćanja</div><div class="value">${Common.formatDate(zaduzenje.datumPlaniranogVracanja)}</div></div>
    `;

    document.getElementById("confirm-form").addEventListener("submit", (e) => {
        e.preventDefault();
        const today = new Date().toISOString().slice(0, 10);
        DB.update("zaduzenja", id, { status: "zatvoreno", datumStvarnogVracanja: today });
        DB.update("primjerci", zaduzenje.primjerakId, { status: "dostupan" });

        // Auto-fulfill the oldest active reservation for this book, if any.
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
