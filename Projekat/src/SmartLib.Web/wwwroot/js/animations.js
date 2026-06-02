// ═══════════════════════════════════════════════════════════════════════════════
// SmartLib — Cinematic Animations Controller
// Overlapping 4-phase book overlay + menu stagger + tab switching + catalog reveal
// ═══════════════════════════════════════════════════════════════════════════════

document.addEventListener('DOMContentLoaded', () => {
  // Respect server-rendered preference to disable animations for some roles
  const disableAnimations = document.body?.getAttribute('data-disable-animations') === 'true';
  if (disableAnimations) return;

  const prefersReduced = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches;

  const isHomePage = document.body?.classList.contains('home-page');
  const ENABLE_HERO_ZOOM = false;
  let catalogTransitionInFlight = false;

  const CTS_TIMINGS = {
    PULL_DELAY_MS: 420,
    PULL_DURATION_MS: 650,
    FALL_DURATION_MS: 780,
    ZOOM_DURATION_MS: 900,
    WHITEOUT_DELAY_MS: 260,
    NAVIGATE_BUFFER_MS: 320
  };

  const CTS_PHASES = {
    FALL_START_MS: CTS_TIMINGS.PULL_DELAY_MS + CTS_TIMINGS.PULL_DURATION_MS,
    ZOOM_START_MS: CTS_TIMINGS.PULL_DELAY_MS + CTS_TIMINGS.PULL_DURATION_MS + CTS_TIMINGS.FALL_DURATION_MS,
    WHITEOUT_START_MS: CTS_TIMINGS.PULL_DELAY_MS + CTS_TIMINGS.PULL_DURATION_MS + CTS_TIMINGS.FALL_DURATION_MS + CTS_TIMINGS.WHITEOUT_DELAY_MS,
    NAVIGATE_MS: CTS_TIMINGS.PULL_DELAY_MS + CTS_TIMINGS.PULL_DURATION_MS + CTS_TIMINGS.FALL_DURATION_MS + CTS_TIMINGS.ZOOM_DURATION_MS + CTS_TIMINGS.NAVIGATE_BUFFER_MS
  };

  const HERO_TIMINGS = {
    ZOOM_START_MS: CTS_PHASES.FALL_START_MS + 120,
    ZOOM_DURATION_MS: CTS_TIMINGS.ZOOM_DURATION_MS,
    WHITEOUT_START_MS: CTS_PHASES.WHITEOUT_START_MS,
    NAVIGATE_MS: CTS_PHASES.NAVIGATE_MS
  };

  const overlay = document.getElementById('book-overlay');
  const book = document.querySelector('.book');
  const $menu = document.getElementById('nav-menu');

  // ─── Catalog Transition: Bookshelf → Pull Book → Zoom Gap → Navigate ─────────
  function markCatalogSeen() {
    try { sessionStorage.setItem('smartlib-catalog-seen', '1'); } catch (err) {}
  }

  function runHeroZoomTransition(targetUrl) {
    if (!ENABLE_HERO_ZOOM) return false;
    if (!isHomePage) return false;

    const heroImg = document.querySelector('.hero-book-showcase .hero-showcase-img');
    if (!heroImg) return false;

    const rect = heroImg.getBoundingClientRect();
    if (rect.width < 40 || rect.height < 40) return false;

    if (catalogTransitionInFlight) return true;
    catalogTransitionInFlight = true;

    runShelfFallBackdrop();

    const overlay = document.createElement('div');
    overlay.className = 'cts-hero-overlay';

    const whiteout = document.createElement('div');
    whiteout.className = 'cts-hero-whiteout';

    const clone = document.createElement('img');
    clone.className = 'cts-hero-clone';
    clone.alt = '';
    clone.src = heroImg.currentSrc || heroImg.src;
    clone.style.left = `${rect.left}px`;
    clone.style.top = `${rect.top}px`;
    clone.style.width = `${rect.width}px`;
    clone.style.height = `${rect.height}px`;
    clone.style.setProperty('--cts-hero-zoom-duration', `${HERO_TIMINGS.ZOOM_DURATION_MS}ms`);

    document.body.appendChild(overlay);
    document.body.appendChild(clone);
    document.body.appendChild(whiteout);

    const centerX = window.innerWidth / 2;
    const centerY = window.innerHeight / 2;
    const startX = rect.left + rect.width / 2;
    const startY = rect.top + rect.height / 2;
    const translateX = centerX - startX;
    const translateY = centerY - startY;
    const scale = rect.width > window.innerWidth * 0.6 ? 1.25 : 1.45;

    requestAnimationFrame(() => {
      overlay.classList.add('cts-hero-active');
    });

    setTimeout(() => {
      clone.classList.add('cts-hero-zoom');
      clone.style.transform = `translate3d(${translateX}px, ${translateY}px, 0) scale(${scale}) rotate(-3deg)`;
    }, HERO_TIMINGS.ZOOM_START_MS);

    setTimeout(() => {
      whiteout.classList.add('cts-hero-active');
    }, HERO_TIMINGS.WHITEOUT_START_MS);

    setTimeout(() => {
      window.location.href = targetUrl;
    }, HERO_TIMINGS.NAVIGATE_MS);

    return true;
  }

  function resetShelfState(shelf) {
    shelf.classList.remove('cts-active', 'cts-pull', 'cts-fall', 'cts-zoom', 'cts-whiteout', 'cts-hero-mode');
  }

  function runShelfFallBackdrop() {
    const shelf = document.getElementById('catalog-transition-shelf');
    if (!shelf) return;

    resetShelfState(shelf);
    shelf.style.setProperty('--cts-pull-duration', `${CTS_TIMINGS.PULL_DURATION_MS}ms`);
    shelf.style.setProperty('--cts-fall-duration', `${CTS_TIMINGS.FALL_DURATION_MS}ms`);
    shelf.style.setProperty('--cts-zoom-duration', `${CTS_TIMINGS.ZOOM_DURATION_MS}ms`);
    shelf.style.setProperty('--cts-whiteout-delay', `${CTS_TIMINGS.WHITEOUT_DELAY_MS}ms`);

    shelf.style.display = 'block';
    void shelf.offsetWidth;

    shelf.classList.add('cts-active', 'cts-hero-mode');

    setTimeout(() => {
      shelf.classList.add('cts-pull');
    }, CTS_TIMINGS.PULL_DELAY_MS);

    setTimeout(() => {
      shelf.classList.add('cts-fall');
    }, CTS_PHASES.FALL_START_MS);

    const cleanupDelay = CTS_PHASES.FALL_START_MS + CTS_TIMINGS.FALL_DURATION_MS + 160;
    setTimeout(() => {
      resetShelfState(shelf);
      shelf.style.display = 'none';
    }, cleanupDelay);
  }

  function runShelfTransition(targetUrl) {
    const shelf = document.getElementById('catalog-transition-shelf');
    if (!shelf) return false;

    if (catalogTransitionInFlight) return true;
    catalogTransitionInFlight = true;

    resetShelfState(shelf);

    shelf.style.setProperty('--cts-pull-duration', `${CTS_TIMINGS.PULL_DURATION_MS}ms`);
    shelf.style.setProperty('--cts-fall-duration', `${CTS_TIMINGS.FALL_DURATION_MS}ms`);
    shelf.style.setProperty('--cts-zoom-duration', `${CTS_TIMINGS.ZOOM_DURATION_MS}ms`);
    shelf.style.setProperty('--cts-whiteout-delay', `${CTS_TIMINGS.WHITEOUT_DELAY_MS}ms`);

    shelf.style.display = 'block';
    void shelf.offsetWidth; // force reflow

    // BEAT 1: Backdrop fades in, shelf appears
    shelf.classList.add('cts-active');

    // BEAT 2: Target book slides out, gap opens
    setTimeout(() => {
      shelf.classList.add('cts-pull');
    }, CTS_TIMINGS.PULL_DELAY_MS);

    // BEAT 3: The book tips and falls from the shelf
    setTimeout(() => {
      shelf.classList.add('cts-fall');
    }, CTS_PHASES.FALL_START_MS);

    // BEAT 4: Camera dives into the gap
    setTimeout(() => {
      shelf.classList.add('cts-zoom');
    }, CTS_PHASES.ZOOM_START_MS);

    // BEAT 4.5: Whiteout after the fall completes
    setTimeout(() => {
      shelf.classList.add('cts-whiteout');
    }, CTS_PHASES.WHITEOUT_START_MS);

    // BEAT 5: Navigate
    setTimeout(() => {
      window.location.href = targetUrl;
    }, CTS_PHASES.NAVIGATE_MS);

    return true;
  }

  const katalogLinks = document.querySelectorAll('.katalog-nav-link');
  katalogLinks.forEach(link => {
    link.addEventListener('click', function (e) {
      if (disableAnimations || prefersReduced) {
        markCatalogSeen();
        return; // normal navigation
      }

      try {
        if (sessionStorage.getItem('smartlib-catalog-seen')) return; // normal navigation
      } catch (err) {}

      e.preventDefault();
      const targetUrl = this.href;

      markCatalogSeen();

      if (runHeroZoomTransition(targetUrl)) return;
      if (runShelfTransition(targetUrl)) return;

      window.location.href = targetUrl;
    });
  });

  // ─── Menu Reveal (hardware-accelerated stagger) ────────────────────────────

  function animateMenu() {
    if (!$menu) return;
    const segments = $menu.querySelectorAll('.menu-item');

    segments.forEach((item, index) => {
      item.style.transition = 'opacity 0.5s cubic-bezier(0.16, 1, 0.3, 1), transform 0.5s cubic-bezier(0.16, 1, 0.3, 1)';
      item.style.transitionDelay = `${index * 45}ms`;
      item.style.marginRight = '';

      requestAnimationFrame(() => {
        item.style.opacity = '1';
        item.style.transform = 'translateY(0)';
      });
    });
  }

  // ─── Book Overlay — 4-Phase Cinematic Sequence (OVERLAPPING) ───────────────

  if (book && overlay) {
    let dismissTimer = null;
    let isDismissed = false;
    const phaseTimers = [];

    function clearPhaseTimers() {
      while (phaseTimers.length) clearTimeout(phaseTimers.pop());
    }

    function schedule(delay, action) {
      phaseTimers.push(setTimeout(action, delay));
    }

    function dismissOverlay() {
      if (isDismissed) return;
      isDismissed = true;
      if (dismissTimer) clearTimeout(dismissTimer);
      clearPhaseTimers();

      overlay.classList.add('fade-out');
      animateMenu();

      setTimeout(() => {
        try { overlay.remove(); } catch(e) { overlay.style.display = 'none'; }
      }, 800);
    }

    // ── Phase timing — ABSOLUTE offsets from t=0 (OVERLAPPING, not sequential) ──
    //
    //  0ms          150ms        1200ms       2500ms       4300ms       8300ms
    //  |-------------|-------------|-------------|-------------|-------------|
    //  layout stable  ZOOM (2s)    SHIMMER      OPEN+BOOK    CONTENT      DISMISS
    //                 ──────────── ──────────── ──────────── ────────────
    //                 (still going) (overlaps!) (overlaps!)  (overlaps!)
    //
    const T_ZOOM     = 0;      // Phase 1: Start zoom approach (cover title already visible)
    const T_SHIMMER  = 1200;   // Phase 2: Gold shimmer begins WHILE zoom still happening
    const T_OPEN     = 2500;   // Phase 3: Pullback + open begins BEFORE shimmer ends
    const T_CONTENT  = 4300;   // Phase 4: Content appears WHILE book still settling
    const T_DISMISS  = 8300;   // Auto-dismiss after reading pause

    function startCinematicSequence() {
      // Shortcut for reduced-motion users — show everything instantly
      if (prefersReduced) {
        overlay.classList.add('phase-zoom', 'phase-shimmer', 'phase-open', 'phase-content');
        book.classList.add('open');
        dismissTimer = setTimeout(dismissOverlay, 1500);
        return;
      }

      // Phase 1: Zoom in on the cover (title is ALREADY visible)
      overlay.classList.add('phase-zoom');

      // Phase 2: Decorative shimmer sweep (OVERLAPS with zoom — starts while zoom is still happening)
      schedule(T_SHIMMER - T_ZOOM, () => {
        overlay.classList.add('phase-shimmer');
      });

      // Phase 3: Camera pullback + book opens (OVERLAPS with shimmer end)
      schedule(T_OPEN - T_ZOOM, () => {
        overlay.classList.add('phase-open');
        book.classList.add('open');
      });

      // Phase 4: Inner page content reveals (OVERLAPS with book settling)
      schedule(T_CONTENT - T_ZOOM, () => {
        overlay.classList.add('phase-content');
      });

      // Auto-dismiss after all phases + reading time
      dismissTimer = setTimeout(dismissOverlay, T_DISMISS - T_ZOOM);
    }

    // Kick off after layout stabilization and fonts loaded
    document.fonts.ready.then(() => {
      schedule(T_ZOOM, startCinematicSequence);
    });

    // Click anywhere on overlay to dismiss instantly
    overlay.addEventListener('click', () => dismissOverlay());

    // Prevent dismiss when clicking inside the book
    book.addEventListener('click', (e) => e.stopPropagation());

    // Action button dismissal
    document.getElementById('close-overlay-btn')?.addEventListener('click', (e) => {
      e.stopPropagation();
      dismissOverlay();
    });

  } else {
    // No overlay — reveal menu immediately
    animateMenu();
  }

  // ─── Tab Switching ─────────────────────────────────────────────────────────

  function switchTab(oldEl, newEl) {
    if (oldEl) {
      oldEl.classList.add('slide-leave');
      setTimeout(() => oldEl.classList.remove('active', 'slide-leave'), 300);
    }
    if (newEl) {
      newEl.classList.add('slide-enter', 'active');
      requestAnimationFrame(() => newEl.classList.remove('slide-enter'));
    }
  }

  document.querySelectorAll('.tab-button').forEach(btn => {
    btn.addEventListener('click', () => {
      const target = document.querySelector(btn.dataset.target);
      const current = document.querySelector('.tab-content.active');
      if (target && target !== current) switchTab(current, target);
    });
  });

  // ─── Catalog Card Stagger ──────────────────────────────────────────────────

  function staggerCatalogCards() {
    document.querySelectorAll('.lb-list-item').forEach((item, i) => {
      item.style.setProperty('--card-index', i);
    });
    document.querySelectorAll('.lb-grid-card').forEach((card, i) => {
      card.style.setProperty('--card-index', i);
    });
  }

  staggerCatalogCards();

  // ─── Menu Trigger Backup ───────────────────────────────────────────────────

  document.getElementById('menu-trigger')?.addEventListener('click', () => {
    animateMenu();
  });
});
