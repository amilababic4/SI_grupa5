/**
 * SmartLib Client-Side Cache Service
 * Validates server-provided versions and manages localStorage lifecycle
 */

const SmartLibCache = (() => {
  const NAMESPACE = 'smartlib:';
  const VERSION_KEY = 'smartlib:version';

  return {
    /**
     * Get cached item if version matches and not expired
     * @param {string} key - Cache key
     * @param {number} serverVersion - Version from server header
     * @returns {any|null}
     */
    get(key, serverVersion) {
      try {
        const item = localStorage.getItem(NAMESPACE + key);
        if (!item) return null;

        const data = JSON.parse(item);
        
        // Check version match
        if (data.version !== serverVersion) {
          this.remove(key);
          return null;
        }
        
        // Check TTL
        const now = Date.now();
        if (data.fetchedAt + data.ttlMs < now) {
          this.remove(key);
          return null;
        }
        
        return data.payload;
      } catch (err) {
        console.warn(`Cache get error for ${key}:`, err);
        return null;
      }
    },

    /**
     * Set cached item with version and TTL
     * @param {string} key - Cache key
     * @param {any} payload - Data to cache
     * @param {number} version - Version from server
     * @param {number} ttlMs - Time to live in milliseconds
     */
    set(key, payload, version, ttlMs = 5 * 60 * 1000) {
      try {
        const item = {
          payload,
          version,
          fetchedAt: Date.now(),
          ttlMs
        };
        localStorage.setItem(NAMESPACE + key, JSON.stringify(item));
      } catch (err) {
        console.warn(`Cache set error for ${key}:`, err);
      }
    },

    /**
     * Remove cached item
     * @param {string} key - Cache key
     */
    remove(key) {
      try {
        localStorage.removeItem(NAMESPACE + key);
      } catch (err) {
        console.warn(`Cache remove error for ${key}:`, err);
      }
    },

    /**
     * Clear all SmartLib cache on logout/role change
     */
    clearAll() {
      try {
        const keys = [];
        for (let i = 0; i < localStorage.length; i++) {
          const key = localStorage.key(i);
          if (key && key.startsWith(NAMESPACE)) keys.push(key);
        }
        keys.forEach(k => localStorage.removeItem(k));
      } catch (err) {
        console.warn('Cache clearAll error:', err);
      }
    }
  };
})();
