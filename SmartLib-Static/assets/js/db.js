/*
 * SmartLib-Static — localStorage "repository" layer.
 * Every collection is stored as a JSON array under the key `smartlib:<collection>`.
 * This replaces the real SQL database for the static demo build.
 */
(function (global) {
    "use strict";

    const PREFIX = "smartlib:";

    function key(collection) {
        return PREFIX + collection;
    }

    function getAll(collection) {
        const raw = localStorage.getItem(key(collection));
        return raw ? JSON.parse(raw) : [];
    }

    function saveAll(collection, items) {
        localStorage.setItem(key(collection), JSON.stringify(items));
    }

    function nextId(collection) {
        const items = getAll(collection);
        return items.length ? Math.max.apply(null, items.map((i) => i.id)) + 1 : 1;
    }

    function insert(collection, obj) {
        const items = getAll(collection);
        if (obj.id == null) obj.id = nextId(collection);
        items.push(obj);
        saveAll(collection, items);
        return obj;
    }

    function update(collection, id, patch) {
        const items = getAll(collection);
        const idx = items.findIndex((i) => i.id === Number(id));
        if (idx === -1) return null;
        items[idx] = Object.assign({}, items[idx], patch);
        saveAll(collection, items);
        return items[idx];
    }

    function remove(collection, id) {
        const items = getAll(collection);
        saveAll(collection, items.filter((i) => i.id !== Number(id)));
    }

    function find(collection, id) {
        if (id == null) return null;
        return getAll(collection).find((i) => i.id === Number(id)) || null;
    }

    function query(collection, predicate) {
        return getAll(collection).filter(predicate || (() => true));
    }

    function isSeeded() {
        return localStorage.getItem(key("__seeded")) === "true";
    }

    function markSeeded() {
        localStorage.setItem(key("__seeded"), "true");
    }

    function resetAll() {
        Object.keys(localStorage)
            .filter((k) => k.indexOf(PREFIX) === 0)
            .forEach((k) => localStorage.removeItem(k));
    }

    function paginate(items, page, pageSize) {
        page = Math.max(1, page || 1);
        pageSize = pageSize || 10;
        const total = items.length;
        const totalPages = Math.max(1, Math.ceil(total / pageSize));
        page = Math.min(page, totalPages);
        const start = (page - 1) * pageSize;
        return {
            items: items.slice(start, start + pageSize),
            page: page,
            pageSize: pageSize,
            total: total,
            totalPages: totalPages,
        };
    }

    global.DB = {
        PREFIX,
        getAll,
        saveAll,
        nextId,
        insert,
        update,
        remove,
        find,
        query,
        isSeeded,
        markSeeded,
        resetAll,
        paginate,
    };
})(window);
