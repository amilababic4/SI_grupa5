// ═══════════════════════════════════════════════════════════════════════════════
// SmartLib — Cinematic Animations Controller
// Overlapping 4-phase book overlay + menu stagger + tab switching + catalog reveal
// ═══════════════════════════════════════════════════════════════════════════════

document.addEventListener('DOMContentLoaded', () => {
  // Respect server-rendered preference to disable animations for some roles
  const disableAnimations = document.body?.getAttribute('data-disable-animations') === 'true';
  if (disableAnimations) return;

  const prefersReduced = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches;

  const overlay = document.getElementById('book-overlay');
  const book = document.querySelector('.book');
  const $menu = document.getElementById('nav-menu');

  // ─── Navbar Katalog Link Interception ───────────────────────────────────────────────
  const katalogLinks = document.querySelectorAll('.katalog-nav-link');
  katalogLinks.forEach(link => {
      link.addEventListener('click', function(e) {
          if (disableAnimations || prefersReduced) return;
          
          try {
              if (sessionStorage.getItem('smartlib-catalog-seen')) {
                  return; // proceed normally
              }
          } catch(err) {}

          e.preventDefault();
          const targetUrl = this.href;
          
          try { sessionStorage.setItem('smartlib-catalog-seen','1'); } catch(err){}

          // Determine if we are on the home page (has .hero-book-showcase)
          const homeHero = document.querySelector('.hero-book-showcase');
          let zoomTarget = homeHero;

          if (!homeHero) {
              // Create overlay if not on home page
              const overlay = document.createElement('div');
              overlay.id = 'realistic-transition-overlay';
              document.body.appendChild(overlay);
              // Force reflow
              void overlay.offsetWidth;
              overlay.classList.add('is-active');
              zoomTarget = overlay;
          } else {
              // Smooth out other elements on home page
              const content = document.querySelector('.home-hero-content');
              if(content) content.style.opacity = '0';
          }

          // Create the 3D book container
          const bookContainer = document.createElement('div');
          bookContainer.id = 'realistic-3d-book-container';
          bookContainer.innerHTML = `
              <div class="realistic-book">
                  <div class="realistic-book-cover"></div>
                  <div class="realistic-book-pages">
                      <span style="font-family:'Plus Jakarta Sans',sans-serif;font-weight:800;color:#173b63;letter-spacing:2px;font-size:1.5rem;">KATALOG</span>
                  </div>
                  <div class="realistic-book-glow"></div>
              </div>
          `;
          document.body.appendChild(bookContainer);

          // Delay slightly to allow overlay fade in if applicable
          setTimeout(() => {
              if (zoomTarget) {
                  zoomTarget.classList.add('hero-zoom-in');
              }
              
              // Delay pulling out the book until the zoom is well underway
              setTimeout(() => {
                  bookContainer.classList.add('pull-out');
                  
                  // Navigate right as the glow peaks
                  setTimeout(() => {
                      window.location.href = targetUrl;
                  }, 1800);
              }, 600);
          }, homeHero ? 0 : 500); // Wait for overlay to fade in if not on home page
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
    const T_ZOOM     = 150;    // Phase 1: Start zoom approach (cover title already visible)
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
