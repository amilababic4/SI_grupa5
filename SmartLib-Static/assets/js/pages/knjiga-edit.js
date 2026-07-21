(function () {
    "use strict";
    if (!Auth.guard(["Bibliotekar", "Administrator"])) return;

    const bookId = Number(Common.qs("id"));
    const knjiga = DB.find("knjige", bookId);
    if (!knjiga) { document.querySelector("main").innerHTML = "<p>Knjiga nije pronađena.</p>"; return; }

    document.getElementById("back-link").setAttribute("href", "index.html");
    document.getElementById("cancel-link").setAttribute("href", "index.html");

    const katSelect = document.getElementById("kategorija");
    DB.getAll("kategorije").sort((a, b) => a.naziv.localeCompare(b.naziv)).forEach((k) => {
        katSelect.insertAdjacentHTML("beforeend", `<option value="${k.id}">${Common.escapeHtml(k.naziv)}</option>`);
    });

    document.getElementById("naslov").value = knjiga.naslov;
    document.getElementById("autor").value = knjiga.autor;
    document.getElementById("isbn").value = knjiga.isbn;
    katSelect.value = knjiga.kategorijaId;
    document.getElementById("izdavac").value = knjiga.izdavac || "";
    document.getElementById("godina").value = knjiga.godinaIzdanja || "";

    document.getElementById("book-form").addEventListener("submit", (e) => {
        e.preventDefault();
        DB.update("knjige", bookId, {
            naslov: document.getElementById("naslov").value.trim(),
            autor: document.getElementById("autor").value.trim(),
            kategorijaId: Number(katSelect.value),
            izdavac: document.getElementById("izdavac").value.trim(),
            godinaIzdanja: Number(document.getElementById("godina").value) || null,
        });
        Common.Flash.set("success", "Izmjene su sačuvane.");
        window.location.href = "details.html?id=" + bookId;
    });
})();
