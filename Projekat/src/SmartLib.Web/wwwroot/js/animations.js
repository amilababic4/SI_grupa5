// Animations initializer for SmartLib
document.addEventListener('DOMContentLoaded', () => {
  // Respect server-rendered preference to disable animations for some roles
  const disableAnimations = document.body?.getAttribute('data-disable-animations') === 'true';
  if (disableAnimations) return;

  const overlay = document.getElementById('book-overlay');
  const book = document.querySelector('.book');
  const $menu = document.getElementById('nav-menu');

  // Menu reveal using hardware-accelerated CSS transition delays
  function animateMenu() {
    if (!$menu) return;
    const segments = $menu.querySelectorAll('.menu-item');

    segments.forEach((item, index) => {
      // Set uniform premium transition rules
      item.style.transition = 'opacity 0.5s cubic-bezier(0.16, 1, 0.3, 1), transform 0.5s cubic-bezier(0.16, 1, 0.3, 1)';
      // Stagger them with clean delays (45ms increments for responsive fluid stagger)
      item.style.transitionDelay = `${index * 45}ms`;
      
      // Clean up layout offsets to prevent horizontal misalignment or wrapping
      item.style.marginRight = '';
      
      // Apply final target states in the next frame to trigger the transition beautifully
      requestAnimationFrame(() => {
        item.style.opacity = '1';
        item.style.transform = 'translateY(0)';
      });
    });
  }

  // Book loading overlay logic
  if (book && overlay) {
    let dismissTimer = null;
    let isDismissed = false;

    function dismissOverlay() {
      if (isDismissed) return;
      isDismissed = true;
      if (dismissTimer) clearTimeout(dismissTimer);
      
      overlay.classList.add('fade-out');
      animateMenu();
      
      setTimeout(() => {
        try { overlay.remove(); } catch(e) { overlay.style.display = 'none'; }
      }, 600);
    }

    // Wait slightly to ensure layouts are stable
    setTimeout(() => {
      book.classList.add('open');
      
      // Auto-dismiss after 7.5 seconds (gives ample reading time), or let them dismiss manually
      dismissTimer = setTimeout(dismissOverlay, 7500);
    }, 150);

    // Click anywhere on background to dismiss overlay instantly
    overlay.addEventListener('click', () => {
      dismissOverlay();
    });

    // Prevent dismiss when clicking inside the book itself, unless clicking the action button
    book.addEventListener('click', (e) => {
      e.stopPropagation();
    });

    // Action button dismissal
    document.getElementById('close-overlay-btn')?.addEventListener('click', (e) => {
      e.stopPropagation();
      dismissOverlay();
    });
  } else {
    // Stagger reveal immediately if overlay isn't present
    animateMenu();
  }

  // Tab switching helper
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
    btn.addEventListener('click', e => {
      const target = document.querySelector(btn.dataset.target);
      const current = document.querySelector('.tab-content.active');
      if (target && target !== current) switchTab(current, target);
    });
  });



  // Menu trigger manual animate backup
  document.getElementById('menu-trigger')?.addEventListener('click', () => {
    animateMenu();
  });
});
