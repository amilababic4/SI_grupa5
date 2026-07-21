(function () {
    "use strict";
    if (!Auth.guard(["Bibliotekar", "Administrator"])) return;

    const katSelect = document.getElementById("kategorija");
    DB.getAll("kategorije").sort((a, b) => a.naziv.localeCompare(b.naziv)).forEach((k) => {
        katSelect.insertAdjacentHTML("beforeend", `<option value="${k.id}">${Common.escapeHtml(k.naziv)}</option>`);
    });

    document.getElementById("book-form").addEventListener("submit", (e) => {
        e.preventDefault();
        const isbn = document.getElementById("isbn").value.trim();
        const alertContainer = document.getElementById("alert-container");
        if (DB.query("knjige", (k) => k.isbn === isbn).length) {
            alertContainer.innerHTML = '<div class="alert alert-error">Knjiga s ovim ISBN-om već postoji.</div>';
            return;
        }
        const knjiga = DB.insert("knjige", {
            naslov: document.getElementById("naslov").value.trim(),
            autor: document.getElementById("autor").value.trim(),
            isbn: isbn,
            kategorijaId: Number(katSelect.value),
            izdavac: document.getElementById("izdavac").value.trim(),
            godinaIzdanja: Number(document.getElementById("godina").value) || null,
            opis: document.getElementById("opis").value.trim(),
        });
        const brojPrimjeraka = Number(document.getElementById("broj-primjeraka").value) || 1;
        for (let i = 1; i <= brojPrimjeraka; i++) {
            DB.insert("primjerci", { knjigaId: knjiga.id, inventarniBroj: `INV-${knjiga.id}-${i}`, status: "dostupan", datumNabave: new Date().toISOString().slice(0, 10) });
        }
        Common.Flash.set("success", "Knjiga je uspješno dodana.");
        window.location.href = "index.html";
    });
})();
