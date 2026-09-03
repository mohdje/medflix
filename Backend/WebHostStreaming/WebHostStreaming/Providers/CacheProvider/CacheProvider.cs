using System.Collections.Concurrent;


namespace WebHostStreaming.Providers
{
    public class CacheProvider<T> : ICacheProvider<T>
    {
        private readonly ConcurrentDictionary<string, T> cache = [];

        public void AddToCache(string key, T value)
        {
            cache.TryAdd(key, value);
        }

        public T GetFromCache(string key)
        {
            return cache.TryGetValue(key, out var value) ? value : default;
        }

        public void RemoveFromCache(string key)
        {
            cache.TryRemove(key, out _);
        }
    }
}