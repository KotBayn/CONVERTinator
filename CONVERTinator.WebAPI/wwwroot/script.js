/* ==========================================================================
   CONVERTinator — SPA Engine 
   Architecture: Modular Vanilla JS with Canvas API
   ========================================================================== */

/* --------------------------------------------------------------------------
   MODULE 1: CONFIGURATION & CONSTANTS
   -------------------------------------------------------------------------- */
const RegionBackgrounds = {
    'Europe': 'assets/backgrounds/bg-europe.svg',
    'CIS': 'assets/backgrounds/10-full-background.svg',
    'Americas': 'assets/backgrounds/bg-americas.svg',
    'Asia': 'assets/backgrounds/bg-asia.svg',
    'MiddleEast': 'assets/backgrounds/bg-middleeast.svg',
    'Oceania': 'assets/backgrounds/bg-oceania.svg',
    'Global': 'assets/backgrounds/10-full-background.svg'
};

const CurrencyToCountryFlag = {
    'USD': '🇺🇸', 'EUR': '🇪🇺', 'RUB': '🇷🇺', 'GBP': '🇬🇧', 'JPY': '🇯🇵', 'CNY': '🇨🇳', 
    'AED': '🇦🇪', 'AFN': '🇦🇫', 'ALL': '🇦🇱', 'AMD': '🇦🇲', 'AOA': '🇦🇴', 'ARS': '🇦🇷',
    'AUD': '🇦🇺', 'AWG': '🇦🇼', 'AZN': '🇦🇿', 'BAM': '🇧🇦', 'BBD': '🇧🇧', 'BDT': '🇧🇩', 
    'BHD': '🇧🇭', 'BIF': '🇧🇮', 'BMD': '🇧🇲', 'BND': '🇧🇳', 'BOB': '🇧🇴', 'BRL': '🇧🇷',
    'BSD': '🇧🇸', 'BTN': '🇧🇹', 'BWP': '🇧🇼', 'BYN': '🇧🇾', 'BZD': '🇧🇿', 'CAD': '🇨🇦', 
    'CDF': '🇨🇩', 'CHF': '🇨🇭', 'CLP': '🇨🇱', 'COP': '🇨🇴', 'CRC': '🇨🇷', 'CUP': '🇨🇺',
    'CVE': '🇨🇻', 'CZK': '🇨🇿', 'DJF': '🇩🇯', 'DKK': '🇩🇰', 'DOP': '🇩🇴', 'DZD': '🇩🇿', 
    'EGP': '🇪🇬', 'ERN': '🇪🇷', 'ETB': '🇪🇹', 'FJD': '🇫🇯', 'FKP': '🇫🇰', 'GEL': '🇬🇪',
    'GHS': '🇬🇭', 'GIP': '🇬🇮', 'GMD': '🇬🇲', 'GNF': '🇬🇳', 'GTQ': '🇬🇹', 'GYD': '🇬🇾', 
    'HKD': '🇭🇰', 'HNL': '🇭🇳', 'HTG': '🇭🇹', 'HUF': '🇭🇺', 'IDR': '🇮🇩', 'ILS': '🇮🇱',
    'INR': '🇮🇳', 'IQD': '🇮🇶', 'IRR': '🇮🇷', 'ISK': '🇮🇸', 'JMD': '🇯🇲', 'JOD': '🇯🇴', 
    'KES': '🇰🇪', 'KGS': '🇰🇬', 'KHR': '🇰🇭', 'KMF': '🇰🇲', 'KPW': '🇰🇵', 'KRW': '🇰🇷',
    'KWD': '🇰🇼', 'KYD': '🇰🇾', 'KZT': '🇰🇿', 'LAK': '🇱🇦', 'LBP': '🇱🇧', 'LKR': '🇱🇰', 
    'LRD': '🇱🇷', 'LSL': '🇱🇸', 'LYD': '🇱🇾', 'MAD': '🇲🇦', 'MDL': '🇲🇩', 'MGA': '🇲🇬',
    'MKD': '🇲🇰', 'MMK': '🇲🇲', 'MNT': '🇲🇳', 'MOP': '🇲🇴', 'MRU': '🇲🇷', 'MUR': '🇲🇺', 
    'MVR': '🇲🇻', 'MWK': '🇲🇼', 'MXN': '🇲🇽', 'MYR': '🇲🇾', 'MZN': '🇲🇿', 'NAD': '🇳🇦',
    'NGN': '🇳🇬', 'NIO': '🇳🇮', 'NOK': '🇳🇴', 'NPR': '🇳🇵', 'NZD': '🇳🇿', 'OMR': '🇴🇲', 
    'PAB': '🇵🇦', 'PEN': '🇵🇪', 'PGK': '🇵🇬', 'PHP': '🇵🇭', 'PKR': '🇵🇰', 'PLN': '🇵🇱',
    'PYG': '🇵🇾', 'QAR': '🇶🇦', 'RON': '🇷🇴', 'RSD': '🇷🇸', 'RWF': '🇷🇼', 'SAR': '🇸🇦', 
    'SBD': '🇸🇧', 'SCR': '🇸🇨', 'SDG': '🇸🇩', 'SEK': '🇸🇪', 'SGD': '🇸🇬', 'SHP': '🇸🇭',
    'SLL': '🇸🇱', 'SOS': '🇸🇴', 'SRD': '🇸🇷', 'SSP': '🇸🇸', 'STN': '🇸🇹', 'SVC': '🇸🇻', 
    'SYP': '🇸🇾', 'SZL': '🇸🇿', 'THB': '🇹🇭', 'TJS': '🇹🇯', 'TMT': '🇹🇲', 'TND': '🇹🇳',
    'TOP': '🇹🇴', 'TRY': '🇹🇷', 'TTD': '🇹🇹', 'TWD': '🇹🇼', 'TZS': '🇹🇿', 'UAH': '🇺🇦', 
    'UGX': '🇺🇬', 'UYU': '🇺🇾', 'UZS': '🇺🇿', 'VES': '🇻🇪', 'VND': '🇻🇳', 'VUV': '🇻🇺',
    'WST': '🇼🇸', 'XAF': '🇨🇲', 'XCD': '🇦🇬', 'XOF': '🇸🇳', 'XPF': '🇵🇫', 'YER': '🇾🇪', 
    'ZAR': '🇿🇦', 'ZMW': '🇿🇲', 'ZWG': '🇿🇼'
};

const ALL_CURRENCY_CODES = Object.keys(CurrencyToCountryFlag);
const OPTIONS_HTML = ALL_CURRENCY_CODES
    .map(code => `<option value="${code}">${CurrencyToCountryFlag[code]} ${code}</option>`)
    .join('');

/* --------------------------------------------------------------------------
   MODULE 2: INITIALIZATION & GEO-LOCATION
   -------------------------------------------------------------------------- */
function populateInitialSelects() {
    const baseSelect = document.getElementById('baseCurrency');
    if (baseSelect) {
        baseSelect.innerHTML = OPTIONS_HTML;
        baseSelect.value = 'PLN'; // Default fallback
    }

    const targetSelects = document.querySelectorAll('.target-select');
    targetSelects.forEach((select, index) => {
        select.innerHTML = OPTIONS_HTML;
        select.value = index === 0 ? 'USD' : 'EUR'; 
    });
}

async function initApp() {
    let iso = 'PL', region = 'Europe', baseCurrency = 'PLN';

    try {
        const response = await fetch('/api/Location/current', {
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
        console.warn('[Network] Location API offline. Using defaults.');
    }

    // Trigger translation based on the detected country code BEFORE setting UI
    loadLanguage(iso);

    applyRegionSettings(iso, region, baseCurrency);
}

function applyRegionSettings(isoCode, regionName, currencyCode) {
    const regionSelector = document.getElementById('regionSelector');
    if (regionSelector) regionSelector.value = regionName;

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

    window.location.hash = `#${regionName}`;
    updateTargetOptions();
    updateConversions();
}

// Manual region change handler
document.getElementById('regionSelector')?.addEventListener('change', (e) => {
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
});

/* --------------------------------------------------------------------------
   MODULE 3: DYNAMIC CONVERTER UI & UNIQUE SELECT LOGIC
   -------------------------------------------------------------------------- */
const MAX_CURRENCIES = 10;
let currentCurrencyCount = 2;

const targetRowsContainer = document.getElementById('targetRows');
const addCurrencyBtn = document.getElementById('addCurrencyBtn');
const removeCurrencyBtn = document.getElementById('removeCurrencyBtn');
const currencyCountSpan = document.getElementById('currencyCount');
const baseCurrencySelect = document.getElementById('baseCurrency');
const amountInput = document.getElementById('amount');

// Prevents the user from selecting the same currency multiple times across all dropdowns
function updateTargetOptions() {
    const baseSelect = document.getElementById('baseCurrency');
    const targetSelects = Array.from(document.querySelectorAll('.target-select'));

    if (!baseSelect) return;
    const baseValue = baseSelect.value;

    Array.from(baseSelect.options).forEach(option => {
        option.hidden = false;
        option.disabled = false;
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
    const allSelects = [
        document.getElementById('baseCurrency'), 
        ...document.querySelectorAll('.target-select')
    ].filter(el => el !== null);
    
    const selectedValues = new Set(allSelects.map(s => s.value));
    const available = ALL_CURRENCY_CODES.filter(code => !selectedValues.has(code));
    
    return available.length > 0 ? available[Math.floor(Math.random() * available.length)] : null;
}

if (addCurrencyBtn) {
    addCurrencyBtn.addEventListener('click', () => {
        if (currentCurrencyCount >= MAX_CURRENCIES) return;

        const newRow = document.createElement('div');
        newRow.className = 'row target-row';
        newRow.innerHTML = `
            <select class="glass select target-select">${OPTIONS_HTML}</select>
            <input type="text" class="glass input result-input target-result" value="0.0000" readonly>
        `;

        targetRowsContainer.appendChild(newRow);
        const newSelect = newRow.querySelector('.target-select');
        const randomCurrency = getRandomCurrency();
        if (randomCurrency) newSelect.value = randomCurrency;

        newSelect.addEventListener('change', () => { 
            updateTargetOptions(); 
            updateConversions(); 
        });

        currentCurrencyCount++;
        if (currencyCountSpan) currencyCountSpan.textContent = currentCurrencyCount;

        if (currentCurrencyCount >= MAX_CURRENCIES) addCurrencyBtn.style.display = 'none';
        if (currentCurrencyCount > 2 && removeCurrencyBtn) removeCurrencyBtn.style.display = 'flex';

        updateTargetOptions(); 
        updateConversions(); 
        adjustGraphHeight();
    });
}

if (removeCurrencyBtn) {
    removeCurrencyBtn.addEventListener('click', () => {
        if (currentCurrencyCount <= 2) return;

        const targetRows = document.querySelectorAll('.target-row');
        const lastRow = targetRows[targetRows.length - 1];
        
        const selectToRemove = lastRow.querySelector('.target-select');
        
        // Remove from blacklist if the deleted currency was hidden
        if (selectToRemove && typeof hiddenChartCurrencies !== 'undefined' && hiddenChartCurrencies.has(selectToRemove.value)) {
            hiddenChartCurrencies.delete(selectToRemove.value);
        }

        lastRow.remove();
        currentCurrencyCount--;
        if (currencyCountSpan) currencyCountSpan.textContent = currentCurrencyCount;

        addCurrencyBtn.style.display = 'flex';
        if (currentCurrencyCount <= 2) removeCurrencyBtn.style.display = 'none';

        updateTargetOptions(); 
        updateConversions(); 
        adjustGraphHeight(); 
        renderCurrencyChart();
    });
}

/* --------------------------------------------------------------------------
   MODULE 4: API INTEGRATION (REACTIVE FETCH)
   -------------------------------------------------------------------------- */
// Limits the rate of API calls while typing
function debounce(func, wait) {
    let timeout;
    return function(...args) { 
        clearTimeout(timeout); 
        timeout = setTimeout(() => func.apply(this, args), wait); 
    };
}

async function performRealConversion() {
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
        const url = new URL('/api/Convert/multi', window.location.origin);
        
        url.searchParams.append('baseCur', baseCurrency);
        url.searchParams.append('amount', amount);
        
        targetCurrencies.forEach(cur => url.searchParams.append('targetCurs', cur));

        const response = await fetch(url.toString(), {
            method: 'GET', 
            headers: { 'Accept': 'application/json' }
        });

        if (!response.ok) throw new Error(`HTTP Error: ${response.status}`);

        const data = await response.json();

        if (data.status === "success" && data.conversions) {
            targetRows.forEach(row => {
                const select = row.querySelector('.target-select');
                const resultInput = row.querySelector('.target-result');
                if (!select || !resultInput) return;

                const conversion = data.conversions.find(c => c.targetCurrency === select.value);

                if (conversion && conversion.success) {
                    resultInput.value = conversion.convertedAmount.toLocaleString('ru-RU', {
                        minimumFractionDigits: 4, 
                        maximumFractionDigits: 4
                    }) + ' ' + select.value;
                } else {
                    resultInput.value = "API Error";
                }
            });
        }
    } catch (err) {
        console.warn(`[API Error] Failed to fetch conversions:`, err);
    }

    if (typeof updateChartToggles === 'function') updateChartToggles();
    if (typeof renderCurrencyChart === 'function') renderCurrencyChart();
}

const updateConversions = debounce(performRealConversion, 300);

// Attach primary input listeners
if (amountInput) amountInput.addEventListener('input', updateConversions);
if (baseCurrencySelect) {
    baseCurrencySelect.addEventListener('change', () => { 
        updateTargetOptions(); 
        updateConversions(); 
    });
}

/* --------------------------------------------------------------------------
   MODULE 5: SMART CANVAS GRAPH ENGINE (WITH BLACKLIST & HIERARCHY)
   -------------------------------------------------------------------------- */
let hiddenChartCurrencies = new Set(); // Stores currencies the user has manually hidden
let currentRange = '1M';
const chartColors = { 
    base: '#9245e5', 
    line: '#4db892', 
    grid: 'rgba(255, 255, 255, 0.05)',
    text: 'rgba(255, 255, 255, 0.5)' 
};

// Fetches historical trend data
async function fetchRealHistory(baseCur, targetCur, range) {
    try {
        const url = `/api/Convert/history?baseCur=${baseCur}&targetCur=${targetCur}&range=${range}`;
        const response = await fetch(url, { headers: { 'Accept': 'application/json' } });
        if (!response.ok) throw new Error('History API failed');
        const json = await response.json();
        return json.status === 'success' ? json.data : [];
    } catch (e) {
        console.warn(`Failed to fetch history for ${targetCur}`, e);
        return [];
    }
}

// Main rendering function for the Canvas context
async function renderCurrencyChart() {
    const canvas = document.getElementById('currencyChart');
    const container = document.getElementById('canvasContainer');
    const baseCurrencySelect = document.getElementById('baseCurrency');
    
    if (!canvas || !container || !baseCurrencySelect) return;
    
    const ctx = canvas.getContext('2d');
    
    canvas.width = container.clientWidth; 
    canvas.height = container.clientHeight;
    const width = canvas.width; 
    const height = canvas.height;
    
    const padding = { top: 20, right: 60, bottom: 30, left: 10 };
    const graphWidth = width - padding.left - padding.right;
    const graphHeight = height - padding.top - padding.bottom;
    
    ctx.clearRect(0, 0, width, height);
    
    const baseCurrency = baseCurrencySelect.value;
    const currentTargets = Array.from(document.querySelectorAll('.target-select')).map(s => s.value);
    
    // Filter out user-hidden currencies
    const linesToDraw = ['base', ...currentTargets].filter(cur => !hiddenChartCurrencies.has(cur));
    
    const fetchPromises = linesToDraw.map(async (curType) => {
        const targetCur = curType === 'base' ? 'USD' : curType;
        const dataPoints = await fetchRealHistory(baseCurrency, targetCur, currentRange);
        const strokeColor = curType === 'base' ? chartColors.base : chartColors.line;
        return { dataPoints, strokeColor, targetCur, isBase: curType === 'base' };
    });

    const results = await Promise.all(fetchPromises);
    
    let globalMin = Infinity;
    let globalMax = -Infinity;
    
    // Calculate boundaries for the Y-Axis
    results.forEach(({ dataPoints }) => {
        if (!dataPoints.length) return;
        const prices = dataPoints.map(p => p.price);
        globalMin = Math.min(globalMin, ...prices);
        globalMax = Math.max(globalMax, ...prices);
    });

    const priceRange = globalMax - globalMin || 1;
    globalMin -= priceRange * 0.05; // 5% visual padding
    globalMax += priceRange * 0.05;
    const bufferedRange = globalMax - globalMin;

    ctx.font = '10px Inter, sans-serif';
    ctx.fillStyle = chartColors.text;
    ctx.textAlign = 'left';
    ctx.strokeStyle = chartColors.grid;
    ctx.lineWidth = 1;
    
    const horizontalLines = 4;
    for (let i = 0; i <= horizontalLines; i++) {
        let y = padding.top + (graphHeight / horizontalLines) * i;
        let priceLabel = globalMax - (bufferedRange / horizontalLines) * i;
        ctx.beginPath();
        ctx.moveTo(padding.left, y);
        ctx.lineTo(width - padding.right, y);
        ctx.stroke();
        ctx.fillText(priceLabel.toFixed(4), width - padding.right + 5, y + 4);
    }

    // Render individual trend lines
    results.forEach(({ dataPoints, strokeColor, targetCur, isBase }, index) => {
        if (!dataPoints || dataPoints.length === 0) return;

        ctx.beginPath(); 
        ctx.lineWidth = isBase ? 4 : 2; // Hierarchy: Base line is thicker
        ctx.strokeStyle = strokeColor; 
        ctx.lineJoin = 'round'; 
        ctx.lineCap = 'round';
        
        dataPoints.forEach((point, pIndex) => {
            let x = padding.left + (graphWidth / (dataPoints.length - 1)) * pIndex;
            let y = padding.top + graphHeight - ((point.price - globalMin) / bufferedRange) * graphHeight;
            
            pIndex === 0 ? ctx.moveTo(x, y) : ctx.lineTo(x, y);

            // X-Axis labels
            if (index === 0) {
                let labelStep = Math.max(1, Math.floor(dataPoints.length / 5));
                if (pIndex % labelStep === 0 || pIndex === dataPoints.length - 1) {
                    ctx.textAlign = 'center';
                    ctx.fillText(point.date, x, height - 10);
                }
            }
        });
        
        ctx.stroke();
        
        // Neon glow below the line
        let gradient = ctx.createLinearGradient(0, padding.top, 0, height - padding.bottom);
        let alpha = isBase ? '33' : '08'; // Base gradient is brighter
        gradient.addColorStop(0, strokeColor + alpha); 
        gradient.addColorStop(1, strokeColor + '00'); 
        ctx.lineTo(padding.left + graphWidth, height - padding.bottom); 
        ctx.lineTo(padding.left, height - padding.bottom); 
        ctx.closePath(); 
        ctx.fillStyle = gradient; 
        ctx.fill();

        // Currency text label at the end of the line
        const lastPoint = dataPoints[dataPoints.length - 1];
        const lastX = padding.left + graphWidth;
        const lastY = padding.top + graphHeight - ((lastPoint.price - globalMin) / bufferedRange) * graphHeight;
        ctx.fillStyle = strokeColor;
        ctx.font = 'bold 13px Inter, sans-serif';
        ctx.textAlign = 'right';
        ctx.fillText(targetCur, lastX, lastY - 8);
    });
}

function updateChartToggles() {
    const togglesContainer = document.getElementById('chartToggles');
    if (!togglesContainer) return;
    
    const baseCurrency = document.getElementById('baseCurrency').value;
    const currentTargets = Array.from(document.querySelectorAll('.target-select')).map(select => select.value);
    
    let html = `<div class="toggle-badge ${hiddenChartCurrencies.has('base') ? '' : 'active'}" data-cur="base">📈 ${baseCurrency} / USD (Base)</div>`;
    
    currentTargets.forEach(cur => {
        const isActive = !hiddenChartCurrencies.has(cur);
        html += `<div class="toggle-badge ${isActive ? 'active' : ''}" data-cur="${cur}">${CurrencyToCountryFlag[cur] || '🏳️'} ${baseCurrency} / ${cur}</div>`;
    });
    
    togglesContainer.innerHTML = html;
    togglesContainer.querySelectorAll('.toggle-badge').forEach(badge => {
        badge.addEventListener('click', () => {
            const cur = badge.getAttribute('data-cur');
            if (hiddenChartCurrencies.has(cur)) { 
                hiddenChartCurrencies.delete(cur); 
            } else { 
                hiddenChartCurrencies.add(cur); 
            }
            updateChartToggles(); 
            renderCurrencyChart();
        });
    });
}

function adjustGraphHeight() {
    const container = document.getElementById('canvasContainer');
    if (!container) return;
    container.style.height = `${260 + Math.max(0, currentCurrencyCount - 2) * 10}px`;
    setTimeout(renderCurrencyChart, 450);
}

// Event Listeners for Time Range Buttons (1D, 1W, 1M, 1Y, etc.)
document.getElementById('timeRangeControls')?.querySelectorAll('.time-btn').forEach(btn => {
    btn.addEventListener('click', () => {
        document.querySelector('.time-btn.active')?.classList.remove('active');
        btn.classList.add('active');
        currentRange = btn.getAttribute('data-range');
        renderCurrencyChart();
    });
});

window.addEventListener('resize', renderCurrencyChart);

/* --------------------------------------------------------------------------
   MODULE 6: VISUAL FX & BOOTSTRAP
   -------------------------------------------------------------------------- */
document.addEventListener('DOMContentLoaded', () => {
    const flagIcon = document.getElementById('flagIcon');
    const moneyIcon = document.getElementById('moneyIcon');
    const moneyLi = document.querySelector('[data-emoji-rotate="money"]'); 
    const moneyEmojis = ['💵', '💶', '💷', '💴', '💳'];

    // Auto-rotate flags
    if (flagIcon) {
        setInterval(() => {
            const randomCode = ALL_CURRENCY_CODES[Math.floor(Math.random() * ALL_CURRENCY_CODES.length)];
            flagIcon.textContent = CurrencyToCountryFlag[randomCode];
        }, 800); 
    }

    // Hover effect for money emojis
    if (moneyIcon && moneyLi) {
        let mIndex = 0; let moneyInterval = null;
        moneyLi.addEventListener('mouseenter', () => { 
            moneyInterval = setInterval(() => { 
                mIndex = (mIndex + 1) % moneyEmojis.length; 
                moneyIcon.textContent = moneyEmojis[mIndex]; 
            }, 350); 
        });
        moneyLi.addEventListener('mouseleave', () => { clearInterval(moneyInterval); });
    }
});

// Theme toggler
const moonBtn = document.querySelector('.moon-btn');
if (moonBtn) {
    moonBtn.addEventListener('click', () => {
        const html = document.documentElement;
        const isDark = html.getAttribute('data-theme') === 'dark';
        html.setAttribute('data-theme', isDark ? 'light' : 'dark');
        moonBtn.style.transform = isDark ? 'rotate(0deg)' : 'rotate(180deg)';
        renderCurrencyChart();
    });
}

// Parallax scrolling effect
const parallaxBg = document.getElementById('bg-image');
let isScrolling = false;
window.addEventListener('scroll', () => {
    if (!isScrolling && parallaxBg) {
        window.requestAnimationFrame(() => {
            parallaxBg.style.transform = `translateY(-${window.scrollY * 0.3}px)`;
            isScrolling = false;
        });
        isScrolling = true;
    }
});

/* --------------------------------------------------------------------------
   MODULE 7: I18N (INTERNATIONALIZATION) ENGINE
   -------------------------------------------------------------------------- */
const CountryToLanguage = {
    'RU': 'ru', 'BY': 'ru', 'KZ': 'ru', 'KG': 'ru', 'TJ': 'ru', 'UZ': 'ru',
    'US': 'en', 'GB': 'en', 'AU': 'en', 'CA': 'en', 'NZ': 'en', 'IE': 'en',
    'DE': 'de', 'AT': 'de', 'CH': 'de', 'LI': 'de',
    'FR': 'fr', 'BE': 'fr', 'LU': 'fr', 'MC': 'fr',
    'NL': 'nl', 'UA': 'uk', 'PL': 'pl',
    'ES': 'es', 'MX': 'es', 'AR': 'es', 'CO': 'es', 'CL': 'es', 
    'PE': 'es', 'VE': 'es', 'EC': 'es', 'GT': 'es', 'CU': 'es', 
    'BO': 'es', 'DO': 'es', 'HN': 'es', 'PY': 'es', 'SV': 'es', 
    'NI': 'es', 'CR': 'es', 'PA': 'es', 'UY': 'es',
    'BR': 'pt', 'PT': 'pt', 'AO': 'pt', 'MZ': 'pt',
    'CN': 'zh', 'TW': 'zh', 'SG': 'zh', 'HK': 'zh',
    'JP': 'ja', 'KR': 'ko', 'IN': 'hi', 'TR': 'tr',
    'AE': 'ar', 'SA': 'ar', 'EG': 'ar', 'IQ': 'ar', 'MA': 'ar', 
    'DZ': 'ar', 'SY': 'ar', 'YE': 'ar', 'TN': 'ar', 'JO': 'ar', 
    'LY': 'ar', 'LB': 'ar', 'KW': 'ar', 'OM': 'ar', 'QA': 'ar', 'BH': 'ar'
};

function getSystemLanguage() {
    const browserLang = navigator.language || navigator.userLanguage; 
    const shortLang = browserLang.split('-')[0].toLowerCase(); 
    
    const supportedLangs = ['en', 'ru', 'es', 'fr', 'de', 'pt', 'zh', 'ja', 'ko', 'hi', 'ar', 'tr', 'pl', 'nl', 'uk'];
    return supportedLangs.includes(shortLang) ? shortLang : 'en';
}

async function loadLanguage(isoCode = null) {
    let langCode = localStorage.getItem('userLang');

    if (!langCode) {
        langCode = getSystemLanguage();
    }

    const langSelector = document.getElementById('langSelector');
    if (langSelector) {
        langSelector.value = langCode;
    }

    try {
        const response = await fetch(`assets/i18n/${langCode}.json`);
        if (!response.ok) throw new Error('Translation file not found');
        
        const translations = await response.json();

        document.querySelectorAll('[data-i18n]').forEach(element => {
            const key = element.getAttribute('data-i18n');
            if (translations[key]) {
                element.innerHTML = translations[key];
            }
        });
        
        document.documentElement.lang = langCode;
        
        if (langCode === 'ar' || langCode === 'he') {
            document.documentElement.setAttribute('dir', 'rtl');
        } else {
            document.documentElement.removeAttribute('dir');
        }
        
    } catch (e) {
        console.warn(`[i18n] Failed to load language dictionary for: ${langCode}`, e);
    }
}

document.addEventListener('DOMContentLoaded', () => {
    const langSelector = document.getElementById('langSelector');
    if (langSelector) {
        langSelector.addEventListener('change', (e) => {
            const newLang = e.target.value;
            localStorage.setItem('userLang', newLang); 
            loadLanguage(); 
        });
    }
});

/* --------------------------------------------------------------------------
   APP BOOTSTRAP
   -------------------------------------------------------------------------- */
document.addEventListener('DOMContentLoaded', () => {
    populateInitialSelects();
    initApp();

    const prefersDark = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
    document.documentElement.setAttribute('data-theme', prefersDark ? 'dark' : 'light');

    document.querySelectorAll('.target-select').forEach(select => {
        select.addEventListener('change', () => {
            if (typeof updateTargetOptions === 'function') updateTargetOptions();
            updateConversions();
        });
    });
});