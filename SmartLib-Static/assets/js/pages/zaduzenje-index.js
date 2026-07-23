(function () {
    "use strict";
    if (!Auth.guard(["Bibliotekar", "Administrator"])) return;
    Common.Flash.renderInto(document.getElementById("alert-container"));

    const today = new Date().toISOString().slice(0, 10);
    const soonThreshold = new Date(Date.now() + 3 * 86400000).toISOString().slice(0, 10);
    let clanFilter = "";

    function render() {
        const q = clanFilter.trim().toLowerCase();
        const loans = DB.query("zaduzenja", (z) => z.status !== "zatvoreno").map((z) => ({
            z, korisnik: DB.find("korisnici", z.korisnikId), knjiga: DB.find("knjige", z.knjigaId),
        })).filter(({ korisnik }) => !q || (korisnik && (korisnik.ime + " " + korisnik.prezime + " " + korisnik.email).toLowerCase().includes(q)))
            .sort((a, b) => a.z.datumPlaniranogVracanja.localeCompare(b.z.datumPlaniranogVracanja));

        document.getElementById("table-wrap").style.display = loans.length ? "" : "none";
        document.getElementById("empty-wrap").style.display = loans.length ? "none" : "";
        document.getElementById("empty-msg").textContent = "Nema aktivnih zaduženja" + (q ? " za zadani filter." : ".");
        if (!loans.length) return;

        document.getElementById("loans-tbody").innerHTML = loans.map(({ z, korisnik, knjiga }) => {
            const primjerak = DB.find("primjerci", z.primjerakId);
            const kasni = z.datumPlaniranogVracanja < today;
            const uskoro = !kasni && z.datumPlaniranogVracanja <= soonThreshold;
            return `<tr class="${kasni ? "tr-kasni" : uskoro ? "tr-blizi" : ""}">
                <td data-label="Član"><strong>${korisnik ? Common.escapeHtml(korisnik.ime + " " + korisnik.prezime) : "—"}</strong></td>
                <td data-label="Email" class="members-email">${korisnik ? Common.escapeHtml(korisnik.email) : "—"}</td>
                <td data-label="Knjiga">${knjiga ? Common.escapeHtml(knjiga.naslov) : "—"}</td>
                <td data-label="Inv. br." class="katalog-isbn">${primjerak ? Common.escapeHtml(primjerak.inventarniBroj) : "—"}</td>
                <td data-label="Zaduženo">${Common.formatDate(z.datumZaduzivanja)}</td>
                <td data-label="Rok vraćanja">
                    ${Common.formatDate(z.datumPlaniranogVracanja)}
                    ${kasni ? '<span class="status-kasni">Zakašnjelo</span>' : uskoro ? '<span class="status-blizi">Uskoro</span>' : ""}
                </td>
                <td data-label="Akcije" class="katalog-actions">
                    <a href="details.html?id=${z.id}" class="btn btn-secondary btn-sm">Detalji</a>
                </td>
            </tr>`;
        }).join("");
    }

    document.getElementById("search-form").addEventListener("submit", (e) => {
        e.preventDefault();
        clanFilter = document.getElementById("clan-input").value;
        render();
    });
    document.getElementById("reset-btn").addEventListener("click", () => {
        document.getElementById("clan-input").value = "";
        clanFilter = "";
        render();
    });

    render();
})();
