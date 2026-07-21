(function () {
    "use strict";
    if (!Auth.guard(["Član"])) return;
    const user = Auth.currentUser();
    const knjigaId = Number(Common.qs("knjigaId"));
    const knjiga = DB.find("knjige", knjigaId);
    if (!knjiga) { document.querySelector("main").innerHTML = "<p>Knjiga nije pronađena.</p>"; return; }

    document.getElementById("page-title").textContent = "Ocijeni: " + knjiga.naslov;

    const hasBorrowed = DB.query("zaduzenja", (z) => z.korisnikId === user.id && z.knjigaId === knjigaId && z.status === "zatvoreno").length > 0;
    const existing = DB.query("recenzije", (r) => r.knjigaId === knjigaId && r.korisnikId === user.id)[0];
    const alertContainer = document.getElementById("alert-container");

    if (!hasBorrowed) {
        alertContainer.innerHTML = '<div class="alert alert-error">Recenziju možete ostaviti samo za knjige koje ste ranije zadužili.</div>';
        document.getElementById("review-form").style.display = "none";
        return;
    }
    if (existing) {
        alertContainer.innerHTML = '<div class="alert alert-success">Već ste ostavili recenziju za ovu knjigu.</div>';
        document.getElementById("review-form").style.display = "none";
        return;
    }

    document.getElementById("review-form").addEventListener("submit", (e) => {
        e.preventDefault();
        const ocjena = document.querySelector("input[name='ocjena']:checked");
        if (!ocjena) { alert("Odaberite ocjenu."); return; }
        const komentar = document.getElementById("komentar").value.trim();
        if (!komentar) {
            if (!confirm("Niste unijeli komentar. Nastaviti bez komentara?")) return;
        }
        DB.insert("recenzije", {
            knjigaId,
            korisnikId: user.id,
            ocjena: Number(ocjena.value),
            komentar,
            datumKreiranja: new Date().toISOString().slice(0, 10),
        });
        Common.Flash.set("success", "Recenzija je sačuvana.");
        window.location.href = "../knjiga/details.html?id=" + knjigaId;
    });
})();
