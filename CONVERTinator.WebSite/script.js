/* =========================================================
CONVERTinator — SPA Engine
Clean, Modular, and Enterprise-ready Vanilla JS
========================================================= */

/* ---------------------------------------------------------
MODULE 1: CONFIGURATION (Mapping & Regions)
--------------------------------------------------------- */
const RegionBackgrounds = {
    'Europe': 'bg-europe.svg',
    'CIS': '10-full-background.svg',
    'Americas': 'bg-americas.svg',
    'Asia': 'bg-asia.svg',
    'MiddleEast': 'bg-middleeast.svg',
    'Oceania': 'bg-oceania.svg',
    'Global': '10-full-background.svg'
};

const CountryToRegionMap = {
    'PL': 'Europe', 'DE': 'Europe', 'UA': 'Europe', 'RO': 'Europe', 'RS': 'Europe',
    'HR': 'Europe', 'SI': 'Europe', 'BG': 'Europe', 'GR': 'Europe', 'CY': 'Europe',
    'IS': 'Europe', 'GB': 'Europe', 'IE': 'Europe', 'FR': 'Europe', 'IT': 'Europe',
    'ES': 'Europe', 'PT': 'Europe', 'NL': 'Europe', 'BE': 'Europe', 'CH': 'Europe',
    'AT': 'Europe', 'CZ': 'Europe', 'SK': 'Europe', 'HU': 'Europe', 'NO': 'Europe',
    'SE': 'Europe', 'FI': 'Europe', 'DK': 'Europe', 'EE': 'Europe', 'LV': 'Europe', 'LT': 'Europe',
    'BY': 'CIS', 'RU': 'CIS', 'KZ': 'CIS', 'UZ': 'CIS', 'GE': 'CIS', 
    'AM': 'CIS', 'AZ': 'CIS', 'MD': 'CIS', 'KG': 'CIS', 'TJ': 'CIS',
    'US': 'Americas', 'CA': 'Americas', 'MX': 'Americas',
    'CN': 'Asia', 'JP': 'Asia', 'KR': 'Asia', 'TR': 'Asia',
    'SA': 'MiddleEast', 'AE': 'MiddleEast', 'IL': 'MiddleEast',
    'AU': 'Oceania', 'NZ': 'Oceania'
};

/* ---------------------------------------------------------
MODULE 2: CURRENCY → COUNTRY FLAG MAPPING (Fully Fixed!)
--------------------------------------------------------- */
const CurrencyToCountryFlag = {
    'USD': '🇺🇸', 'EUR': '🇪🇺', 'RUB': '🇷🇺', 'GBP': '🇬🇧',
    'JPY': '🇯🇵', 'CNY': '🇨🇳', 'AED': '🇦🇪', 'AFN': '🇦🇫',
    'ALL': '🇦🇱', 'AMD': '🇦🇲', 'AOA': '🇦🇴', 'ARS': '🇦🇷',
    'AUD': '🇦🇺', 'AWG': '🇦🇼', 'AZN': '🇦🇿', 'BAM': '🇧🇦',
    'BBD': '🇧🇧', 'BDT': '🇧🇩', 'BHD': '🇧🇭', 'BIF': '🇧🇮',
    'BMD': '🇧🇲', 'BND': '🇧🇳', 'BOB': '🇧🇴', 'BRL': '🇧🇷',
    'BSD': '🇧🇸', 'BTN': '🇧🇹', 'BWP': '🇧🇼', 'BYN': '🇧🇾',
    'BZD': '🇧🇿', 'CAD': '🇨🇦', 'CDF': '🇨🇩', 'CHF': '🇨🇭',
    'CLP': '🇨🇱', 'COP': '🇨🇴', 'CRC': '🇨🇷', 'CUP': '🇨🇺',
    'CVE': '🇨🇻', 'CZK': '🇨🇿', 'DJF': '🇩🇯', 'DKK': '🇩🇰',
    'DOP': '🇩🇴', 'DZD': '🇩🇿', 'EGP': '🇪🇬', 'ERN': '🇪🇷',
    'ETB': '🇪🇹', 'FJD': '🇫🇯', 'FKP': '🇫🇰', 'GEL': '🇬🇪',
    'GHS': '🇬🇭', 'GIP': '🇬🇮', 'GMD': '🇬🇲', 'GNF': '🇬🇳',
    'GTQ': '🇬🇹', 'GYD': '🇬🇾', 'HKD': '🇭🇰', 'HNL': '🇭🇳',
    'HTG': '🇭🇹', 'HUF': '🇭🇺', 'IDR': '🇮🇩', 'ILS': '🇮🇱',
    'INR': '🇮🇳', 'IQD': '🇮🇶', 'IRR': '🇮🇷', 'ISK': '🇮🇸',
    'JMD': '🇯🇲', 'JOD': '🇯🇴', 'KES': '🇰🇪', 'KGS': '🇰🇬',
    'KHR': '🇰🇭', 'KMF': '🇰🇲', 'KPW': '🇰🇵', 'KRW': '🇰🇷',
    'KWD': '🇰🇼', 'KYD': '🇰🇾', 'KZT': '🇰🇿', 'LAK': '🇱🇦',
    'LBP': '🇱🇧', 'LKR': '🇱🇰', 'LRD': '🇱🇷', 'LSL': '🇱🇸',
    'LYD': '🇱🇾', 'MAD': '🇲🇦', 'MDL': '🇲🇩', 'MGA': '🇲🇬',
    'MKD': '🇲🇰', 'MMK': '🇲🇲', 'MNT': '🇲🇳', 'MOP': '🇲🇴',
    'MRU': '🇲🇷', 'MUR': '🇲🇺', 'MVR': '🇲🇻', 'MWK': '🇲🇼',
    'MXN': '🇲🇽', 'MYR': '🇲🇾', 'MZN': '🇲🇿', 'NAD': '🇳🇦',
    'NGN': '🇳🇬', 'NIO': '🇳🇮', 'NOK': '🇳🇴', 'NPR': '🇳🇵',
    'NZD': '🇳🇿', 'OMR': '🇴🇲', 'PAB': '🇵🇦', 'PEN': '🇵🇪',
    'PGK': '🇵🇬', 'PHP': '🇵🇭', 'PKR': '🇵🇰', 'PLN': '🇵🇱',
    'PYG': '🇵🇾', 'QAR': '🇶🇦', 'RON': '🇷🇴', 'RSD': '🇷🇸',
    'RWF': '🇷🇼', 'SAR': '🇸🇦', 'SBD': '🇸🇧', 'SCR': '🇸🇨',
    'SDG': '🇸🇩', 'SEK': '🇸🇪', 'SGD': '🇸🇬', 'SHP': '🇸🇭',
    'SLL': '🇸🇱', 'SOS': '🇸🇴', 'SRD': '🇸🇷', 'SSP': '🇸🇸',
    'STN': '🇸🇹', 'SVC': '🇸🇻', 'SYP': '🇸🇾', 'SZL': '🇸🇿',
    'THB': '🇹🇭', 'TJS': '🇹🇯', 'TMT': '🇹🇲', 'TND': '🇹🇳',
    'TOP': '🇹🇴', 'TRY': '🇹🇷', 'TTD': '🇹🇹', 'TWD': '🇹🇼',
    'TZS': '🇹🇿', 'UAH': '🇺🇦', 'UGX': '🇺🇬', 'UYU': '🇺🇾',
    'UZS': '🇺🇿', 'VES': '🇻🇪', 'VND': '🇻🇳', 'VUV': '🇻🇺',
    'WST': '🇼🇸', 'XAF': '🇨🇲', 'XCD': '🇦🇬', 'XOF': '🇸🇳',
    'XPF': '🇵🇫', 'YER': '🇾🇪', 'ZAR': '🇿🇦', 'ZMW': '🇿🇲',
    'ZWG': '🇿🇼'
};

const ALL_CURRENCY_CODES = Object.keys(CurrencyToCountryFlag);
const OPTIONS_HTML = ALL_CURRENCY_CODES
    .map(code => `<option value="${code}">${CurrencyToCountryFlag[code]} ${code}</option>`)
    .join('');

function populateInitialSelects() {
    // Base Currency
    const baseSelect = document.getElementById('baseCurrency');
    if (baseSelect) {
        baseSelect.innerHTML = OPTIONS_HTML;
        baseSelect.value = 'UYU'; // Default
    }

    const targetSelects = document.querySelectorAll('.target-select');
    targetSelects.forEach((select, index) => {
        select.innerHTML = OPTIONS_HTML;
        select.value = index === 0 ? 'USD' : 'EUR'; 
    });
}

/* ---------------------------------------------------------
MODULE 3: EMOJI ROTATION (Money icons & Flags)
--------------------------------------------------------- */
const moneyEmojis = ['💵', '💶', '💷', '💴', '💳'];
let moneyIndex = 0;
let moneyInterval = null;

const flagEmojis = Object.values(CurrencyToCountryFlag);
let flagIndex = 0;
let flagInterval = null;

function startMoneyRotation(iconEl) {
    if (moneyInterval) clearInterval(moneyInterval);
    moneyInterval = setInterval(() => {
        moneyIndex = (moneyIndex + 1) % moneyEmojis.length;
        iconEl.textContent = moneyEmojis[moneyIndex];
    }, 400);
}

function stopMoneyRotation(iconEl) {
    if (moneyInterval) {
        clearInterval(moneyInterval);
        moneyInterval = null;
    }
}

function startFlagRotation(iconEl) {
    if (flagInterval) clearInterval(flagInterval);
    flagInterval = setInterval(() => {
        flagIndex = (flagIndex + 1) % flagEmojis.length;
        iconEl.textContent = flagEmojis[flagIndex];
    }, 350);
}

function stopFlagRotation(iconEl) {
    if (flagInterval) {
        clearInterval(flagInterval);
        flagInterval = null;
    }
}

/* ---------------------------------------------------------
MODULE 4: CORE LOGIC & API (Auto-Detection & Manual)
--------------------------------------------------------- */
async function initApp() {
    let iso = 'US';
    let region = 'Americas'; 
    let baseCurrency = 'USD';

    try {
        // Fetch the location data directly from the self-contained C# controller
        // You can test other regions locally by appending a query string, e.g., ?overrideIso=NO
        const response = await fetch('https://localhost:7256/api/Location/current', {
            method: 'GET',
            headers: { 'Accept': 'application/json' },
        });

        if (response.ok) {
            const data = await response.json();
            if (data.isoCode) {
                iso = data.isoCode.toUpperCase();
                region = data.region;
                baseCurrency = data.currencyCode; 
            }
        }
    } catch (error) {
        console.warn('Backend is offline. Using fallback environment profiles (RU/CIS).', error);
    }

    // Initialize application state with the resolved parameters
    applyRegionSettings(iso, region, baseCurrency);
}

function applyRegionSettings(isoCode, regionName, currencyCode) {
    const regionSelector = document.getElementById('regionSelector');
    if (regionSelector) {
        regionSelector.value = regionName;
    }

    // Enforce the detected primary country currency as the base asset
    const baseCurrencySelect = document.getElementById('baseCurrency');
    if (baseCurrencySelect && currencyCode) {
        baseCurrencySelect.value = currencyCode;
    }

    const bgImageFile = RegionBackgrounds[regionName] || RegionBackgrounds['Global'];
    const bgElement = document.getElementById('bg-image');
    if (bgElement) {
        bgElement.style.opacity = 0.5;
        setTimeout(() => {
            bgElement.src = bgImageFile;
            bgElement.style.opacity = 1;
        }, 200);
    }

    // Update the URL hash routing signature silently
    window.location.hash = `#${regionName}`;
    console.log(`[Smart Init] IP mapped -> ISO: ${isoCode} | Region: ${regionName} | Base: ${currencyCode}`);

    if (typeof updateTargetOptions === 'function') updateTargetOptions();
    if (typeof updateConversions === 'function') updateConversions();
    if (typeof updateChartToggles === 'function') updateChartToggles();
    if (typeof renderCurrencyChart === 'function') renderCurrencyChart();
}

// Global DOM listener for interactive manual region overrides from the header select
document.addEventListener('DOMContentLoaded', () => {
    const regionSelector = document.getElementById('regionSelector');
    if (regionSelector) {
        regionSelector.addEventListener('change', (e) => {
            const selectedRegion = e.target.value;
            const bgImageFile = RegionBackgrounds[selectedRegion] || RegionBackgrounds['Global'];
            
            const bgElement = document.getElementById('bg-image');
            if (bgElement) {
                bgElement.style.opacity = 0.5;
                setTimeout(() => {
                    bgElement.src = bgImageFile;
                    bgElement.style.opacity = 1;
                }, 200);
            }
            
            window.location.hash = `#${selectedRegion}`;
            console.log(`[Manual Override] User switched region to: ${selectedRegion} | Background: ${bgImageFile}`);
        });
    }
});

/* ---------------------------------------------------------
MODULE 5: REAL API CONVERSION (Optimized Single Request)
--------------------------------------------------------- */

// Debounce function to protect your C# server from spam
function debounce(func, wait) {
    let timeout;
    return function(...args) {
        clearTimeout(timeout);
        timeout = setTimeout(() => func.apply(this, args), wait);
    };
}

// The main fetching engine
async function performRealConversion() {
    const amountInput = document.getElementById('amount');
    const baseCurrencySelect = document.getElementById('baseCurrency');
    
    if (!amountInput || !baseCurrencySelect) return;
    
    const amount = parseFloat(amountInput.value) || 0;
    const baseCurrency = baseCurrencySelect.value;
    const targetRows = document.querySelectorAll('.target-row');
    const targetCurrencies = Array.from(targetRows).map(row => {
        const select = row.querySelector('.target-select');
        return select ? select.value : null;
    }).filter(val => val !== null);

    if (targetCurrencies.length === 0) return;

    try {
        // Build the URL for a SINGLE request containing all targets
        // IMPORTANT: Adjust 'baseCur', 'amount', and 'targetCur' parameter names 
        // if they differ in C# controller!
        const url = new URL('https://localhost:7256/api/Convert/exchange');
        url.searchParams.append('baseCur', baseCurrency);
        url.searchParams.append('amount', amount);
        
        // Append each target currency. ASP.NET will automatically parse this into a List<string>
        targetCurrencies.forEach(cur => url.searchParams.append('targetCur', cur));

        const response = await fetch(url.toString(), {
            method: 'GET',
            headers: { 'Accept': 'application/json' }
        });

        if (!response.ok) throw new Error(`HTTP Error: ${response.status}`);

        const data = await response.json();

        // Map the backend results directly to the UI rows
        if (data.status === "success" && data.conversions) {
            targetRows.forEach(row => {
                const select = row.querySelector('.target-select');
                const resultInput = row.querySelector('.target-result');
                if (!select || !resultInput) return;

                // Find the specific currency in the backend response array
                const conversion = data.conversions.find(c => c.targetCurrency === select.value);

                if (conversion && conversion.success) {
                    resultInput.value = conversion.convertedAmount.toLocaleString('us-US', {
                        minimumFractionDigits: 3,
                        maximumFractionDigits: 3
                    }) + ' ' + select.value;
                } else {
                    resultInput.value = "BAD API";
                }
            });
        }

    } catch (err) {
        console.warn(`[API Error] Failed to fetch rates:`, err);
        // Optional: you can add the local stub fallback here if the server drops
    }

    if (typeof updateChartToggles === 'function') updateChartToggles();
    if (typeof renderCurrencyChart === 'function') renderCurrencyChart();
}

const updateConversions = debounce(performRealConversion, 300);

/* ---------------------------------------------------------
FILTER: Hide base currency from target selects
--------------------------------------------------------- */
function updateTargetOptions() {
    if (!baseCurrencySelect) return;
    const baseValue = baseCurrencySelect.value;

    document.querySelectorAll('.target-select').forEach(select => {
        const currentValue = select.value;

        Array.from(select.options).forEach(option => {
            if (option.value === baseValue) {
                option.hidden = true;
                option.disabled = true;
            } else {
                option.hidden = false;
                option.disabled = false;
            }
        });

        if (currentValue === baseValue) {
            const firstAvailable = Array.from(select.options).find(opt => opt.value !== baseValue);
            if (firstAvailable) {
                select.value = firstAvailable.value;
            }
        }
    });
}

/* ---------------------------------------------------------
GET RANDOM CURRENCY (excluding base and already selected)
--------------------------------------------------------- */
function getRandomCurrency() {
    const baseValue = baseCurrencySelect ? baseCurrencySelect.value : null;
    const selectedValues = new Set();
    
    if (baseValue) selectedValues.add(baseValue);
    
    document.querySelectorAll('.target-select').forEach(select => {
        if (select.value) selectedValues.add(select.value);
    });

    const available = ALL_CURRENCY_CODES.filter(code => !selectedValues.has(code));
    
    if (available.length === 0) return null;
    
    const randomIndex = Math.floor(Math.random() * available.length);
    return available[randomIndex];
}

/* ---------------------------------------------------------
LOGIC: ADD BUTTON (+)
--------------------------------------------------------- */
if (addCurrencyBtn) {
    addCurrencyBtn.addEventListener('click', () => {
        if (currentCurrencyCount >= MAX_CURRENCIES) return;

        const allOptionsHtml = OPTIONS_HTML;
        const randomCurrency = getRandomCurrency();

        const newRow = document.createElement('div');
        newRow.className = 'row target-row';

        newRow.innerHTML = `
            <select class="glass select target-select">
                ${allOptionsHtml}
            </select>
            <input type="text" class="glass input result-input target-result" value="0.0000" readonly>
        `;

        targetRowsContainer.appendChild(newRow);

        const newSelect = newRow.querySelector('.target-select');
        if (randomCurrency) newSelect.value = randomCurrency;

        newSelect.addEventListener('change', () => {
            updateTargetOptions();
            updateConversions();
            
            // Sync with Graph Module
            if (typeof updateChartToggles === 'function') updateChartToggles();
            if (typeof renderCurrencyChart === 'function') renderCurrencyChart();
        });

        currentCurrencyCount++;
        if (currencyCountSpan) currencyCountSpan.textContent = currentCurrencyCount;

        // Hide (+) button if limit reached
        if (currentCurrencyCount >= MAX_CURRENCIES) addCurrencyBtn.style.display = 'none';
        
        // Show (-) button because we have more than 2 currencies
        if (currentCurrencyCount > 2 && removeCurrencyBtn) removeCurrencyBtn.style.display = 'flex';

        updateTargetOptions();
        updateConversions();
        
        // Sync with Graph Module (Stretch canvas)
        if (typeof updateChartToggles === 'function') updateChartToggles();
        if (typeof adjustGraphHeight === 'function') adjustGraphHeight();
    });
}

/* ---------------------------------------------------------
LOGIC: REMOVE BUTTON (-)
--------------------------------------------------------- */
if (removeCurrencyBtn) {
    removeCurrencyBtn.addEventListener('click', () => {
        if (currentCurrencyCount <= 2) return; // Cannot have less than 2

        const targetRows = document.querySelectorAll('.target-row');
        const lastRow = targetRows[targetRows.length - 1];
        
        // Remove currency from graph if it was active
        const selectToRemove = lastRow.querySelector('.target-select');
        if (selectToRemove && typeof activeChartCurrencies !== 'undefined' && activeChartCurrencies.has(selectToRemove.value)) {
            activeChartCurrencies.delete(selectToRemove.value);
        }

        lastRow.remove();
        currentCurrencyCount--;
        if (currencyCountSpan) currencyCountSpan.textContent = currentCurrencyCount;

        // Show (+) button again
        addCurrencyBtn.style.display = 'flex';
        
        // Hide (-) button if only 2 left
        if (currentCurrencyCount <= 2) removeCurrencyBtn.style.display = 'none';

        updateTargetOptions();
        updateConversions();
        
        // Sync with Graph Module (Shrink canvas and re-render)
        if (typeof updateChartToggles === 'function') updateChartToggles();
        if (typeof adjustGraphHeight === 'function') adjustGraphHeight();
        if (typeof renderCurrencyChart === 'function') renderCurrencyChart();
    });
}

/* ---------------------------------------------------------
CONVERSION MATH — 4 decimal places precision
--------------------------------------------------------- */
function updateConversions() {
    if (!amountInput || !baseCurrencySelect) return;
    const amount = parseFloat(amountInput.value) || 0;
    const baseCurrency = baseCurrencySelect.value;

    const baseRate = STUB_RATES[baseCurrency] || 1;
    const inUsd = amount / baseRate;

    const targetRows = document.querySelectorAll('.target-row');

    targetRows.forEach(row => {
        const targetSelect = row.querySelector('.target-select');
        const resultInput = row.querySelector('.target-result');

        if (!targetSelect || !resultInput) return;

        const targetCurrency = targetSelect.value;
        const targetRate = STUB_RATES[targetCurrency] || 1;

        const result = inUsd * targetRate;
        const decimals = 4; // Strict precision to 4 decimal places

        resultInput.value = result.toLocaleString('ru-RU', {
            minimumFractionDigits: decimals,
            maximumFractionDigits: decimals
        }) + ' ' + targetCurrency;
    });

    // Sync with Graph Module on value changes
    if (typeof updateChartToggles === 'function') updateChartToggles();
    if (typeof renderCurrencyChart === 'function') renderCurrencyChart();
}

/* ---------------------------------------------------------
INITIAL EVENT LISTENERS
--------------------------------------------------------- */
if (amountInput) amountInput.addEventListener('input', updateConversions);

if (baseCurrencySelect) {
    baseCurrencySelect.addEventListener('change', () => {
        updateTargetOptions();
        updateConversions();
    });
}

document.querySelectorAll('.target-select').forEach(select => {
    select.addEventListener('change', () => {
        updateTargetOptions();
        updateConversions();
    });
});

/* ---------------------------------------------------------
MODULE 6: EMOJI ROTATION (Flags Auto, Money on Hover)
--------------------------------------------------------- */
document.addEventListener('DOMContentLoaded', () => {
    const flagIcon = document.getElementById('flagIcon');
    const moneyIcon = document.getElementById('moneyIcon');
    const moneyLi = document.querySelector('[data-emoji-rotate="money"]'); 
    
    const moneyEmojis = ['💵', '💶', '💷', '💴', '💳'];

    // Flags
    if (flagIcon) {
        setInterval(() => {
            const randomCode = ALL_CURRENCY_CODES[Math.floor(Math.random() * ALL_CURRENCY_CODES.length)];
            flagIcon.textContent = CurrencyToCountryFlag[randomCode];
        }, 800); 
    }

    // Money
    if (moneyIcon && moneyLi) {
        let mIndex = 0;
        let moneyInterval = null;
        moneyLi.addEventListener('mouseenter', () => {
            moneyInterval = setInterval(() => {
                mIndex = (mIndex + 1) % moneyEmojis.length;
                moneyIcon.textContent = moneyEmojis[mIndex];
            }, 350); 
        });

        moneyLi.addEventListener('mouseleave', () => {
            clearInterval(moneyInterval);
            moneyInterval = null;
        });
    }
});

/* ---------------------------------------------------------
MODULE 7: VISUAL EFFECTS (Theme & Parallax)
--------------------------------------------------------- */
const moonBtn = document.querySelector('.moon-btn');
if (moonBtn) {
    moonBtn.addEventListener('click', () => {
        const html = document.documentElement;
        const isDark = html.getAttribute('data-theme') === 'dark';
        html.setAttribute('data-theme', isDark ? 'light' : 'dark');
        moonBtn.style.transform = isDark ? 'rotate(0deg)' : 'rotate(180deg)';
    });
}

const parallaxBg = document.getElementById('bg-image');
let isScrolling = false;
window.addEventListener('scroll', () => {
    if (!isScrolling && parallaxBg) {
        window.requestAnimationFrame(() => {
            const offset = window.scrollY * 0.3;
            parallaxBg.style.transform = `translateY(-${offset}px)`;
            isScrolling = false;
        });
        isScrolling = true;
    }
});

/* =========================================================
   MODULE 8: LIGHTWEIGHT CANVAS GRAPH ENGINE (TradingView Style)
   ========================================================= */
let activeChartCurrencies = new Set(['base']); // 'base' means baseline (e.g. RUB -> USD)
let currentRange = '1M';
const chartColors = {
    base: '#9245e5', // Purple for baseline
    line: '#4db892', // Green for active targets
    grid: 'rgba(255, 255, 255, 0.1)'
};

// Generates pseudo-random historical wave data for charts based on currency seed
function generateHistoryData(seed, pointsCount = 30) {
    let data = [];
    let hash = 0;
    for (let i = 0; i < seed.length; i++) {
        hash = seed.charCodeAt(i) + ((hash << 5) - hash);
    }
    let baseline = 0.5 + (Math.abs(hash % 100) / 200);
    
    for (let i = 0; i < pointsCount; i++) {
        let noise = Math.sin(i * 0.4 + hash) * 0.15;
        let trend = (i / pointsCount) * 0.1 * (hash % 2 === 0 ? 1 : -1);
        data.push(baseline + noise + trend);
    }
    return data;
}

// Main render function for HTML5 Canvas chart
function renderCurrencyChart() {
    const canvas = document.getElementById('currencyChart');
    if (!canvas) return;
    
    const ctx = canvas.getContext('2d');
    const container = document.getElementById('canvasContainer');
    
    // Set internal resolution matching logical display size (Fixes blurry canvas)
    canvas.width = container.clientWidth;
    canvas.height = container.clientHeight;
    
    const width = canvas.width;
    const height = canvas.height;
    
    // Clear canvas canvas
    ctx.clearRect(0, 0, width, height);
    
    // 1. Draw subtle grid lines
    ctx.strokeStyle = document.documentElement.getAttribute('data-theme') === 'dark' 
        ? 'rgba(255, 255, 255, 0.06)' 
        : 'rgba(0, 0, 0, 0.06)';
    ctx.lineWidth = 1;
    
    // Horizontal grid
    for (let i = 1; i < 4; i++) {
        let y = (height / 4) * i;
        ctx.beginPath(); ctx.moveTo(0, y); ctx.lineTo(width, y); ctx.stroke();
    }
    // Vertical grid
    for (let i = 1; i < 6; i++) {
        let x = (width / 6) * i;
        ctx.beginPath(); ctx.moveTo(x, 0); ctx.lineTo(x, height); ctx.stroke();
    }
    
    const baseCurrency = baseCurrencySelect.value;
    
    // 2. Render selected currency paths
    activeChartCurrencies.forEach(curType => {
        let points = [];
        let label = '';
        let strokeColor = chartColors.line;
        
        if (curType === 'base') {
            points = generateHistoryData(baseCurrency + "USD" + currentRange);
            label = `${baseCurrency} → USD`;
            strokeColor = chartColors.base;
        } else {
            points = generateHistoryData(baseCurrency + curType + currentRange);
            label = `${baseCurrency} → ${curType}`;
            strokeColor = chartColors.line;
        }
        
        // Find min & max for absolute scaling to bounds
        const min = Math.min(...points);
        const max = Math.max(...points);
        const range = max - min || 1;
        
        ctx.beginPath();
        ctx.lineWidth = 3;
        ctx.strokeStyle = strokeColor;
        ctx.lineJoin = 'round';
        ctx.lineCap = 'round';
        
        points.forEach((val, index) => {
            let x = (width / (points.length - 1)) * index;
            // Map values linearly to fit height canvas container safely
            let y = height - 25 - ((val - min) / range) * (height - 50);
            
            if (index === 0) ctx.moveTo(x, y);
            else ctx.lineTo(x, y);
        });
        ctx.stroke();
        
        // Draw subtle premium neon gradient under the active curve
        let gradient = ctx.createLinearGradient(0, 0, 0, height);
        gradient.addColorStop(0, strokeColor + '22'); // 13% opacity hex alpha
        gradient.addColorStop(1, strokeColor + '00'); // Invisible
        
        ctx.lineTo(width, height);
        ctx.lineTo(0, height);
        ctx.closePath();
        ctx.fillStyle = gradient;
        ctx.fill();
    });
}

// Synchronizes and updates interactive badges below the chart plane
function updateChartToggles() {
    const togglesContainer = document.getElementById('chartToggles');
    if (!togglesContainer) return;
    
    const baseCurrency = baseCurrencySelect.value;
    
    // Gather all target currencies currently available on the right side
    const currentTargets = Array.from(document.querySelectorAll('.target-select')).map(s => select.value);
    
    let html = `
        <div class="toggle-badge ${activeChartCurrencies.has('base') ? 'active' : ''}" data-cur="base">
            📈 ${baseCurrency} / USD (Базовый)
        </div>
    `;
    
    currentTargets.forEach(cur => {
        const flag = CurrencyToCountryFlag[cur] || '🏳️';
        const isActive = activeChartCurrencies.has(cur);
        html += `
            <div class="toggle-badge ${isActive ? 'active' : ''}" data-cur="${cur}">
                ${flag} ${baseCurrency} / ${cur}
            </div>
        `;
    });
    
    togglesContainer.innerHTML = html;
    
    // Attach click listeners to handle toggling multiple lines on/off
    togglesContainer.querySelectorAll('.toggle-badge').forEach(badge => {
        badge.addEventListener('click', () => {
            const cur = badge.getAttribute('data-cur');
            if (activeChartCurrencies.has(cur)) {
                if (activeChartCurrencies.size > 1) activeChartCurrencies.delete(cur);
            } else {
                activeChartCurrencies.add(cur);
            }
            updateChartToggles();
            renderCurrencyChart();
        });
    });
}

// Dynamically auto-sizes graph height container as list expands
function adjustGraphHeight() {
    const container = document.getElementById('canvasContainer');
    if (!container) return;
    
    // Standard starting height is 220px. Add 15px for every extra target select added.
    const baseHeight = 220;
    const additionalHeight = Math.max(0, currentCurrencyCount - 2) * 15;
    
    container.style.height = `${baseHeight + additionalHeight}px`;
    
    // Redraw with new boundaries
    setTimeout(renderCurrencyChart, 450); // Syncs with CSS transition delay
}

// Intercept time range buttons click
document.addEventListener('DOMContentLoaded', () => {
    const rangeContainer = document.getElementById('timeRangeControls');
    if (rangeContainer) {
        rangeContainer.querySelectorAll('.time-btn').forEach(btn => {
            btn.addEventListener('click', () => {
                rangeContainer.querySelector('.time-btn.active').classList.remove('active');
                btn.classList.add('active');
                currentRange = btn.getAttribute('data-range');
                renderCurrencyChart();
            });
        });
    }
    
    // Listen to window resizing to keep resolution sharp
    window.addEventListener('resize', renderCurrencyChart);
});

/* ---------------------------------------------------------
BOOTSTRAP
--------------------------------------------------------- */
document.addEventListener('DOMContentLoaded', () => {
    populateInitialSelects();
    initApp();
    updateTargetOptions();
    updateConversions();
});