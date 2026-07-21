(function () {
    "use strict";
    if (!Auth.guard(["Bibliotekar", "Administrator"])) return;

    const MONTHS = ["Januar", "Februar", "Mart", "April", "Maj", "Juni", "Juli", "Avgust", "Septembar", "Oktobar", "Novembar", "Decembar"];
    const today = new Date();
    const mjesecSelect = document.getElementById("mjesec");
    const godinaSelect = document.getElementById("godina");
    MONTHS.forEach((m, i) => mjesecSelect.insertAdjacentHTML("beforeend", `<option value="${i + 1}">${m}</option>`));
    mjesecSelect.value = today.getMonth() + 1;
    for (let y = today.getFullYear(); y >= today.getFullYear() - 3; y--) {
        godinaSelect.insertAdjacentHTML("beforeend", `<option value="${y}">${y}</option>`);
    }

    document.querySelectorAll(".report-tile").forEach((tile) => {
        tile.addEventListener("click", () => {
            document.querySelectorAll(".report-tile").forEach((t) => t.classList.remove("selected"));
            tile.classList.add("selected");
            document.getElementById("tip-izvjestaja").value = tile.getAttribute("data-tip");
        });
    });

    document.getElementById("report-form").addEventListener("submit", (e) => {
        e.preventDefault();
        const tip = document.getElementById("tip-izvjestaja").value;
        const mjesec = mjesecSelect.value;
        const godina = godinaSelect.value;
        const target = tip === "rezervacije" ? "pregled-rezervacija.html" : tip === "clanovi" ? "pregled-clanova.html" : "pregled.html";
        window.location.href = `${target}?mjesec=${mjesec}&godina=${godina}`;
    });
})();
