(function () {
    "use strict";
    if (!Auth.guard(["Bibliotekar", "Administrator"])) return;

    const MONTHS = ["Januar", "Februar", "Mart", "April", "Maj", "Juni", "Juli", "Avgust", "Septembar", "Oktobar", "Novembar", "Decembar"];
    const mjesec = Number(Common.qs("mjesec", new Date().getMonth() + 1));
    const godina = Number(Common.qs("godina", new Date().getFullYear()));
    const isDark = document.documentElement.getAttribute("data-theme") === "dark";
    const textColor = isDark ? "#cbd5e1" : "#334155";

    document.getElementById("report-title").textContent = `Izvještaj o rezervacijama — ${MONTHS[mjesec - 1]} ${godina}`;

    const prefix = `${godina}-${String(mjesec).padStart(2, "0")}`;
    const rows = DB.query("rezervacije", (r) => r.datumRezervacije.startsWith(prefix)).map((r) => ({
        r, korisnik: DB.find("korisnici", r.korisnikId), knjiga: DB.find("knjige", r.knjigaId),
    }));

    const ukupno = rows.length;
    const aktivne = rows.filter(({ r }) => r.status === "aktivna").length;
    const zavrsene = rows.filter(({ r }) => r.status === "realizovana").length;
    const otkazane = rows.filter(({ r }) => r.status === "otkazana" || r.status === "istekla").length;

    document.getElementById("stat-grid").innerHTML = `
        <div class="stat-card"><div class="num">${ukupno}</div><div class="label">Ukupno rezervacija</div></div>
        <div class="stat-card"><div class="num">${aktivne}</div><div class="label">Aktivne</div></div>
        <div class="stat-card"><div class="num">${zavrsene}</div><div class="label">Realizovane</div></div>
        <div class="stat-card"><div class="num">${otkazane}</div><div class="label">Otkazane/Istekle</div></div>
    `;

    const daysInMonth = new Date(godina, mjesec, 0).getDate();
    const perDay = new Array(daysInMonth).fill(0);
    rows.forEach(({ r }) => { const day = Number(r.datumRezervacije.slice(8, 10)); perDay[day - 1]++; });

    const bookCounts = {};
    rows.forEach(({ knjiga }) => { if (knjiga) bookCounts[knjiga.naslov] = (bookCounts[knjiga.naslov] || 0) + 1; });
    const topBooks = Object.entries(bookCounts).sort((a, b) => b[1] - a[1]).slice(0, 5);

    new Chart(document.getElementById("chart-status"), {
        type: "doughnut",
        data: { labels: ["Aktivne", "Realizovane", "Otkazane/Istekle"], datasets: [{ data: [aktivne, zavrsene, otkazane], backgroundColor: ["#2563eb", "#10b981", "#94a3b8"] }] },
        options: { plugins: { legend: { labels: { color: textColor } }, title: { display: true, text: "Status rezervacija", color: textColor } } },
    });
    new Chart(document.getElementById("chart-daily"), {
        type: "bar",
        data: { labels: perDay.map((_, i) => i + 1), datasets: [{ label: "Rezervacije po danu", data: perDay, backgroundColor: "#2563eb" }] },
        options: { plugins: { legend: { display: false }, title: { display: true, text: "Rezervacije po danima", color: textColor } }, scales: { x: { ticks: { color: textColor } }, y: { ticks: { color: textColor } } } },
    });
    new Chart(document.getElementById("chart-top"), {
        type: "bar",
        indexAxis: "y",
        data: { labels: topBooks.map((b) => b[0]), datasets: [{ label: "Broj rezervacija", data: topBooks.map((b) => b[1]), backgroundColor: "#c8974a" }] },
        options: { plugins: { legend: { display: false }, title: { display: true, text: "Top 5 knjiga", color: textColor } }, scales: { x: { ticks: { color: textColor } }, y: { ticks: { color: textColor } } } },
    });

    document.getElementById("tbody").innerHTML = rows.sort((a, b) => b.r.datumRezervacije.localeCompare(a.r.datumRezervacije)).map(({ r, korisnik, knjiga }) => `
        <tr>
            <td>${korisnik ? Common.escapeHtml(korisnik.ime + " " + korisnik.prezime) : "—"}</td>
            <td>${knjiga ? Common.escapeHtml(knjiga.naslov) : "—"}</td>
            <td>${Common.formatDate(r.datumRezervacije)}</td>
            <td>${Common.formatDate(r.datumIsteka)}</td>
            <td>${Common.statusBadgeHtml(r.status)}</td>
        </tr>
    `).join("") || `<tr><td colspan="5" style="text-align:center;color:var(--sl-muted);">Nema podataka za odabrani period.</td></tr>`;
})();
