(function () {
    "use strict";
    if (!Auth.guard(["Član"])) return;

    const user = Auth.currentUser();
    const cards = DB.getAll("knjige").sort(() => Math.random() - 0.5).slice(0, 8);
    let index = 0;
    const liked = [];

    function renderCard() {
        const container = document.getElementById("swipe-container");
        if (index >= cards.length) { renderDone(); return; }
        const k = cards[index];
        const kategorija = DB.find("kategorije", k.kategorijaId);
        container.innerHTML = `
            <div class="swipe-card">
                <img src="${Common.bookCoverUrl(k.naslov)}" alt="${Common.escapeHtml(k.naslov)}" />
                <h3>${Common.escapeHtml(k.naslov)}</h3>
                <div class="cat">${Common.escapeHtml(k.autor)} · ${kategorija ? Common.escapeHtml(kategorija.naziv) : ""}</div>
                <p style="color:var(--sl-muted);font-size:.88rem;flex:1;">${Common.escapeHtml((k.opis || "").slice(0, 140))}</p>
            </div>
            <div class="swipe-actions">
                <button class="swipe-btn skip" id="skip-btn" title="Preskoči">✕</button>
                <button class="swipe-btn like" id="like-btn" title="Sviđa mi se">♥</button>
            </div>
            <p style="color:var(--sl-muted);font-size:.8rem;margin-top:1rem;">${index + 1} / ${cards.length}</p>
        `;
        document.getElementById("skip-btn").addEventListener("click", () => { index++; renderCard(); });
        document.getElementById("like-btn").addEventListener("click", () => { liked.push(k); index++; renderCard(); });
    }

    function renderDone() {
        const container = document.getElementById("swipe-container");
        let recommendation = null;
        if (liked.length) {
            const katCounts = {};
            liked.forEach((k) => { katCounts[k.kategorijaId] = (katCounts[k.kategorijaId] || 0) + 1; });
            const topKatId = Object.keys(katCounts).sort((a, b) => katCounts[b] - katCounts[a])[0];
            const candidates = DB.query("knjige", (k) => String(k.kategorijaId) === topKatId && !liked.find((l) => l.id === k.id));
            recommendation = candidates.length ? candidates[Math.floor(Math.random() * candidates.length)] : null;
        }
        if (!recommendation) {
            const all = DB.getAll("knjige");
            recommendation = all[Math.floor(Math.random() * all.length)];
        }
        container.innerHTML = `
            <div class="explore-done">
                <h3>Hvala! Lajkovali ste ${liked.length} ${liked.length === 1 ? "naslov" : "naslova"}.</h3>
                <div class="recommend-card">
                    <p style="color:var(--sl-muted);font-size:.85rem;">Preporuka za vas:</p>
                    <img src="${Common.bookCoverUrl(recommendation.naslov)}" style="width:120px;border-radius:10px;margin:.5rem 0;" alt="" />
                    <h3>${Common.escapeHtml(recommendation.naslov)}</h3>
                    <p style="color:var(--sl-muted);">${Common.escapeHtml(recommendation.autor)}</p>
                    <a href="details.html?id=${recommendation.id}" class="btn btn-primary" style="margin-top:.75rem;">Pogledaj knjigu</a>
                </div>
                <button class="btn btn-secondary" id="restart-btn" style="margin-top:1.25rem;">Igraj ponovo</button>
            </div>
        `;
        document.getElementById("restart-btn").addEventListener("click", () => window.location.reload());
    }

    renderCard();
})();
