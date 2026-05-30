# SmartLib — Catalog Entrance Animation: Full Implementation Prompt

> **Copy everything below the line and paste it as your prompt to the LLM in a new conversation.**

---

Read and follow the instruction files in the `.agents` folder before making any code changes. The three key files are:

- `.agents/catalog-animation.analysis.instructions.md` — Read this FIRST. It maps the entire codebase, documents every file you need to touch (with exact line numbers), lists all dead/broken code to delete, explains the existing `#book-overlay` welcome animation that you must NOT break, and enumerates constraints (Razor `@@` escaping, sessionStorage keys, z-index rules, theme system).

- `.agents/catalog-animation.implementation.instructions.md` — Read this SECOND. It contains the exact 5-beat storyboard for the animation, the complete HTML structure to add to `_Layout.cshtml`, a full CSS skeleton for the transition, the JavaScript click handler logic, theme compatibility rules, a 13-point quality checklist, and a list of 6 common mistakes to avoid.

- `.agents/catalog-animation.bugfixes.instructions.md` — Read this THIRD. It documents two visual bugs (light/dark theme flash on the welcome overlay, and the SmartLib cover title flicker) with root causes and multiple fix options.

## Your task (execute in this exact order):

### Phase 1: Cleanup
1. Open `wwwroot/css/animations.css` and DELETE all dead bookshelf CSS that no longer has corresponding HTML. This includes `.bookshelf-wood-shelf`, `.shelf-books-row`, `.bookshelf-spotlight`, all `.book-spine` / `.spine-*` variants, `#bookshelf-scene` and its `.bookshelf-zoom`/`.bookshelf-dissolve` rules, `.target-book-container`, `.catalog-book-3d`, all `.cb-*` parts, `.hint-pulse`, `hintPulse`/`hintLabelFade` keyframes, the `[data-theme="dark"] #bookshelf-scene` override, and the entire "Realistic Cinematic Catalog Transition" section at the bottom (`#realistic-transition-overlay`, `#realistic-3d-book-container`, `.hero-zoom-in`, `heroImageZoom`, `.realistic-book*`, `realisticBookPullOut`, `openRealisticCover`, `glowSurge`). Keep ALL other CSS intact (the `#book-overlay` welcome animation, particles, dark theme overrides for the overlay, reduced motion rules, etc.).
2. Open `wwwroot/js/animations.js` and DELETE the entire `.katalog-nav-link` click handler block (lines 17–84 approximately — everything from the `// ─── Navbar Katalog Link Interception` comment through the `});` that closes the `forEach`). Keep everything else intact.

### Phase 2: Build the new catalog transition
3. In `Views/Shared/_Layout.cshtml`, add the new `<div id="catalog-transition-shelf">` HTML block just before the `<nav id="main-nav">` tag. Use the exact HTML structure from the implementation instructions (backdrop, shelf with spotlight, book spines with CSS custom properties for color/height/width, the target book with `id="cts-target-book"`, the gap element, the gap glow, the wooden plank, and the white-out overlay). The div must start with `style="display:none;"`.
4. In `wwwroot/css/animations.css`, add the new "Catalog Transition Shelf (CTS)" CSS section at the end. Implement the full transition using CSS classes toggled by JS: `.cts-active` (backdrop + shelf fade in), `.cts-pull` (target book slides forward and fades, gap widens, gap glow appears), `.cts-zoom` (shelf scales up and blurs, gap glow expands to fill screen, white-out fades in). Include `[data-theme="dark"]` overrides so the white-out matches the dark page background instead of white. Include a `prefers-reduced-motion` rule.
5. In `wwwroot/js/animations.js`, add the new katalog link click handler in the same location where the old one was deleted. It must: check `disableAnimations` and `prefersReduced`, check `sessionStorage.getItem('smartlib-catalog-seen')`, prevent default on first click, set the sessionStorage flag, show the shelf container, force reflow, then add classes on a timed sequence: `cts-active` at 0ms, `cts-pull` at ~500ms, `cts-zoom` at ~1400ms, `window.location.href` at ~2800ms. Adjust these timings so the animation feels smooth and cinematic.

### Phase 3: Wire up all trigger points
6. Verify that ALL `<a>` tags pointing to `Knjiga/Index` in `_Layout.cshtml` (there are 4: one per role in the desktop nav, plus one in the mobile drawer) have `class="katalog-nav-link"`.
7. In `Views/Home/Index.cshtml`, find the "Pregledaj katalog" button (around line 313) and add `katalog-nav-link` to its class list so it also triggers the animation.

### Phase 4: Fix the theme flash bugs
8. Fix **BUG 1** (light/dark overlay flash): The `#book-overlay` welcome animation uses a dark background regardless of theme. When it dismisses in light mode, there's a jarring dark→light flash. Choose the best fix from the bugfixes instructions file (I recommend Option B: make the fade-out transition to white in light mode and to dark in dark mode, so the overlay's last frame matches the page beneath it).
9. Fix **BUG 2** (SmartLib cover flicker): The book cover is visible for ~150ms before the zoom starts. Fix by either reducing `T_ZOOM` to 0, or starting `.book-container` at `opacity: 0` and revealing it only when `phase-zoom` is added.

### Phase 5: Verify
10. Run `dotnet build Projekat/src/SmartLib.Web/SmartLib.Web.csproj` and confirm 0 errors, 0 warnings.
11. Run through the quality checklist from the implementation instructions (13 items).

## Critical constraints:
- This is an ASP.NET Core MVC project with Razor views. Inside `<style>` blocks in `.cshtml` files, CSS `@keyframes` and `@media` must be escaped as `@@keyframes` and `@@media`. In standalone `.css` files, use single `@`.
- Do NOT use any external JS libraries (no jQuery, GSAP, anime.js). Pure vanilla JS only.
- Do NOT modify the `#book-overlay` welcome animation logic (JS lines 104–198, CSS lines 1–640) EXCEPT to fix the two bugs described above.
- The catalog transition `z-index` must be 9998 (below `#book-overlay` at 9999).
- Use `sessionStorage` key `'smartlib-catalog-seen'` — do not change this key name.
- The total animation must be ≤ 3.5 seconds from click to navigation.
- The shelf HTML must be STATIC in `_Layout.cshtml`, not dynamically created by JavaScript. JS only toggles CSS classes.
- Test in both light mode AND dark mode. The final frame before navigation must match the destination page's background color.
