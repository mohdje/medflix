using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebHostStreaming.Providers
{
    public abstract class DataStoreProvider<T> where T : class
    {
        private List<T> dataStore;
        protected abstract int MaxLimit { get; }
        protected abstract string FilePath { get; }

        protected IEnumerable<T> Data
        {
            get { lock (dataStore) { return dataStore; } }
        }

        protected DataStoreProvider()
        {
            dataStore = DeserializeFromFile();
        }

        protected void ExecuteInLockedContext(Action action, string contextMessage)
        {
            var oldDataStore = dataStore;

            try
            {
                lock (dataStore)
                {
                    action();
                }
            }
            catch (Exception ex)
            {
                dataStore = oldDataStore;
                throw new InvalidOperationException($"{contextMessage} FAILED: {ex.Message}", ex);
            }
        }

        protected void AddData(T data)
        {
            ExecuteInLockedContext(async () =>
            {
                if (data != null)
                {
                    if (dataStore.Count >= MaxLimit)
                        dataStore.RemoveAt(0);
                    dataStore.Add(data);

                    await SaveDataAsync();
                }
            }, "Add data to store");
        }

       
        protected void RemoveData(T data)
        {
            ExecuteInLockedContext(async () =>
            {
                var index = dataStore.IndexOf(data);
                if (index >= 0)
                {
                    dataStore.RemoveAt(index);
                    await SaveDataAsync();
                }

            }, "Remove data from store");
        }

        protected void UpdateData(T data)
        {
            ExecuteInLockedContext(async () =>
            {
                var index = dataStore.IndexOf(data);
                if (index >= 0)
                {
                    dataStore.RemoveAt(index);
                    dataStore.Add(data);

                    await SaveDataAsync();
                }
                else
                    throw new InvalidOperationException("Old data not found in store.");
                
            }, "Update data in store");
        }

        private List<T> DeserializeFromFile()
        {
            try
            {
                if (!File.Exists(FilePath))
                    return new List<T>();

                var json = File.ReadAllText(FilePath);

                return JsonSerializer.Deserialize<List<T>>(json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to deserialize data from file {FilePath}", ex);
            }
        }

        private async Task SaveDataAsync()
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(FilePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(FilePath));

                var jsonString = JsonSerializer.Serialize(dataStore);
                await File.WriteAllTextAsync(FilePath, jsonString);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to save data to file {FilePath}", ex);
            }
        }
    }
}
