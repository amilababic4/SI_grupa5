# Catalog Entrance Animation — Analysis & Codebase Map

> **Purpose of this file:** Before writing any code, an agent MUST read this file
> to understand the full codebase state, known bugs, design constraints, and the
> exact files involved. Do NOT skip any section.

---

## 1. Project Overview

SmartLib is an ASP.NET Core 8 MVC web app (Razor views, no SPA framework).
It uses server-rendered HTML with client-side JS for animations and interactivity.

- **CSS:** `wwwroot/css/site.css` (main styles) + `wwwroot/css/animations.css` (all animation styles)
- **JS:** `wwwroot/js/animations.js` (single animation controller, runs on DOMContentLoaded)
- **Layout:** `Views/Shared/_Layout.cshtml` (shared shell — navbar, drawer, footer, theme toggle)
- **Home page:** `Views/Home/Index.cshtml` (hero section with book stack image)
- **Catalog page:** `Views/Knjiga/Index.cshtml` (the destination — book catalog listing)
- **Theme system:** `localStorage.getItem('smartlib-theme')` → `data-theme="dark"` on `<html>`.
  A blocking `<script>` in `_Layout.cshtml` line 11–16 sets the theme before first paint.

---

## 2. What Already Exists (Current Animation State)

### 2.1 The "Book Overlay" Welcome Animation (UNRELATED — do NOT touch)

There is a full-screen cinematic book-opening overlay (`#book-overlay` in `_Layout.cshtml`)
that plays when the site first loads (any page). This is the "SmartLib" branded book that
zooms in, shimmers, opens, shows welcome content, then dismisses.

- **HTML:** `#book-overlay` block in `_Layout.cshtml` (~line 210–295 area)
- **CSS:** Lines 1–1139 of `animations.css` (everything before the "Realistic Cinematic" section)
- **JS:** The `#book-overlay` / `.book` code block in `animations.js` lines 104–198

> **CRITICAL: This overlay is a SEPARATE animation. Do NOT modify it, remove it,
> or interfere with its timing. The catalog transition is a DIFFERENT feature.**

### 2.2 The Broken Catalog Transition (MUST BE REPLACED)

The current catalog-link animation code is broken and needs complete replacement.
Here is what exists now and why it fails:

#### In `animations.js` (lines 17–84):
- Intercepts clicks on `.katalog-nav-link` elements
- Checks `sessionStorage.getItem('smartlib-catalog-seen')` for one-time behavior
- On first click: prevents navigation, creates a dynamic `#realistic-3d-book-container`
  div, applies `.hero-zoom-in` to the home hero image (or creates an overlay on other pages),
  then navigates after timeouts
- **Problems:**
  - The 3D book appears out of nowhere in the center of the screen — no connection to the bookshelf
  - The book opens and just shows "KATALOG" text — looks cheap and purposeless
  - The animation has no narrative (no bookshelf → pull book → zoom into gap → catalog)
  - On non-home pages, a dark overlay appears for a split second before zooming, creating a flash

#### In `animations.css` (lines 1141–1277):
- `#realistic-transition-overlay` — an overlay for non-home pages
- `#realistic-3d-book-container` — a floating 3D book with cover image
- `.hero-zoom-in` / `heroImageZoom` — zooms into the hero
- `.realistic-book-cover`, `.realistic-book-pages`, `.realistic-book-glow` — 3D book parts
- `realisticBookPullOut`, `openRealisticCover`, `glowSurge` keyframes
- **ALL of this code (lines 1141–1277) must be deleted and replaced**

#### In `animations.css` (lines 640–1060):
- Old bookshelf scene CSS (`.bookshelf-wood-shelf`, `.shelf-books-row`, `.book-spine`,
  `.target-book-container`, `.catalog-book-3d`, etc.)
- `#bookshelf-scene` zoom/dissolve transitions
- These classes are referenced in CSS but the HTML that used them was removed from
  `_Layout.cshtml`. This is dead CSS code.
- **This dead CSS (approximately lines 640–1060) should be cleaned up too**

#### In `_Layout.cshtml`:
- The `<div id="bookshelf-scene">` was already removed (previously was ~line 300)
- The navbar links have `class="katalog-nav-link"` on all Katalog `<a>` tags — **keep this class**
- The "Pregledaj katalog" button on the home page (in `Home/Index.cshtml` line 313) does NOT
  have `.katalog-nav-link` — **it should also get this class**

### 2.3 Assets

- `wwwroot/images/smartlib-hero-books.png` — the hero book stack image (used on home page)
- `wwwroot/images/realistic_book_cover.png` — a generated dark blue leather book cover (available)
- `wwwroot/images/booooks.png`, `bookStore.jpg`, `shelvesBooks.jpg` — other library images

---

## 3. Known Bugs & Issues to Fix

### BUG 1: Light/Dark Theme Flash on First Load

**Symptom:** When the site loads in light mode, the `#book-overlay` (welcome animation)
uses a dark background (`#0B162C`). This means the user sees:
1. A dark cinematic overlay (correct — this is the welcome animation)
2. The overlay dismisses → reveals the actual page in LIGHT mode
3. This transition is jarring — dark overlay dissolving into a bright white page

**Root cause:** The `#book-overlay` always uses dark colors regardless of theme.
The `[data-theme="dark"]` overrides in CSS only make it *darker*, not lighter.

**Required fix:** The `#book-overlay` should respect the current theme:
- In dark mode: keep the current dark cinematic look
- In light mode: use a warm, light-toned version (cream/ivory background, dark text)
  so the transition to the light page is seamless
- OR: skip the book overlay entirely when in light mode (simplest fix)
- OR: ensure the overlay's dismiss animation fades to white when in light mode

### BUG 2: "SmartLib" Cover Flash

**Symptom:** The book overlay's front cover shows "SmartLib" title for a fraction of a
second before the zoom animation kicks in. The user catches a glimpse of the cover
before the cinematic sequence properly starts.

**Root cause:** The cover is visible at `opacity: 1` from the start. The zoom phase
(`phase-zoom` class) takes 150ms to begin (see `T_ZOOM = 150` in `animations.js` line 141).
During those 150ms the static cover is fully visible with no animation.

**Required fix:** Either:
- Start the cover at `opacity: 0` and fade it in as part of phase-zoom
- OR reduce T_ZOOM to 0 so the animation starts instantly
- OR use `visibility: hidden` initially and reveal with the zoom class

### BUG 3: Catalog Transition Has No Narrative

**Symptom:** Clicking "Katalog" shows a random book floating in screen center, opening to
show "KATALOG" text, then navigating. There's no bookshelf, no pulling, no gap.

**This is the main issue this instruction set exists to solve. See the implementation
instructions file for the correct approach.**

---

## 4. File Map — What to Edit

| File | What to do |
|------|-----------|
| `wwwroot/css/animations.css` lines 640–1060 | **DELETE** — dead bookshelf CSS (no HTML uses it) |
| `wwwroot/css/animations.css` lines 1141–1277 | **DELETE & REPLACE** — broken realistic transition |
| `wwwroot/js/animations.js` lines 17–84 | **DELETE & REPLACE** — broken katalog link interceptor |
| `Views/Shared/_Layout.cshtml` | Verify `.katalog-nav-link` class is on all Katalog links |
| `Views/Home/Index.cshtml` line 313 | Add `.katalog-nav-link` to "Pregledaj katalog" button |

---

## 5. Constraints

1. **Razor `@@` escaping:** Inside `<style>` blocks in `.cshtml` files, `@` must be doubled
   to `@@` for CSS at-rules (`@@keyframes`, `@@media`). In standalone `.css` files, use single `@`.

2. **No new JS frameworks:** Pure vanilla JS only. No jQuery, no GSAP, no anime.js.

3. **sessionStorage key:** Use `'smartlib-catalog-seen'` for the one-time flag. Do not change this.

4. **`data-disable-animations` attribute:** Check `document.body?.getAttribute('data-disable-animations')`
   before running any animation. If `'true'`, skip everything.

5. **`prefers-reduced-motion`:** Already checked in `animations.js` line 11. Respect it.

6. **Do NOT modify the `#book-overlay` welcome animation** (lines 104–198 in JS, lines 1–640 in CSS)
   unless specifically fixing the light/dark theme flash bug described in section 3.

7. **The catalog page itself** (`Views/Knjiga/Index.cshtml`) should NOT contain animation markup.
   All animation markup should live in `_Layout.cshtml` or be injected by JS.

8. **Docker deployment:** The project runs via `docker compose up --build -d` from the
   `Projekat/` directory. Static files in `wwwroot/` are served directly.
