const cacheCollection = [];

export function setCache(id, data) {
    let cache = cacheCollection.find(c => c.id === id);
    if (!cache) {
        cacheCollection.push({
            id: id,
            data: data
        });
    }
    else
        cache.data = data;
}

export function getCache(id) {
    return cacheCollection.find(c => c.id === id)?.data;
}
