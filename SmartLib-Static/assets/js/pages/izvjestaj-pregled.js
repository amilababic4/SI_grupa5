(function () {
    "use strict";
    if (!Auth.guard(["Bibliotekar", "Administrator"])) return;

    const MONTHS = ["Januar", "Februar", "Mart", "April", "Maj", "Juni", "Juli", "Avgust", "Septembar", "Oktobar", "Novembar", "Decembar"];
    const mjesec = Number(Common.qs("mjesec", new Date().getMonth() + 1));
    const godina = Number(Common.qs("godina", new Date().getFullYear()));
    const isDark = document.documentElement.getAttribute("data-theme") === "dark";
    const textColor = isDark ? "#cbd5e1" : "#334155";

    document.getElementById("report-title").textContent = `Izvještaj o zaduživanjima — ${MONTHS[mjesec - 1]} ${godina}`;

    const prefix = `${godina}-${String(mjesec).padStart(2, "0")}`;
    const loans = DB.query("zaduzenja", (z) => z.datumZaduzivanja.startsWith(prefix)).map((z) => ({
        z, korisnik: DB.find("korisnici", z.korisnikId), knjiga: DB.find("knjige", z.knjigaId),
    }));

    const ukupno = loans.length;
    const aktivna = loans.filter(({ z }) => z.status !== "zatvoreno").length;
    const zatvorena = loans.filter(({ z }) => z.status === "zatvoreno").length;
    const zakasnjela = DB.query("zaduzenja", (z) => z.status === "zakašnjelo" && z.datumZaduzivanja.startsWith(prefix)).length;

    document.getElementById("stat-grid").innerHTML = `
        <div class="stat-card"><div class="num">${ukupno}</div><div class="label">Ukupno zaduženja</div></div>
        <div class="stat-card"><div class="num">${aktivna}</div><div class="label">Aktivna</div></div>
        <div class="stat-card"><div class="num">${zatvorena}</div><div class="label">Zatvorena</div></div>
        <div class="stat-card"><div class="num">${zakasnjela}</div><div class="label">Zakašnjela</div></div>
    `;

    const daysInMonth = new Date(godina, mjesec, 0).getDate();
    const perDay = new Array(daysInMonth).fill(0);
    loans.forEach(({ z }) => { const day = Number(z.datumZaduzivanja.slice(8, 10)); perDay[day - 1]++; });

    const bookCounts = {};
    loans.forEach(({ knjiga }) => { if (knjiga) bookCounts[knjiga.naslov] = (bookCounts[knjiga.naslov] || 0) + 1; });
    const topBooks = Object.entries(bookCounts).sort((a, b) => b[1] - a[1]).slice(0, 5);

    new Chart(document.getElementById("chart-status"), {
        type: "doughnut",
        data: { labels: ["Aktivna", "Zatvorena"], datasets: [{ data: [aktivna, zatvorena], backgroundColor: ["#2563eb", "#94a3b8"] }] },
        options: { plugins: { legend: { labels: { color: textColor } }, title: { display: true, text: "Status zaduženja", color: textColor } } },
    });
    new Chart(document.getElementById("chart-daily"), {
        type: "bar",
        data: { labels: perDay.map((_, i) => i + 1), datasets: [{ label: "Zaduženja po danu", data: perDay, backgroundColor: "#2563eb" }] },
        options: { plugins: { legend: { display: false }, title: { display: true, text: "Zaduženja po danima", color: textColor } }, scales: { x: { ticks: { color: textColor } }, y: { ticks: { color: textColor } } } },
    });
    new Chart(document.getElementById("chart-top"), {
        type: "bar",
        indexAxis: "y",
        data: { labels: topBooks.map((b) => b[0]), datasets: [{ label: "Broj zaduženja", data: topBooks.map((b) => b[1]), backgroundColor: "#c8974a" }] },
        options: { plugins: { legend: { display: false }, title: { display: true, text: "Top 5 knjiga", color: textColor } }, scales: { x: { ticks: { color: textColor } }, y: { ticks: { color: textColor } } } },
    });

    document.getElementById("tbody").innerHTML = loans.sort((a, b) => b.z.datumZaduzivanja.localeCompare(a.z.datumZaduzivanja)).map(({ z, korisnik, knjiga }) => `
        <tr>
            <td>${korisnik ? Common.escapeHtml(korisnik.ime + " " + korisnik.prezime) : "—"}</td>
            <td>${knjiga ? Common.escapeHtml(knjiga.naslov) : "—"}</td>
            <td>${Common.formatDate(z.datumZaduzivanja)}</td>
            <td>${z.status === "zatvoreno" ? Common.formatDate(z.datumStvarnogVracanja) : Common.formatDate(z.datumPlaniranogVracanja)}</td>
            <td>${Common.statusBadgeHtml(z.status)}</td>
        </tr>
    `).join("") || `<tr><td colspan="5" style="text-align:center;color:var(--sl-muted);">Nema podataka za odabrani period.</td></tr>`;
})();
