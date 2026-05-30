# Catalog Entrance Animation — Implementation Instructions

> **PREREQUISITE:** You MUST read `catalog-animation.analysis.instructions.md` first.
> That file maps the codebase, documents bugs, and identifies what to delete.
> Do NOT start coding until you have read it completely.

---

## 1. The Vision — What the Animation Should Look and Feel Like

When the user clicks "Katalog" for the first time in a session, a **cinematic bookshelf
transition** should play. Think of it like a movie camera movement:

### Storyboard (5 beats, ~3 seconds total):

```
BEAT 1 (0ms–400ms): BOOKSHELF APPEARS
─────────────────────────────────────
A full-screen dark overlay fades in (200ms).
A realistic wooden bookshelf filled with book spines fades in on top of it.
The bookshelf should look like a real library shelf — warm wood tones, varied
book spines in different colors/heights, a warm spotlight from above.
The shelf appears centered vertically, filling about 60% of the viewport height.

BEAT 2 (400ms–1200ms): ONE BOOK SLIDES OUT
───────────────────────────────────────────
One specific book (the "SmartLib Katalog" book, positioned in the middle of
the row) starts sliding forward out of the shelf toward the viewer. It should
move on the Z-axis (perspective), growing slightly larger as it comes toward
the camera. The other books stay put. A subtle gap opens where the book was.

BEAT 3 (1200ms–2000ms): ZOOM INTO THE GAP
──────────────────────────────────────────
As the pulled book drifts off-screen (falling or sliding out of view downward
or to the side), the camera zooms INTO the gap left on the shelf. The gap
should glow warmly (as if there's light behind the books). The surrounding
books blur as the camera pushes through the gap.

BEAT 4 (2000ms–2800ms): WHITE-OUT / TRANSITION
───────────────────────────────────────────────
The warm glow from the gap fills the entire screen, becoming a bright
white wash. This is the "portal" moment — the user is passing through
the bookshelf into the catalog.

BEAT 5 (2800ms–3000ms): NAVIGATE
────────────────────────────────
`window.location.href = targetUrl` fires. The browser navigates to
`/Knjiga/Index`. The white screen naturally transitions into the catalog
page loading.
```

### What it should NOT look like:
- ❌ A random book floating in the center of the screen
- ❌ A book opening to show the word "KATALOG"
- ❌ A flat 2D image zooming in
- ❌ Any animation that takes more than 3.5 seconds
- ❌ Any flash of dark-mode content on a light-mode page (or vice versa)

---

## 2. Technical Implementation Plan

### STEP 1: Clean Up Dead Code

**In `animations.css`:**
1. DELETE all old bookshelf CSS that has no corresponding HTML. This includes:
   - `.bookshelf-wood-shelf`, `.shelf-books-row`, `.bookshelf-spotlight`
   - `.book-spine`, `.spine-*` (all spine color variants)
   - `#bookshelf-scene` and its `.bookshelf-zoom`, `.bookshelf-dissolve` rules
   - `.target-book-container`, `.catalog-book-3d`, `.cb-*` (all 3D book parts)
   - `.hint-pulse`, `hintPulse`, `hintLabelFade` keyframes
   - `.catalog-entrance` transition rules that reference old classes
   - All of `[data-theme="dark"] #bookshelf-scene` and related dark overrides
   
2. DELETE the broken "Realistic Cinematic" section (lines 1141–1277):
   - `#realistic-transition-overlay`
   - `#realistic-3d-book-container`
   - `.hero-zoom-in`, `heroImageZoom`
   - `.realistic-book`, `.realistic-book-cover`, `.realistic-book-pages`, `.realistic-book-glow`
   - `realisticBookPullOut`, `openRealisticCover`, `glowSurge` keyframes

**In `animations.js`:**
1. DELETE lines 17–84 (the `.katalog-nav-link` click handler and all its contents)

### STEP 2: Build the New Bookshelf HTML

Add a new `<div id="catalog-transition-shelf">` in `_Layout.cshtml`, placed just before
the `<nav id="main-nav">` tag. This div starts hidden (`display:none`).

```html
<!-- Catalog Entrance Transition (one-time per session) -->
<div id="catalog-transition-shelf" style="display:none;" aria-hidden="true">
    <!-- Dark backdrop -->
    <div class="cts-backdrop"></div>
    
    <!-- Shelf structure -->
    <div class="cts-shelf">
        <div class="cts-spotlight"></div>
        <div class="cts-books-row">
            <!-- Books to the LEFT of the gap -->
            <div class="cts-book" style="--h:220px;--w:38px;--bg:linear-gradient(180deg,#5c1a2a,#3d1018);--c:#ffc8b4;">KLASICI</div>
            <div class="cts-book" style="--h:210px;--w:35px;--bg:linear-gradient(180deg,#1e3a5f,#162d4a);--c:#b0c4de;">MUDROST</div>
            <div class="cts-book" style="--h:230px;--w:42px;--bg:linear-gradient(180deg,#2d4a3a,#1e3328);--c:#b4d2b4;">NAUKA</div>
            
            <!-- THE target book that slides out — MUST have id="cts-target-book" -->
            <div class="cts-book cts-target" id="cts-target-book"
                 style="--h:240px;--w:48px;--bg:linear-gradient(180deg,#173b63,#0e2a4a);--c:#c8974a;">
                KATALOG
            </div>
            
            <!-- The gap that appears when the book slides out -->
            <div class="cts-gap"></div>
            
            <!-- Books to the RIGHT of the gap -->
            <div class="cts-book" style="--h:215px;--w:40px;--bg:linear-gradient(180deg,#6b2020,#4a1515);--c:#ffc896;">POETIKA</div>
            <div class="cts-book" style="--h:200px;--w:33px;--bg:linear-gradient(180deg,#d4c9a8,#b8a88a);--c:#3d2b1f;">ISTORIJA</div>
            <div class="cts-book" style="--h:225px;--w:36px;--bg:linear-gradient(180deg,#0f1e3d,#0a142a);--c:#c8c8dc;">FILOZOFIJA</div>
            <div class="cts-book" style="--h:205px;--w:34px;--bg:linear-gradient(180deg,#4a4a2a,#333320);--c:#d2d2aa;">ROMANI</div>
        </div>
        
        <!-- Wooden shelf plank (the surface the books rest on) -->
        <div class="cts-plank"></div>
    </div>
    
    <!-- Warm glow that expands from the gap -->
    <div class="cts-gap-glow"></div>
    
    <!-- White wash overlay for the transition -->
    <div class="cts-whiteout"></div>
</div>
```

### STEP 3: Write the New CSS

Add a new clearly-commented section at the end of `animations.css`:

```css
/* ═══════════════════════════════════════════════════════════════════════════════
   Catalog Transition Shelf (CTS)
   Cinematic bookshelf → book pull-out → zoom into gap → white-out → navigate
   ═══════════════════════════════════════════════════════════════════════════════ */

#catalog-transition-shelf {
    position: fixed;
    inset: 0;
    z-index: 9998;  /* Below #book-overlay (9999) but above everything else */
    pointer-events: all;
}

.cts-backdrop {
    position: absolute;
    inset: 0;
    background: #0B162C;
    opacity: 0;
    transition: opacity 0.3s ease;
}

#catalog-transition-shelf.cts-active .cts-backdrop {
    opacity: 1;
}

.cts-shelf {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    /* Size the shelf to hold the book row */
    padding: 2rem 3rem 0 3rem;
    opacity: 0;
    transition: opacity 0.4s ease 0.15s, transform 1.5s cubic-bezier(0.16, 1, 0.3, 1);
}

#catalog-transition-shelf.cts-active .cts-shelf {
    opacity: 1;
}

/* When zooming into the gap, the shelf scales up and centers on the gap */
#catalog-transition-shelf.cts-zoom .cts-shelf {
    transform: translate(-50%, -50%) scale(3);
    filter: blur(3px);
    opacity: 0;
    transition: transform 1s cubic-bezier(0.65, 0, 0.35, 1),
                filter 0.8s ease,
                opacity 0.8s ease 0.3s;
}

.cts-spotlight {
    position: absolute;
    top: -60px;
    left: 50%;
    transform: translateX(-50%);
    width: 400px;
    height: 120px;
    background: radial-gradient(ellipse, rgba(200, 151, 74, 0.35) 0%, transparent 70%);
    pointer-events: none;
}

.cts-books-row {
    display: flex;
    align-items: flex-end;
    gap: 3px;
    position: relative;
}

.cts-book {
    width: var(--w, 40px);
    height: var(--h, 220px);
    background: var(--bg);
    color: var(--c, #fff);
    border-radius: 2px 3px 3px 2px;
    display: flex;
    align-items: center;
    justify-content: center;
    writing-mode: vertical-rl;
    text-orientation: mixed;
    font-family: 'Plus Jakarta Sans', 'Manrope', sans-serif;
    font-weight: 700;
    font-size: 0.72rem;
    letter-spacing: 0.12em;
    text-transform: uppercase;
    box-shadow: inset -2px 0 4px rgba(0,0,0,0.3), 2px 0 4px rgba(0,0,0,0.15);
    transition: transform 0.6s cubic-bezier(0.16, 1, 0.3, 1);
    flex-shrink: 0;
}

/* The target book that slides out */
.cts-target {
    z-index: 5;
    box-shadow: inset -2px 0 4px rgba(0,0,0,0.3), 2px 0 8px rgba(0,0,0,0.3),
                0 0 20px rgba(200, 151, 74, 0.2);
    transition: transform 0.8s cubic-bezier(0.16, 1, 0.3, 1),
                opacity 0.4s ease;
}

#catalog-transition-shelf.cts-pull .cts-target {
    transform: translateZ(200px) translateY(-30px) scale(1.3);
    opacity: 0;
}

/* The gap — starts at width 0, expands when the book slides out */
.cts-gap {
    width: 0;
    height: 240px;
    position: relative;
    transition: width 0.8s cubic-bezier(0.16, 1, 0.3, 1);
    overflow: visible;
}

#catalog-transition-shelf.cts-pull .cts-gap {
    width: 50px;
}

/* Warm glow from behind the gap */
.cts-gap-glow {
    position: fixed;
    top: 50%;
    left: 50%;
    width: 60px;
    height: 200px;
    transform: translate(-50%, -50%);
    background: radial-gradient(ellipse, rgba(255, 220, 150, 0.9) 0%, rgba(255, 200, 100, 0.4) 40%, transparent 70%);
    opacity: 0;
    pointer-events: none;
    transition: opacity 0.6s ease, transform 1s cubic-bezier(0.16, 1, 0.3, 1);
    z-index: 9998;
}

#catalog-transition-shelf.cts-pull .cts-gap-glow {
    opacity: 1;
}

#catalog-transition-shelf.cts-zoom .cts-gap-glow {
    transform: translate(-50%, -50%) scale(30);
    opacity: 1;
    background: radial-gradient(ellipse, rgba(255, 255, 255, 1) 0%, rgba(255, 255, 255, 0.8) 40%, rgba(255, 255, 255, 0) 70%);
    transition: transform 1.2s cubic-bezier(0.65, 0, 0.35, 1), opacity 0.3s ease;
}

/* Full white-out */
.cts-whiteout {
    position: fixed;
    inset: 0;
    background: white;
    opacity: 0;
    pointer-events: none;
    z-index: 10001;
    transition: opacity 0.4s ease;
}

#catalog-transition-shelf.cts-zoom .cts-whiteout {
    opacity: 1;
    transition: opacity 0.5s ease 0.6s;
}

/* Wooden plank */
.cts-plank {
    height: 18px;
    background: linear-gradient(180deg, #8B6F47 0%, #6B5335 50%, #5A4428 100%);
    border-radius: 0 0 3px 3px;
    box-shadow: 0 4px 12px rgba(0,0,0,0.4), inset 0 2px 0 rgba(255,255,255,0.1);
    margin-top: 0;
}

/* Dark theme adjustments */
[data-theme="dark"] .cts-backdrop {
    background: #010205;
}

[data-theme="dark"] .cts-whiteout {
    /* In dark mode, fade to the dark page background instead of white */
    background: #0f172a;
}

/* Reduced motion: skip the whole thing */
@media (prefers-reduced-motion: reduce) {
    #catalog-transition-shelf * {
        animation: none !important;
        transition-duration: 0.01ms !important;
    }
}
```

> **IMPORTANT:** The CSS above is a STARTING POINT. You are expected to refine it
> so the animation looks polished and cinematic. The key structural elements must remain:
> backdrop → shelf appears → book slides out → gap opens → zoom into gap → white-out → navigate.

### STEP 4: Write the New JavaScript

Replace the katalog link interceptor in `animations.js` (lines 17–84) with:

```javascript
// ─── Catalog Transition: Bookshelf → Pull Book → Zoom Gap → Navigate ─────────
const katalogLinks = document.querySelectorAll('.katalog-nav-link');
katalogLinks.forEach(link => {
    link.addEventListener('click', function(e) {
        if (disableAnimations || prefersReduced) return; // normal navigation

        try {
            if (sessionStorage.getItem('smartlib-catalog-seen')) return; // normal navigation
        } catch(err) {}

        e.preventDefault();
        const targetUrl = this.href;

        try { sessionStorage.setItem('smartlib-catalog-seen', '1'); } catch(err) {}

        const shelf = document.getElementById('catalog-transition-shelf');
        if (!shelf) {
            window.location.href = targetUrl;
            return;
        }

        // Show the shelf container
        shelf.style.display = 'block';
        void shelf.offsetWidth; // force reflow

        // BEAT 1: Backdrop fades in, shelf appears
        shelf.classList.add('cts-active');

        // BEAT 2: Target book slides out, gap opens
        setTimeout(() => {
            shelf.classList.add('cts-pull');
        }, 500);

        // BEAT 3 & 4: Zoom into the gap, glow expands, white-out
        setTimeout(() => {
            shelf.classList.add('cts-zoom');
        }, 1400);

        // BEAT 5: Navigate
        setTimeout(() => {
            window.location.href = targetUrl;
        }, 2800);
    });
});
```

> **IMPORTANT:** The timings above are starting values. You MUST test and adjust them
> so each beat flows smoothly into the next with no awkward pauses or jumps.

### STEP 5: Add `.katalog-nav-link` to the Home Page Button

In `Views/Home/Index.cshtml`, find the "Pregledaj katalog" button (around line 313):
```html
<a asp-controller="Knjiga" asp-action="Index" class="btn btn-secondary immersive-btn-secondary">Pregledaj katalog</a>
```
Add `katalog-nav-link` to its class list:
```html
<a asp-controller="Knjiga" asp-action="Index" class="btn btn-secondary immersive-btn-secondary katalog-nav-link">Pregledaj katalog</a>
```

---

## 3. Theme Compatibility Rules

The animation MUST look correct in both light and dark modes:

1. **Backdrop:** Use `#0B162C` (dark) normally. In `[data-theme="dark"]`, use `#010205`.
2. **White-out:** Use `white` normally. In `[data-theme="dark"]`, use the dark page
   background color (check `site.css` for `--bg` or body background in dark mode).
3. **Book spine colors:** These are self-contained with inline styles — they look fine in both modes.
4. **Shelf plank:** Wooden brown works in both modes.
5. **The gap glow:** Warm golden/white works in both modes.

> **The critical rule:** The final frame of the animation (the white-out) must match
> the background color of the PAGE that is about to load. If the catalog page loads
> with a white background in light mode, the white-out must be white. If it loads
> with a dark background in dark mode, the white-out must match that dark color.

---

## 4. Quality Checklist

Before considering this done, verify ALL of these:

- [ ] Clicking "Katalog" in the top navbar triggers the animation (first time only)
- [ ] Clicking "Pregledaj katalog" on the home page ALSO triggers the animation
- [ ] Second click on either link navigates normally without animation
- [ ] The animation plays correctly in LIGHT mode (no dark flashes)
- [ ] The animation plays correctly in DARK mode (no white flashes)
- [ ] The animation respects `prefers-reduced-motion` (skips straight to navigation)
- [ ] The animation respects `data-disable-animations="true"` on body
- [ ] The total animation duration is ≤ 3.5 seconds
- [ ] The `#book-overlay` welcome animation still works correctly (not broken)
- [ ] There is no dead/unused CSS left from the old bookshelf implementation
- [ ] The code compiles with `dotnet build` (0 errors, 0 warnings)
- [ ] The code works after `docker compose up --build -d`

---

## 5. Common Mistakes to Avoid

1. **Do NOT use `perspective` on the shelf container for the zoom.** CSS perspective
   with scale transforms creates nauseating distortion. Use `transform: scale()` only.

2. **Do NOT create elements dynamically with `innerHTML` template literals for the shelf.**
   The shelf HTML should be static in `_Layout.cshtml`. JS only toggles classes.

3. **Do NOT animate with JavaScript `setInterval` or manual frame loops.** Use CSS
   transitions/animations triggered by class toggles. JS only adds/removes classes
   and sets the navigation timeout.

4. **Do NOT forget the `void element.offsetWidth` reflow trick** between setting
   `display: block` and adding the first animation class. Without it, the browser
   batches the changes and skips the initial state.

5. **Do NOT set `z-index` higher than 9999.** The `#book-overlay` welcome animation
   uses 9999. The catalog transition should use 9998. The white-out can go to 10001
   only because it's the final frame before navigation.

6. **Do NOT use `transform: translateZ()` without `transform-style: preserve-3d`
   on the parent.** If you want the book to slide on the Z-axis, the parent needs
   `perspective` and `transform-style: preserve-3d`.
