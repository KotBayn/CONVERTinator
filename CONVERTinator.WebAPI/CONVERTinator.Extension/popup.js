/* ==========================================================================
   CONVERTinator EXTENSION - LOGIC ENGINE
   ========================================================================== */
const API_BASE = 'https://convertinator.onrender.com';
const MAX_CURRENCIES = 10;
let currentCurrencyCount = 2;

const ALL_CURRENCIES = ['USD', 'EUR', 'RUB', 'GBP', 'JPY', 'CNY', 'AED', 'ARS', 'AUD', 'BRL', 'CAD', 'CHF', 'GEL', 'HKD', 'ILS', 'INR', 'KRW', 'KZT', 'MXN', 'PLN', 'TRY', 'UAH', 'ZAR']; // Сокращенный список самых популярных для скорости работы, можешь расширить.

const OPTIONS_HTML = ALL_CURRENCIES
    .map(code => `<option value="${code}">${code}</option>`)
    .join('');

const amountInput = document.getElementById('amount');
const baseCurrencySelect = document.getElementById('baseCurrency');
const targetRowsContainer = document.getElementById('targetRows');
const addCurrencyBtn = document.getElementById('addCurrencyBtn');
const removeCurrencyBtn = document.getElementById('removeCurrencyBtn');
const currencyCountSpan = document.getElementById('currencyCount');

/* --------------------------------------------------------------------------
   1. Initialization (chrome.storage)
   -------------------------------------------------------------------------- */
async function initApp() {
    chrome.storage.local.get(['theme', 'lang', 'baseCur', 'targetCurs', 'amount'], async (data) => {

        const isDark = data.theme !== 'light';
        document.documentElement.setAttribute('data-theme', isDark ? 'dark' : 'light');

        let lang = data.lang;
        if (!lang) lang = getSystemLanguage();
        document.getElementById('langSelector').value = lang;
        await loadLanguage(lang);

        let base = data.baseCur;
        let targets = data.targetCurs || ['USD', 'EUR'];
        
        if (!base) {
            try {
                const res = await fetch(`${API_BASE}/api/Location/current`);
                const geo = await res.json();
                base = geo.currencyCode || 'USD';
            } catch {
                base = 'USD'; // Fallback
            }
        }

        if (data.amount) amountInput.value = data.amount;

        buildCurrencyUI(base, targets);
        updateConversions();
    });
}

function saveState() {
    const targets = Array.from(document.querySelectorAll('.target-select')).map(s => s.value);
    chrome.storage.local.set({
        baseCur: baseCurrencySelect.value,
        targetCurs: targets,
        amount: amountInput.value
    });
}

/* --------------------------------------------------------------------------
   2. Internationalize
   -------------------------------------------------------------------------- */
function getSystemLanguage() {
    const browserLang = navigator.language || navigator.userLanguage;
    const shortLang = browserLang.split('-')[0].toLowerCase();
    const supportedLangs = ['en', 'ru', 'es', 'fr', 'de', 'pt', 'zh', 'ja', 'ko', 'hi', 'ar', 'tr', 'pl', 'nl', 'uk'];
    return supportedLangs.includes(shortLang) ? shortLang : 'en';
}

async function loadLanguage(langCode) {
    try {

        const response = await fetch(`${API_BASE}/assets/i18n/${langCode}.json`);
        if (!response.ok) return;
        
        const translations = await response.json();

        document.querySelectorAll('[data-i18n]').forEach(element => {
            const key = element.getAttribute('data-i18n');
            if (translations[key]) element.innerHTML = translations[key];
        });
        
        if (langCode === 'ar' || langCode === 'he') {
            document.documentElement.setAttribute('dir', 'rtl');
        } else {
            document.documentElement.removeAttribute('dir');
        }
    } catch (e) {
        console.warn('Translate error', e);
    }
}

document.getElementById('langSelector').addEventListener('change', (e) => {
    const newLang = e.target.value;
    chrome.storage.local.set({ lang: newLang });
    loadLanguage(newLang);
});

document.querySelector('.moon-btn').addEventListener('click', () => {
    const isDark = document.documentElement.getAttribute('data-theme') === 'dark';
    const newTheme = isDark ? 'light' : 'dark';
    document.documentElement.setAttribute('data-theme', newTheme);
    chrome.storage.local.set({ theme: newTheme });
});

/* --------------------------------------------------------------------------
   3. UI logic & Convertion
   -------------------------------------------------------------------------- */
function buildCurrencyUI(base, targets) {
    baseCurrencySelect.innerHTML = OPTIONS_HTML;
    baseCurrencySelect.value = base;

    targetRowsContainer.innerHTML = '';
    currentCurrencyCount = targets.length;
    
    targets.forEach(cur => addTargetRow(cur));
    updateCounters();
}

function addTargetRow(currencyVal) {
    const row = document.createElement('div');
    row.className = 'row target-row';
    row.innerHTML = `
        <select class="glass select target-select">${OPTIONS_HTML}</select>
        <input type="text" class="glass input result-input target-result" value="0.00" readonly>
    `;
    
    targetRowsContainer.appendChild(row);
    const select = row.querySelector('.target-select');
    if (currencyVal) select.value = currencyVal;

    select.addEventListener('change', () => {
        saveState();
        updateConversions();
    });
}

function updateCounters() {
    currencyCountSpan.textContent = currentCurrencyCount;
    addCurrencyBtn.style.display = currentCurrencyCount >= MAX_CURRENCIES ? 'none' : 'flex';
    removeCurrencyBtn.style.display = currentCurrencyCount > 1 ? 'flex' : 'none';
}

addCurrencyBtn.addEventListener('click', () => {
    if (currentCurrencyCount >= MAX_CURRENCIES) return;
    addTargetRow('USD');
    currentCurrencyCount++;
    updateCounters();
    saveState();
    updateConversions();
});

removeCurrencyBtn.addEventListener('click', () => {
    if (currentCurrencyCount <= 1) return;
    targetRowsContainer.lastElementChild.remove();
    currentCurrencyCount--;
    updateCounters();
    saveState();
    updateConversions();
});

/* --------------------------------------------------------------------------
   4. RENDER (Debounce & API)
   -------------------------------------------------------------------------- */
function debounce(func, wait) {
    let timeout;
    return function(...args) { 
        clearTimeout(timeout); 
        timeout = setTimeout(() => func.apply(this, args), wait); 
    };
}

async function performRealConversion() {
    const amount = parseFloat(amountInput.value) || 0;
    const baseCur = baseCurrencySelect.value;
    const targetRows = document.querySelectorAll('.target-row');
    const targetCurs = Array.from(targetRows).map(row => row.querySelector('.target-select').value);

    if (targetCurs.length === 0) return;

    try {
        const url = new URL(`${API_BASE}/api/Convert/multi`);
        url.searchParams.append('baseCur', baseCur);
        url.searchParams.append('amount', amount);
        targetCurs.forEach(cur => url.searchParams.append('targetCurs', cur));

        const response = await fetch(url.toString());
        if (!response.ok) throw new Error('API Error');
        const data = await response.json();

        if (data.status === "success" && data.conversions) {
            targetRows.forEach(row => {
                const select = row.querySelector('.target-select');
                const resultInput = row.querySelector('.target-result');
                const conversion = data.conversions.find(c => c.targetCurrency === select.value);

                if (conversion && conversion.success) {
                    resultInput.value = conversion.convertedAmount.toLocaleString('en-US', {
                        minimumFractionDigits: 2, maximumFractionDigits: 2
                    });
                }
            });
        }
    } catch (err) {
        console.warn('API connection failed', err);
    }
}

const updateConversions = debounce(performRealConversion, 400);

amountInput.addEventListener('input', () => {
    saveState();
    updateConversions();
});
baseCurrencySelect.addEventListener('change', () => {
    saveState();
    updateConversions();
});

// START APP
document.addEventListener('DOMContentLoaded', initApp);