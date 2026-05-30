# Catalog Entrance Animation — Bug Report: Theme Flash & Cover Flicker

> **This file documents two visual bugs** that exist independently of the catalog
> transition feature. They affect the `#book-overlay` welcome animation and must
> be fixed to ensure a polished user experience.

---

## BUG 1: Light-Mode → Dark-Overlay → Light-Page Flash

### Description
When the user is in **light mode** and loads any page for the first time, they see:
1. **Frame 1:** Brief white flash (browser default before CSS loads)
2. **Frame 2:** The `#book-overlay` appears — it has a dark navy/midnight background
   (`radial-gradient(circle at center, #0B162C 0%, #040A18 100%)`)
3. **Frame 3:** The overlay plays the cinematic book animation (all dark themed)
4. **Frame 4:** Overlay dismisses (fades out) → reveals the LIGHT-themed page beneath

The transition from a **dark cinematic overlay to a bright white page** is jarring.
It feels like a camera flash or a broken transition.

### Files Involved
- `wwwroot/css/animations.css` — `#book-overlay` base styles (line ~24–42)
- `wwwroot/css/animations.css` — `[data-theme="dark"] #book-overlay` overrides (line ~807–817)
- `wwwroot/js/animations.js` — overlay dismiss logic (`dismissOverlay` function, line ~119–131)

### Root Cause
The `#book-overlay` was designed for dark mode only. There are `[data-theme="dark"]`
overrides that make it *even darker*, but there is no light-mode variant. The base
styles ARE the dark variant.

### Suggested Fix Options

**Option A (Recommended): Theme-aware overlay backgrounds**
```css
/* Light mode — warm cream/ivory tones */
#book-overlay {
    background: radial-gradient(circle at center, #f5f0e8 0%, #ede5d8 100%);
}

/* Dark mode — keep existing dark look */
[data-theme="dark"] #book-overlay {
    background: radial-gradient(circle at center, #0B162C 0%, #040A18 100%);
}
```
This requires also updating the book cover colors, shimmer colors, page background,
and text colors for light mode — it's a significant but worthwhile change.

**Option B: Match the dismiss to the destination**
Keep the overlay dark in both modes, but change the dismiss animation:
- Light mode: fade to white (not transparent) before removing the overlay
- Dark mode: fade to dark background before removing

```css
#book-overlay.fade-out {
    background: white; /* or var(--bg) */
    opacity: 0;
}
[data-theme="dark"] #book-overlay.fade-out {
    background: #0f172a;
}
```

**Option C (Simplest): Skip overlay in light mode**
In `animations.js`, check the theme before starting the cinematic sequence:
```javascript
const isDark = document.documentElement.getAttribute('data-theme') === 'dark';
if (!isDark) {
    // Skip the overlay entirely, just show menu
    dismissOverlay();
    return;
}
```
This is simple but loses the animation entirely for light-mode users.

---

## BUG 2: "SmartLib" Cover Title Visible Before Animation Starts

### Description
When the `#book-overlay` first appears, the book's front cover (showing "SmartLib"
in gold lettering) is **fully visible and static** for approximately 150ms before
the zoom animation begins. The user catches a frozen frame of the cover.

### Files Involved
- `wwwroot/js/animations.js` — `T_ZOOM = 150` (line ~141) — 150ms delay before zoom starts
- `wwwroot/js/animations.js` — `startCinematicSequence` function (line ~147)
- `wwwroot/css/animations.css` — `.book .cover` styles (the cover is visible by default)

### Root Cause
The cinematic sequence waits 150ms (`T_ZOOM`) after `document.fonts.ready` before
adding the `phase-zoom` class. During those 150ms, the overlay is visible at full
opacity with the book in its default (un-animated) position.

Additionally, there's another layer of delay: `document.fonts.ready.then(...)` adds
the `T_ZOOM` timeout, meaning the actual delay is `fonts_load_time + 150ms`.

### Suggested Fix
Reduce or eliminate the initial delay, and start the cover at `opacity: 0`:

```javascript
// Change T_ZOOM from 150 to 0
const T_ZOOM = 0;
```

Or make the book invisible until the zoom class is added:
```css
.book-container {
    opacity: 0;
    transition: opacity 0.3s ease;
}

#book-overlay.phase-zoom .book-container {
    opacity: 1;
}
```

This way, the overlay background fades in first, then the book fades in simultaneously
with the zoom, eliminating the frozen frame.

---

## Priority

- **BUG 1** is HIGH priority — it affects every single light-mode user on every page load
- **BUG 2** is MEDIUM priority — it's a brief visual glitch but noticeable on fast machines

Both should be fixed alongside or immediately after the catalog transition implementation.
