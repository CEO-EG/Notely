/**
 * theme-toggle.js
 * Manages dark / light mode with localStorage persistence.
 */
(function () {
    'use strict';

    var STORAGE_KEY = 'notely-theme';
    var DARK_THEME  = 'dark';
    var LIGHT_THEME = 'light';

    /** Return the saved theme, falling back to OS preference. */
    function getSavedTheme() {
        var saved = localStorage.getItem(STORAGE_KEY);
        if (saved === DARK_THEME || saved === LIGHT_THEME) return saved;
        return window.matchMedia('(prefers-color-scheme: dark)').matches
            ? DARK_THEME
            : LIGHT_THEME;
    }

    /** Apply a theme to <html> and update every toggle button. */
    function applyTheme(theme) {
        document.documentElement.setAttribute('data-theme', theme);
        localStorage.setItem(STORAGE_KEY, theme);
        updateButtons(theme);
    }

    /** Reflect the current theme on all toggle buttons. */
    function updateButtons(theme) {
        document.querySelectorAll('.theme-toggle').forEach(function (btn) {
            var icon = btn.querySelector('.theme-icon');
            if (icon) {
                icon.className = 'theme-icon bi ' +
                    (theme === DARK_THEME ? 'bi-sun-fill' : 'bi-moon-fill');
            }
            btn.setAttribute(
                'aria-label',
                theme === DARK_THEME ? 'Switch to light mode' : 'Switch to dark mode'
            );
            btn.setAttribute('title', btn.getAttribute('aria-label'));
        });
    }

    /** Toggle between dark and light. */
    function toggle() {
        var current = document.documentElement.getAttribute('data-theme') || LIGHT_THEME;
        applyTheme(current === DARK_THEME ? LIGHT_THEME : DARK_THEME);
    }

    /* ── Init ──────────────────────────────────────────────── */
    // Apply saved/preferred theme immediately (before paint)
    applyTheme(getSavedTheme());

    // Wire up any toggle buttons already in the DOM
    document.addEventListener('DOMContentLoaded', function () {
        document.querySelectorAll('.theme-toggle').forEach(function (btn) {
            btn.addEventListener('click', toggle);
        });
        // Re-sync icons after DOM ready
        updateButtons(getSavedTheme());
    });

    // Listen for OS-level preference changes
    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', function (e) {
        // Only auto-switch if the user hasn't set a manual preference
        if (!localStorage.getItem(STORAGE_KEY)) {
            applyTheme(e.matches ? DARK_THEME : LIGHT_THEME);
        }
    });
})();
