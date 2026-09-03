namespace WebHostStreaming.Providers
{
    public interface ICacheProvider<T>
    {
        void AddToCache(string key, T value);
        T GetFromCache(string key);
        void RemoveFromCache(string key);
    }
}
