using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebHostStreaming.Helpers;

namespace WebHostStreaming.Providers
{
    public abstract class DataProvider
    {
        private SemaphoreSlim fileLocker = new SemaphoreSlim(1, 1);
        protected abstract int MaxLimit();

        protected async Task<IEnumerable<T>> GetDataAsync<T>(string filePath) where T : class
        {
            if (!System.IO.File.Exists(filePath))
                return null;

            await fileLocker.WaitAsync();

            try
            {
                return ReadDataFromFile<T>(filePath);
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                fileLocker.Release();
            }
        }

        protected async Task SaveDataAsync<T>(string filePath, T dataToSave, Func<T, T, bool> predicate, bool overrideIfExists = false) where T : class
        {
            await fileLocker.WaitAsync();

            try
            {
                var data = ReadDataFromFile<T>(filePath);

                var dataList = data?.ToList();

                if (dataList != null)
                {
                    if (predicate != null && dataList.Any(d => predicate(d, dataToSave)))
                    {
                        if (overrideIfExists)
                            dataList.RemoveAt(dataList.FindIndex(d => predicate(d, dataToSave)));
                        else
                            return;
                    }

                    if (dataList.Count() == MaxLimit())
                        dataList.RemoveAt(0);

                    dataList.Add(dataToSave);
                }
                else
                    dataList = new List<T> { dataToSave };

                if (!Directory.Exists(AppFolders.DataFolder))
                    Directory.CreateDirectory(AppFolders.DataFolder);

                JsonHelper.SerializeToFile(filePath, dataList);
            }
            catch (Exception ex){ }
            finally
            {
                fileLocker.Release();
            }
        }

        private static IEnumerable<T> ReadDataFromFile<T>(string filePath) where T : class
        {
            return JsonHelper.DeserializeFromFile<IEnumerable<T>>(filePath);
        }
    }
}
