const CacheService = {
    cacheCollection: [],
    setCache(id, data){
        let cache = this.getCache(id);
        if(!cache){
            this.cacheCollection.push({
                id: id,
                data: data
            }); 
        }
        else
            cache.data = data;
    },
    getCache(id){
        return this.cacheCollection.find(c => c.id === id);
    },
    clearCache(){
        this.cacheCollection = [];
    }
}

export default CacheService;