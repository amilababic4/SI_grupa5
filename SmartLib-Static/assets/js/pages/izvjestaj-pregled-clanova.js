(function () {
    "use strict";
    if (!Auth.guard(["Bibliotekar", "Administrator"])) return;

    const MONTHS = ["Januar", "Februar", "Mart", "April", "Maj", "Juni", "Juli", "Avgust", "Septembar", "Oktobar", "Novembar", "Decembar"];
    const mjesec = Number(Common.qs("mjesec", new Date().getMonth() + 1));
    const godina = Number(Common.qs("godina", new Date().getFullYear()));
    const isDark = document.documentElement.getAttribute("data-theme") === "dark";
    const textColor = isDark ? "#cbd5e1" : "#334155";

    document.getElementById("report-title").textContent = `Izvještaj o članovima — ${MONTHS[mjesec - 1]} ${godina}`;

    const prefix = `${godina}-${String(mjesec).padStart(2, "0")}`;
    const today = new Date().toISOString().slice(0, 10);
    const clanovi = DB.query("korisnici", (u) => u.uloga === "Član");

    const noviClanovi = clanovi.filter((u) => u.datumKreiranja.startsWith(prefix));
    const clanarine = DB.getAll("clanarine");
    function latestClanarina(korisnikId) {
        return clanarine.filter((c) => c.korisnikId === korisnikId).sort((a, b) => b.datumIsteka.localeCompare(a.datumIsteka))[0];
    }
    const aktivnaClanarina = clanovi.filter((u) => { const c = latestClanarina(u.id); return c && c.datumIsteka >= today; }).length;
    const isteklaClanarina = clanovi.length - aktivnaClanarina;

    document.getElementById("stat-grid").innerHTML = `
        <div class="stat-card"><div class="num">${clanovi.filter((u) => u.status === "aktivan").length}</div><div class="label">Aktivnih članova</div></div>
        <div class="stat-card"><div class="num">${noviClanovi.length}</div><div class="label">Novih ovaj mjesec</div></div>
        <div class="stat-card"><div class="num">${aktivnaClanarina}</div><div class="label">Aktivna članarina</div></div>
        <div class="stat-card"><div class="num">${isteklaClanarina}</div><div class="label">Istekla članarina</div></div>
    `;

    const daysInMonth = new Date(godina, mjesec, 0).getDate();
    const perDay = new Array(daysInMonth).fill(0);
    noviClanovi.forEach((u) => { const day = Number(u.datumKreiranja.slice(8, 10)); perDay[day - 1]++; });

    const loanCounts = clanovi.map((u) => ({ u, count: DB.query("zaduzenja", (z) => z.korisnikId === u.id).length })).sort((a, b) => b.count - a.count).slice(0, 5);

    new Chart(document.getElementById("chart-status"), {
        type: "doughnut",
        data: { labels: ["Aktivna članarina", "Istekla članarina"], datasets: [{ data: [aktivnaClanarina, isteklaClanarina], backgroundColor: ["#2563eb", "#94a3b8"] }] },
        options: { plugins: { legend: { labels: { color: textColor } }, title: { display: true, text: "Status članarina", color: textColor } } },
    });
    new Chart(document.getElementById("chart-daily"), {
        type: "bar",
        data: { labels: perDay.map((_, i) => i + 1), datasets: [{ label: "Novi članovi po danu", data: perDay, backgroundColor: "#2563eb" }] },
        options: { plugins: { legend: { display: false }, title: { display: true, text: "Novi članovi po danima", color: textColor } }, scales: { x: { ticks: { color: textColor } }, y: { ticks: { color: textColor } } } },
    });
    new Chart(document.getElementById("chart-top"), {
        type: "bar",
        indexAxis: "y",
        data: { labels: loanCounts.map((c) => c.u.ime + " " + c.u.prezime), datasets: [{ label: "Ukupno zaduženja", data: loanCounts.map((c) => c.count), backgroundColor: "#c8974a" }] },
        options: { plugins: { legend: { display: false }, title: { display: true, text: "Top 5 članova po aktivnosti", color: textColor } }, scales: { x: { ticks: { color: textColor } }, y: { ticks: { color: textColor } } } },
    });

    document.getElementById("tbody").innerHTML = clanovi.map((u) => {
        const c = latestClanarina(u.id);
        const loans = DB.query("zaduzenja", (z) => z.korisnikId === u.id && z.datumZaduzivanja.startsWith(prefix)).length;
        const rez = DB.query("rezervacije", (r) => r.korisnikId === u.id && r.datumRezervacije.startsWith(prefix)).length;
        return `<tr>
            <td><a href="../korisnik/profil.html?id=${u.id}">${Common.escapeHtml(u.ime + " " + u.prezime)}</a></td>
            <td>${c ? (c.datumIsteka >= today ? '<span class="status-badge status-badge--aktivno">Aktivna</span>' : '<span class="status-badge status-badge--zatvoreno">Istekla</span>') : "—"}</td>
            <td>${loans}</td>
            <td>${rez}</td>
        </tr>`;
    }).join("");
})();
