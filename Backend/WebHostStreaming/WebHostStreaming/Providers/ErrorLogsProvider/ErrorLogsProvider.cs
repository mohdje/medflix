using System.Collections.Generic;
using System.Linq;
using WebHostStreaming.Helpers;
using WebHostStreaming.Models;

namespace WebHostStreaming.Providers
{
    public class ErrorLogsProvider : DataStoreProvider<ErrorInfo>, IErrorLogsProvider
    {
        protected override int MaxLimit => int.MaxValue;

        protected override string FilePath => AppFiles.ErrorLogs;

        public IEnumerable<ErrorInfo> GetErrorLogs()
        {
            return Data;
        }

        public void AddErrorLog(ErrorInfo errorInfo)
        {
            AddData(errorInfo);
        }

        public void DeleteErrorLogs()
        {
            RemoveAllData();
        }
    }
}