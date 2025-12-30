// Simple i18n Translation System
class I18n {
    constructor() {
        this.currentLang = localStorage.getItem('language') || 'en';
        this.translations = {};
    }

    async init() {
        await this.loadTranslations(this.currentLang);
        this.updateDOM();
    }

    async loadTranslations(lang) {
        try {
            const response = await fetch(`locales/${lang}.json`);
            this.translations = await response.json();
            this.currentLang = lang;
            localStorage.setItem('language', lang);

            // Update HTML attributes
            document.documentElement.setAttribute('lang', lang);
            document.documentElement.setAttribute('dir', lang === 'ar' ? 'rtl' : 'ltr');

            return true;
        } catch (error) {
            console.error('Failed to load translations:', error);
            return false;
        }
    }

    t(path) {
        const keys = path.split('.');
        let value = this.translations;

        for (const key of keys) {
            value = value?.[key];
            if (value === undefined) return path;
        }

        return value;
    }

    updateDOM() {
        // Update all [data-i18n] elements
        document.querySelectorAll('[data-i18n]').forEach(element => {
            const key = element.getAttribute('data-i18n');
            element.textContent = this.t(key);
        });

        // Update all [data-i18n-placeholder] elements
        document.querySelectorAll('[data-i18n-placeholder]').forEach(element => {
            const key = element.getAttribute('data-i18n-placeholder');
            element.placeholder = this.t(key);
        });
    }

    async switchLanguage(lang) {
        const success = await this.loadTranslations(lang);
        if (success) {
            this.updateDOM();

            // Update language switcher buttons
            document.querySelectorAll('.lang-btn').forEach(btn => {
                btn.classList.toggle('active', btn.dataset.lang === lang);
            });
        }
    }
}

// Global instance
const i18n = new I18n();

// Initialize on page load
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => i18n.init());
} else {
    i18n.init();
}

// Expose to window for easy access
window.i18n = i18n;
window.t = (key) => i18n.t(key);