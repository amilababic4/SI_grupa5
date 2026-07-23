(function () {
    "use strict";
    if (!Auth.guard()) return;
    const user = Auth.currentUser();

    document.getElementById("create-post-form").addEventListener("submit", (e) => {
        e.preventDefault();
        const post = DB.insert("forumObjave", {
            naslov: document.getElementById("naslov").value.trim(),
            sadrzaj: document.getElementById("sadrzaj").value.trim(),
            kategorija: document.getElementById("kategorija").value,
            datumKreiranja: new Date().toISOString().slice(0, 10),
            zakljucana: false,
            korisnikId: user.id,
        });
        window.location.href = "details.html?id=" + post.id;
    });
})();
