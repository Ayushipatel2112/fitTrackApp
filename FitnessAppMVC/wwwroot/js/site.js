// Global Fitness App Script
document.addEventListener('DOMContentLoaded', () => {
  // Initialize Lucide Icons
  if (typeof lucide !== 'undefined') {
    lucide.createIcons();
  }

  // Theme Handling Logic
  const THEME_KEY = 'fitness_theme';
  const body = document.body;
  const themeToggle = document.getElementById('theme-toggle');
  const themeText = document.getElementById('theme-text');
  const moonIcon = document.getElementById('theme-icon-moon');
  const sunIcon = document.getElementById('theme-icon-sun');

  const getPreferredTheme = () => {
    const savedTheme = localStorage.getItem(THEME_KEY);
    if (savedTheme) return savedTheme;
    return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
  };

  const updateThemeUI = (theme) => {
    if (theme === 'dark') {
      body.classList.add('dark-theme');
      body.classList.remove('light-theme');
      if (moonIcon) moonIcon.style.display = 'none';
      if (sunIcon) sunIcon.style.display = 'inline-block';
      if (themeText) themeText.textContent = 'Light';
    } else {
      body.classList.add('light-theme');
      body.classList.remove('dark-theme');
      if (moonIcon) moonIcon.style.display = 'inline-block';
      if (sunIcon) sunIcon.style.display = 'none';
      if (themeText) themeText.textContent = 'Dark';
    }
    localStorage.setItem(THEME_KEY, theme);
  };

  let currentTheme = getPreferredTheme();
  updateThemeUI(currentTheme);

  if (themeToggle) {
    themeToggle.addEventListener('click', () => {
      currentTheme = currentTheme === 'dark' ? 'light' : 'dark';
      updateThemeUI(currentTheme);
    });
  }

  // Mobile Menu Toggle
  const mobileToggle = document.getElementById('mobile-toggle');
  const sideNav = document.getElementById('user-nav') || document.getElementById('sidebar');
  if (mobileToggle && sideNav) {
    mobileToggle.addEventListener('click', () => {
      sideNav.classList.toggle('open');
      sideNav.classList.toggle('mobile-open');
    });
  }

  // 3D Tilt Effect for All Cards
  const tiltCards = document.querySelectorAll('.feature-card, .journey-card, .service-card');
  tiltCards.forEach(card => {
    card.addEventListener('mousemove', (e) => {
      const rect = card.getBoundingClientRect();
      const x = e.clientX - rect.left;
      const y = e.clientY - rect.top;

      const centerX = rect.width / 2;
      const centerY = rect.height / 2;

      const rotateX = ((y - centerY) / centerY) * 10;
      const rotateY = ((centerX - x) / centerX) * 10;

      card.style.transform = `perspective(1000px) rotateX(${rotateX}deg) rotateY(${rotateY}deg) translateY(-5px)`;
    });

    card.addEventListener('mouseleave', () => {
      card.style.transform = `perspective(1000px) rotateX(0deg) rotateY(0deg) translateY(0)`;
    });
  });
});
