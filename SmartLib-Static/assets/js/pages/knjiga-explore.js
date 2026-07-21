(function () {
    "use strict";
    if (!Auth.guard(["Član"])) return;
    const user = Auth.currentUser();

    // ── Build the card deck (mirrors ExploreViewModel: preferred + surprise categories) ──
    const allBooks = DB.getAll("knjige");
    const kategorije = DB.getAll("kategorije");
    const closedLoans = DB.query("zaduzenja", (z) => z.korisnikId === user.id && z.status === "zatvoreno");
    const preferredCatIds = [...new Set(closedLoans.map((z) => { const k = DB.find("knjige", z.knjigaId); return k ? k.kategorijaId : null; }).filter(Boolean))];
    const surpriseCatIds = kategorije.map((k) => k.id).filter((id) => !preferredCatIds.includes(id));

    function poolFor(catIds) { return allBooks.filter((k) => catIds.includes(k.kategorijaId)); }
    function pickN(arr, n) {
        const copy = arr.slice(); const out = [];
        while (copy.length && out.length < n) out.push(copy.splice(Math.floor(Math.random() * copy.length), 1)[0]);
        return out;
    }

    const preferredPicks = pickN(poolFor(preferredCatIds.length ? preferredCatIds : kategorije.map((k) => k.id)), 7);
    const usedIds = new Set(preferredPicks.map((b) => b.id));
    const surprisePool = poolFor(surpriseCatIds).filter((b) => !usedIds.has(b.id));
    const surprisePicks = pickN(surprisePool.length ? surprisePool : allBooks.filter((b) => !usedIds.has(b.id)), 3);

    const cards = pickN([...preferredPicks.map((b) => ({ book: b, wild: false })), ...surprisePicks.map((b) => ({ book: b, wild: true }))], 10);

    if (!cards.length) {
        document.getElementById("explore-empty").style.display = "block";
        document.getElementById("explore-deck-shell").style.display = "none";
    } else {
        const deck = document.getElementById("explore-deck");
        deck.dataset.userId = String(user.id);
        deck.innerHTML = cards.map((c, i) => {
            const kategorija = DB.find("kategorije", c.book.kategorijaId);
            return `<article class="explore-card" style="--i:${i}" data-index="${i}" data-id="${c.book.id}"
                data-title="${Common.escapeHtml(c.book.naslov)}" data-authors="${Common.escapeHtml(c.book.autor)}"
                data-category="${kategorija ? Common.escapeHtml(kategorija.naziv) : ""}" aria-hidden="${i === 0 ? "false" : "true"}">
                <div class="explore-card-body">
                    <div class="explore-card-badges">
                        <span class="explore-badge">${kategorija ? Common.escapeHtml(kategorija.naziv) : ""}</span>
                        ${c.wild ? `<span class="explore-badge explore-badge--wild">Iznenađenje</span>` : ""}
                    </div>
                    <h3 class="explore-card-title">${Common.escapeHtml(c.book.naslov)}</h3>
                    <div class="explore-card-author">${Common.escapeHtml(c.book.autor)}</div>
                    <p class="explore-card-desc">${Common.escapeHtml((c.book.opis || "").slice(0, 160))}</p>
                </div>
                <div class="explore-card-media"><img src="${Common.bookCoverUrl(c.book.naslov)}" alt="${Common.escapeHtml(c.book.naslov)}" loading="lazy" /></div>
            </article>`;
        }).join("");
        document.getElementById("explore-progress").textContent = `1/${cards.length}`;
        runSwipeDeck();
    }

    function runSwipeDeck() {
        const deck = document.getElementById("explore-deck");
        const cardEls = Array.from(document.querySelectorAll(".explore-card"));
        const progress = document.getElementById("explore-progress");
        const controls = document.getElementById("explore-controls");
        const recommendation = document.getElementById("explore-recommendation");
        const sidepanelContent = document.getElementById("explore-sidepanel-content");
        const buttons = document.querySelectorAll("[data-swipe]");
        const sidepanel = document.querySelector(".explore-sidepanel");
        let currentIndex = 0;
        if (!cardEls.length) return;

        const storageKey = `explore_swipes_v1_${deck.dataset.userId || "anon"}`;
        const readState = () => {
            try {
                const raw = localStorage.getItem(storageKey);
                if (!raw) return { liked: [], disliked: [] };
                const parsed = JSON.parse(raw);
                return { liked: Array.isArray(parsed.liked) ? parsed.liked : [], disliked: Array.isArray(parsed.disliked) ? parsed.disliked : [] };
            } catch { return { liked: [], disliked: [] }; }
        };
        const writeState = (s) => localStorage.setItem(storageKey, JSON.stringify(s));
        const getCardData = (card) => ({ id: card.dataset.id, title: card.dataset.title || "", authors: card.dataset.authors || "", category: card.dataset.category || "" });
        const cardKey = (d) => `${d.title}|${d.authors}`.toLowerCase();

        const recordSwipe = (card, liked) => {
            const data = getCardData(card);
            const key = cardKey(data);
            const state = readState();
            state.liked = state.liked.filter((item) => cardKey(item) !== key);
            state.disliked = state.disliked.filter((item) => cardKey(item) !== key);
            (liked ? state.liked : state.disliked).push(data);
            writeState(state);
            return state;
        };

        const renderRecommendation = (state) => {
            if (!recommendation || !sidepanelContent) return;
            let book = null;
            if (state.liked.length) {
                const category = state.liked[state.liked.length - 1]?.category || "";
                const kat = kategorije.find((k) => k.naziv === category);
                const candidates = kat ? allBooks.filter((b) => b.kategorijaId === kat.id) : [];
                book = candidates.length ? candidates[Math.floor(Math.random() * candidates.length)] : allBooks[Math.floor(Math.random() * allBooks.length)];
            }
            if (!book) {
                recommendation.innerHTML = '<p class="explore-reco-empty">Nema dovoljno feedbacka za personalizovanu preporuku. Označi nekoliko knjiga sa "Sviđa mi se".</p>';
            } else {
                recommendation.innerHTML = `
                    <div class="explore-reco-header" style="margin-bottom: 1.5rem;"><h3>Preporuka iz kataloga</h3><p>Na osnovu tvojih sviđanja</p></div>
                    <div class="explore-reco-card">
                        <div class="explore-reco-body">
                            <div class="explore-reco-title">${Common.escapeHtml(book.naslov)}</div>
                            <div class="explore-reco-author">${Common.escapeHtml(book.autor)}</div>
                            <div class="explore-reco-category">${(() => { const k = DB.find("kategorije", book.kategorijaId); return k ? Common.escapeHtml(k.naziv) : ""; })()}</div>
                            <a class="btn btn-secondary" href="details.html?id=${book.id}">Detalji</a>
                        </div>
                        <div class="explore-reco-media"><img src="${Common.bookCoverUrl(book.naslov)}" alt="${Common.escapeHtml(book.naslov)}" loading="lazy" /></div>
                    </div>`;
            }
            recommendation.hidden = false;
            sidepanelContent.hidden = true;
            if (controls) { controls.hidden = true; controls.style.display = "none"; }
            if (progress) progress.hidden = true;
            document.body.classList.add("explore-complete");
            if (sidepanel) sidepanel.classList.add("is-recommendation");
        };

        const updateActive = () => {
            cardEls.forEach((card, idx) => {
                const isActive = idx === currentIndex;
                card.classList.toggle("is-active", isActive);
                card.setAttribute("aria-hidden", isActive ? "false" : "true");
            });
            if (progress) progress.textContent = `${Math.min(currentIndex + 1, cardEls.length)}/${cardEls.length}`;
            if (currentIndex >= cardEls.length) {
                if (controls) { controls.hidden = true; controls.style.display = "none"; }
                if (progress) progress.hidden = true;
                document.body.classList.add("explore-complete");
            }
        };

        const swipe = (direction) => {
            const card = cardEls[currentIndex];
            if (!card) return;
            const state = recordSwipe(card, direction === "right");
            card.classList.add(direction === "right" ? "swipe-right" : "swipe-left");
            let finalized = false;
            const finalizeSwipe = () => {
                if (finalized) return;
                finalized = true;
                card.classList.add("is-gone");
                currentIndex += 1;
                updateActive();
                if (currentIndex >= cardEls.length) renderRecommendation(state);
            };
            card.addEventListener("transitionend", finalizeSwipe, { once: true });
            setTimeout(finalizeSwipe, 500);
        };

        buttons.forEach((btn) => btn.addEventListener("click", () => swipe(btn.dataset.swipe)));

        let dragStartX = 0, isDragging = false;
        cardEls.forEach((card) => {
            card.addEventListener("pointerdown", (event) => {
                if (!card.classList.contains("is-active")) return;
                isDragging = true; dragStartX = event.clientX; card.setPointerCapture(event.pointerId);
            });
            card.addEventListener("pointerup", (event) => {
                if (!isDragging) return;
                const diff = event.clientX - dragStartX;
                isDragging = false;
                if (Math.abs(diff) > 80) swipe(diff > 0 ? "right" : "left");
            });
        });

        updateActive();
    }
})();
