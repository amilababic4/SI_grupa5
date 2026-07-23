(function () {
    "use strict";
    if (!Auth.guard(["Bibliotekar", "Administrator"])) return;
    Common.Flash.renderInto(document.getElementById("alert-container"));

    const id = Number(Common.qs("id"));
    const z = DB.find("zaduzenja", id);
    if (!z) { document.querySelector("main").innerHTML = "<p>Zaduženje nije pronađeno.</p>"; return; }

    const korisnik = DB.find("korisnici", z.korisnikId);
    const knjiga = DB.find("knjige", z.knjigaId);
    const primjerak = DB.find("primjerci", z.primjerakId);

    const returnUrl = Common.qs("returnUrl") || "Index";
    const korisnikId = Common.qs("korisnikId");
    const primjerakId = Common.qs("primjerakId");

    const backLink = document.getElementById("back-link");
    switch (returnUrl) {
        case "Historija":
            backLink.setAttribute("href", "historija.html");
            backLink.textContent = "← Nazad na historiju";
            break;
        case "ZaduzenjaClana":
            backLink.setAttribute("href", "zaduzenja-clana.html?korisnikId=" + korisnikId);
            backLink.textContent = "← Nazad na aktivna zaduženja";
            break;
        case "HistorijaClana":
            backLink.setAttribute("href", "historija-clana.html?korisnikId=" + korisnikId);
            backLink.textContent = "← Nazad na historiju člana";
            break;
        case "ZaduzenjaPrimjerka":
            backLink.setAttribute("href", "zaduzenja-primjerka.html?id=" + primjerakId);
            backLink.textContent = "← Nazad na zaduženja primjerka";
            break;
        default:
            backLink.setAttribute("href", "index.html");
            backLink.textContent = "← Nazad na listu";
    }

    document.getElementById("page-title").textContent = "Zaduženje #" + z.id;
    document.getElementById("knjiga-naslov").textContent = knjiga ? knjiga.naslov : "—";

    if (z.status !== "zatvoreno") {
        document.getElementById("actions-wrap").style.display = "";
        document.getElementById("vrati-link").setAttribute("href", "vrati-potvrda.html?id=" + z.id);
    }

    document.getElementById("v-clan").textContent = korisnik ? korisnik.ime + " " + korisnik.prezime : "—";
    document.getElementById("v-email").textContent = korisnik ? korisnik.email : "—";
    document.getElementById("v-knjiga").textContent = knjiga ? knjiga.naslov : "—";
    document.getElementById("v-invbroj").textContent = primjerak ? primjerak.inventarniBroj : "—";
    document.getElementById("v-datum-zaduzenja").textContent = Common.formatDate(z.datumZaduzivanja);
    document.getElementById("v-rok").textContent = Common.formatDate(z.datumPlaniranogVracanja) + " ";

    const today = new Date().toISOString().slice(0, 10);
    if (z.status !== "zatvoreno") {
        const kasni = z.datumPlaniranogVracanja < today;
        const uskoro = !kasni && z.datumPlaniranogVracanja <= new Date(Date.now() + 3 * 86400000).toISOString().slice(0, 10);
        document.getElementById("v-rok-badge").innerHTML = kasni
            ? '<span class="status-kasni">Zakašnjelo</span>'
            : uskoro ? '<span class="status-blizi">Uskoro</span>' : "";
    }

    document.getElementById("v-datum-vracanja").innerHTML = z.datumStvarnogVracanja
        ? Common.formatDate(z.datumStvarnogVracanja)
        : '<span style="color:var(--sl-muted);">-</span>';

    document.getElementById("v-status").innerHTML = `<span class="${z.status === "aktivno" ? "status-dostupan" : "status-nedostupan"}">${Common.escapeHtml(z.status)}</span>`;
})();
