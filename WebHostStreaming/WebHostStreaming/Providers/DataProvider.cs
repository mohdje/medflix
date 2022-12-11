using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Helpers;

namespace WebHostStreaming.Providers
{
    public abstract class DataProvider
    {
        protected abstract int MaxLimit();

        protected abstract string FilePath();

        protected async Task<IEnumerable<T>> GetDataAsync<T>() where T : class
        {
            var filePath = FilePath();
            if (!System.IO.File.Exists(filePath))
                return null;

            try
            {
                return await JsonHelper.DeserializeFromFileAsync<IEnumerable<T>>(filePath);
            }
            catch (Exception)
            {
                return null;
            }
        }

        protected async Task SaveDataAsync<T>(T dataToSave, Func<T, T, bool> predicate) where T : class
        {
            var data = await GetDataAsync<T>();

            var dataList = data?.ToList();

            if (dataList != null)
            {
                if (predicate != null && dataList.Any(d => predicate(d, dataToSave)))
                    return;

                if (dataList.Count() == MaxLimit())
                    dataList.RemoveAt(0);

                dataList.Add(dataToSave);
            }
            else
                dataList = new List<T> { dataToSave };

            if (!Directory.Exists(AppFolders.DataFolder))
                Directory.CreateDirectory(AppFolders.DataFolder);

            JsonHelper.SerializeToFileAsync(FilePath(), dataList);
        }
    }
}
