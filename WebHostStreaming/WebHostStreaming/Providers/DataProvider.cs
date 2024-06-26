﻿using System;
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

        protected async Task<IEnumerable<T>> GetDataAsync<T>(string filePath) where T : class
        {
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

        protected async Task SaveDataAsync<T>(string filePath, T dataToSave, Func<T, T, bool> predicate, bool overrideIfExists = false) where T : class
        {
            var data = await GetDataAsync<T>(filePath);

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

            JsonHelper.SerializeToFileAsync(filePath, dataList);
        }
    }
}
