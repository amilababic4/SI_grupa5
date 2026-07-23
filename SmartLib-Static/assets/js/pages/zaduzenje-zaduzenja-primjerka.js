(function () {
    "use strict";
    if (!Auth.guard(["Bibliotekar", "Administrator"])) return;

    const primjerakId = Number(Common.qs("id"));
    const primjerak = DB.find("primjerci", primjerakId);
    if (!primjerak) { document.querySelector("main").innerHTML = "<p>Primjerak nije pronađen.</p>"; return; }
    const knjiga = DB.find("knjige", primjerak.knjigaId);

    document.getElementById("back-link").setAttribute("href", "../knjiga/details.html?id=" + primjerak.knjigaId);
    document.getElementById("primjerak-badge").textContent = primjerak.inventarniBroj;

    const loans = DB.query("zaduzenja", (z) => z.primjerakId === primjerakId).sort((a, b) => b.datumZaduzivanja.localeCompare(a.datumZaduzivanja));
    const aktivna = loans.filter((z) => z.status !== "zatvoreno").length;
    const zatvorena = loans.filter((z) => z.status === "zatvoreno").length;

    document.getElementById("meta-line").innerHTML =
        (knjiga ? `Knjiga: <strong>${Common.escapeHtml(knjiga.naslov)}</strong> · ` : "") +
        `Ukupno: <strong>${loans.length}</strong>` +
        (aktivna > 0 ? ` · <span class="status-dostupan">${aktivna} aktivno</span>` : "") +
        (zatvorena > 0 ? ` · <span style="color:var(--sl-muted)">${zatvorena} zatvoreno</span>` : "");

    document.getElementById("table-wrap").style.display = loans.length ? "" : "none";
    document.getElementById("empty-wrap").style.display = loans.length ? "none" : "";
    if (!loans.length) return;

    const today = new Date().toISOString().slice(0, 10);
    const soonThreshold = new Date(Date.now() + 3 * 86400000).toISOString().slice(0, 10);

    document.getElementById("tbody").innerHTML = loans.map((z) => {
        const korisnik = DB.find("korisnici", z.korisnikId);
        const kasni = z.status !== "zatvoreno" && z.datumPlaniranogVracanja < today;
        const uskoro = z.status !== "zatvoreno" && !kasni && z.datumPlaniranogVracanja <= soonThreshold;

        let statusKlasa, statusLabel;
        if (z.status === "zatvoreno") { statusKlasa = "status-nedostupan"; statusLabel = "zatvoreno"; }
        else if (kasni) { statusKlasa = "status-kasni"; statusLabel = "zakašnjelo"; }
        else { statusKlasa = "status-dostupan"; statusLabel = "aktivno"; }

        return `<tr class="${kasni ? "tr-kasni" : uskoro ? "tr-blizi" : ""}">
            <td data-label="Član"><strong>${korisnik ? Common.escapeHtml(korisnik.ime + " " + korisnik.prezime) : "—"}</strong></td>
            <td data-label="Email" class="members-email">${korisnik ? Common.escapeHtml(korisnik.email) : "—"}</td>
            <td data-label="Zaduženo">${Common.formatDate(z.datumZaduzivanja)}</td>
            <td data-label="Rok vraćanja">
                ${Common.formatDate(z.datumPlaniranogVracanja)}
                ${kasni ? '<span class="status-kasni">Zakašnjelo</span>' : uskoro ? '<span class="status-blizi">Uskoro</span>' : ""}
            </td>
            <td data-label="Vraćeno">${z.datumStvarnogVracanja
                ? `<span class="historija-vraceno">${Common.formatDate(z.datumStvarnogVracanja)}</span>`
                : '<span class="historija-nepoznato">—</span>'}</td>
            <td data-label="Status"><span class="${statusKlasa}">${statusLabel}</span></td>
            <td class="katalog-actions">
                <a href="details.html?id=${z.id}&returnUrl=ZaduzenjaPrimjerka&primjerakId=${primjerakId}" class="btn btn-secondary btn-sm">Detalji</a>
            </td>
        </tr>`;
    }).join("");
})();
