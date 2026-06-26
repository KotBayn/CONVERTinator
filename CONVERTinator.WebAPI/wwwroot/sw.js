const CACHE_NAME = 'convertinator-cache-v1';
const ASSETS_TO_CACHE = [
  '/',
  '/index.html',
  '/styles.css',
  '/script.js',
  '/manifest.json'
];

self.addEventListener('install', (event) => {
    event.waitUntil(
        caches.open(CACHE_NAME)
        .then((cache) => cache.addAll(ASSETS_TO_CACHE))
    );
});

self.addEventListener('fetch', (event) => {
    if (event.request.url.includes('/api/')) {
        return; 
    }
    
    event.respondWith(
        fetch(event.request).catch(() => caches.match(event.request))
    );
});