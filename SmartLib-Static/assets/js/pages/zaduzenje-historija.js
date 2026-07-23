(function () {
    "use strict";
    if (!Auth.guard(["Bibliotekar", "Administrator"])) return;

    const threeYearsAgo = new Date();
    threeYearsAgo.setFullYear(threeYearsAgo.getFullYear() - 3);
    const cutoff = threeYearsAgo.toISOString().slice(0, 10);
    let clanFilter = "";

    function render() {
        const q = clanFilter.trim().toLowerCase();
        const loans = DB.query("zaduzenja", (z) => z.status === "zatvoreno" && z.datumStvarnogVracanja >= cutoff).map((z) => ({
            z, korisnik: DB.find("korisnici", z.korisnikId), knjiga: DB.find("knjige", z.knjigaId), primjerak: DB.find("primjerci", z.primjerakId),
        })).filter(({ korisnik }) => !q || (korisnik && (korisnik.ime + " " + korisnik.prezime + " " + korisnik.email).toLowerCase().includes(q)))
            .sort((a, b) => b.z.datumStvarnogVracanja.localeCompare(a.z.datumStvarnogVracanja));

        document.getElementById("meta-line").innerHTML = `Prikazuju se zatvorena zaduženja iz <strong>posljednje 3 godine</strong>` +
            (q ? ` · filtriran po: <strong>${Common.escapeHtml(clanFilter)}</strong>` : "") +
            ` · ukupno: <strong>${loans.length}</strong>`;

        document.getElementById("table-wrap").style.display = loans.length ? "" : "none";
        document.getElementById("empty-wrap").style.display = loans.length ? "none" : "";
        document.getElementById("empty-msg").textContent = "Nema zatvorenih zaduženja" + (q ? " za zadani filter." : " u posljednje 3 godine.");
        if (!loans.length) return;

        document.getElementById("hist-tbody").innerHTML = loans.map(({ z, korisnik, knjiga, primjerak }) => `
            <tr>
                <td data-label="Član"><strong>${korisnik ? Common.escapeHtml(korisnik.ime + " " + korisnik.prezime) : "—"}</strong></td>
                <td data-label="Email" class="members-email">${korisnik ? Common.escapeHtml(korisnik.email) : "—"}</td>
                <td data-label="Knjiga">${knjiga ? Common.escapeHtml(knjiga.naslov) : "—"}</td>
                <td data-label="Inv. br." class="katalog-isbn">${primjerak ? Common.escapeHtml(primjerak.inventarniBroj) : "—"}</td>
                <td data-label="Zaduženo">${Common.formatDate(z.datumZaduzivanja)}</td>
                <td data-label="Rok vraćanja">${Common.formatDate(z.datumPlaniranogVracanja)}</td>
                <td data-label="Vraćeno">${z.datumStvarnogVracanja
                    ? `<span class="historija-vraceno">${Common.formatDate(z.datumStvarnogVracanja)}</span>`
                    : '<span class="historija-nepoznato">—</span>'}</td>
                <td class="katalog-actions">
                    <a href="details.html?id=${z.id}&returnUrl=Historija" class="btn btn-secondary btn-sm">Detalji</a>
                </td>
            </tr>
        `).join("");
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
