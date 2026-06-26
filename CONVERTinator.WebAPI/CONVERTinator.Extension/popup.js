/* ==========================================================================
   CONVERTinator EXTENSION - LOGIC ENGINE
   ========================================================================== */
const API_BASE = 'https://convertinator.onrender.com';
const MAX_CURRENCIES = 10;
let currentCurrencyCount = 2;

const CurrencyToCountryFlag = {
    'USD': '🇺🇸', 'EUR': '🇪🇺', 'RUB': '🇷🇺', 'GBP': '🇬🇧', 'JPY': '🇯🇵', 'CNY': '🇨🇳', 
    'AED': '🇦🇪', 'AFN': '🇦🇫', 'ALL': '🇦🇱', 'AMD': '🇦🇲', 'AOA': '🇦🇴', 'ARS': '🇦🇷',
    'AUD': '🇦🇺', 'AWG': '🇦🇼', 'AZN': '🇦🇿', 'BAM': '🇧🇦', 'BBD': '🇧🇧', 'BDT': '🇧🇩', 
    'BHD': '🇧🇭', 'BIF': '🇧🇮', 'BMD': '🇧🇲', 'BND': '🇧🇳', 'BOB': '🇧🇴', 'BRL': '🇧🇷',
    'BSD': '🇧🇸', 'BTN': '🇧🇹', 'BWP': '🇧🇼', 'BYN': '🇧🇾', 'BZD': '🇧🇿', 'CAD': '🇨🇦', 
    'CHF': '🇨🇭', 'CLP': '🇨🇱', 'COP': '🇨🇴', 'CRC': '🇨🇷', 'CUP': '🇨🇺', 'CZK': '🇨🇿', 
    'DKK': '🇩🇰', 'DOP': '🇩🇴', 'DZD': '🇩🇿', 'EGP': '🇪🇬', 'GEL': '🇬🇪', 'HKD': '🇭🇰', 
    'HUF': '🇭🇺', 'IDR': '🇮🇩', 'ILS': '🇮🇱', 'INR': '🇮🇳', 'IQD': '🇮🇶', 'IRR': '🇮🇷', 
    'ISK': '🇮🇸', 'JMD': '🇯🇲', 'JOD': '🇯🇴', 'KES': '🇰🇪', 'KGS': '🇰🇬', 'KHR': '🇰🇭', 
    'KRW': '🇰🇷', 'KWD': '🇰🇼', 'KZT': '🇰🇿', 'LBP': '🇱🇧', 'LKR': '🇱🇰', 'MAD': '🇲🇦', 
    'MDL': '🇲🇩', 'MNT': '🇲🇳', 'MXN': '🇲🇽', 'MYR': '🇲🇾', 'NOK': '🇳🇴', 'NZD': '🇳🇿', 
    'OMR': '🇴🇲', 'PEN': '🇵🇪', 'PHP': '🇵🇭', 'PKR': '🇵🇰', 'PLN': '🇵🇱', 'QAR': '🇶🇦', 
    'RON': '🇷🇴', 'RSD': '🇷🇸', 'SAR': '🇸🇦', 'SEK': '🇸🇪', 'SGD': '🇸🇬', 'SYP': '🇸🇾', 
    'THB': '🇹🇭', 'TJS': '🇹🇯', 'TMT': '🇹🇲', 'TND': '🇹🇳', 'TRY': '🇹🇷', 'UAH': '🇺🇦', 
    'UZS': '🇺🇿', 'VND': '🇻🇳', 'ZAR': '🇿🇦'
};

const ALL_CURRENCY_CODES = Object.keys(CurrencyToCountryFlag);
const OPTIONS_HTML = ALL_CURRENCY_CODES
    .map(code => `<option value="${code}">${CurrencyToCountryFlag[code]} ${code}</option>`)
    .join('');

const amountInput = document.getElementById('amount');
const baseCurrencySelect = document.getElementById('baseCurrency');
const targetRowsContainer = document.getElementById('targetRows');
const addCurrencyBtn = document.getElementById('addCurrencyBtn');
const removeCurrencyBtn = document.getElementById('removeCurrencyBtn');
const currencyCountSpan = document.getElementById('currencyCount');

/* --------------------------------------------------------------------------
   1. INITIALIZATION & STATE MANAGEMENT (chrome.storage)
   -------------------------------------------------------------------------- */
async function initApp() {
    chrome.storage.local.get(['theme', 'lang', 'baseCur', 'targetCurs', 'amount'], async (data) => {

        const isDark = data.theme !== 'light';
        document.documentElement.setAttribute('data-theme', isDark ? 'dark' : 'light');
        let lang = data.lang || getSystemLanguage();
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
                base = 'USD'; 
            }
        }

        if (data.amount) amountInput.value = data.amount;

        buildCurrencyUI(base, targets);
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
   2. INTERNATIONALIZATION ENGINE (Remote Proxy Fetch)
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
        console.warn('Localization loading failed', e);
    }
}

document.getElementById('langSelector').addEventListener('change', (e) => {
    const newLang = e.target.value;
    chrome.storage.local.set({ lang: newLang });
    loadLanguage(newLang);
});

/* --------------------------------------------------------------------------
   3. DYNAMIC UI & EXCLUSION FILTER LOGIC
   -------------------------------------------------------------------------- */
function updateTargetOptions() {
    const targetSelects = Array.from(document.querySelectorAll('.target-select'));
    if (!baseCurrencySelect) return;
    const baseValue = baseCurrencySelect.value;

    Array.from(baseCurrencySelect.options).forEach(option => {
        option.hidden = false; option.disabled = false;
    });

    targetSelects.forEach(select => {
        if (select.value === baseValue) {
            const used = new Set([baseValue, ...targetSelects.map(s => s.value)]);
            const available = ALL_CURRENCY_CODES.find(code => !used.has(code));
            if (available) select.value = available;
        }
    });

    const targetValues = targetSelects.map(s => s.value);
    targetSelects.forEach(select => {
        const currentValue = select.value;
        Array.from(select.options).forEach(option => {
            const isBase = option.value === baseValue;
            const isUsedInOtherTarget = targetValues.includes(option.value) && option.value !== currentValue;
            const shouldHide = isBase || isUsedInOtherTarget;
            option.hidden = shouldHide;
            option.disabled = shouldHide;
        });
    });
}

function getRandomCurrency() {
    const allSelects = [baseCurrencySelect, ...document.querySelectorAll('.target-select')].filter(el => el !== null);
    const selectedValues = new Set(allSelects.map(s => s.value));
    const available = ALL_CURRENCY_CODES.filter(code => !selectedValues.has(code));
    return available.length > 0 ? available[Math.floor(Math.random() * available.length)] : null;
}

function buildCurrencyUI(base, targets) {
    baseCurrencySelect.innerHTML = OPTIONS_HTML;
    baseCurrencySelect.value = base;

    targetRowsContainer.innerHTML = '';
    currentCurrencyCount = 0;
    
    const uniqueTargets = [...new Set(targets)].filter(t => t !== base);
    const finalTargets = uniqueTargets.length > 0 ? uniqueTargets : (base !== 'USD' ? ['USD'] : ['EUR']);

    finalTargets.forEach(cur => addTargetRow(cur));
    updateCounters();
    updateTargetOptions();
    updateConversions();
}

function addTargetRow(currencyVal) {
    const row = document.createElement('div');
    row.className = 'row target-row';
    row.innerHTML = `
        <select class="glass select target-select">${OPTIONS_HTML}</select>
        <input type="text" class="glass input result-input target-result" value="0.00" readonly>
    `;
    
    targetRowsContainer.appendChild(row);
    currentCurrencyCount++;
    
    const select = row.querySelector('.target-select');
    if (currencyVal) select.value = currencyVal;

    select.addEventListener('change', () => {
        updateTargetOptions();
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
    
    const randomCur = getRandomCurrency();
    addTargetRow(randomCur);
    
    updateCounters();
    updateTargetOptions();
    saveState();
    updateConversions();
});

removeCurrencyBtn.addEventListener('click', () => {
    if (currentCurrencyCount <= 1) return;
    targetRowsContainer.lastElementChild.remove();
    currentCurrencyCount--;
    updateCounters();
    updateTargetOptions();
    saveState();
    updateConversions();
});

baseCurrencySelect.addEventListener('change', () => {
    updateTargetOptions();
    saveState();
    updateConversions();
});

/* --------------------------------------------------------------------------
   4. DISTRIBUTED CALCULATION SERVICE (API FETCH)
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
        if (!response.ok) throw new Error('API Execution Error');
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
        console.warn('Production API network channel is currently unavailable', err);
    }
}

const updateConversions = debounce(performRealConversion, 400);

amountInput.addEventListener('input', () => {
    saveState();
    updateConversions();
});

// Theme Toggle Switcher
const moonBtn = document.querySelector('.moon-btn');
if (moonBtn) {
    moonBtn.addEventListener('click', () => {
        const html = document.documentElement;
        const isDark = html.getAttribute('data-theme') === 'dark';
        const newTheme = isDark ? 'light' : 'dark';
        html.setAttribute('data-theme', newTheme);
        chrome.storage.local.set({ theme: newTheme });
    });
}

// Bootstrap application scope execution
document.addEventListener('DOMContentLoaded', initApp);
