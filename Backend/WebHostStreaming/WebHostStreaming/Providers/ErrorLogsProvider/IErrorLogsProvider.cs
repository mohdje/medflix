using System.Collections.Generic;
using WebHostStreaming.Models;

namespace WebHostStreaming.Providers
{
    public interface IErrorLogsProvider
    {
        IEnumerable<ErrorInfo> GetErrorLogs();
        void AddErrorLog(ErrorInfo errorInfo);
        void DeleteErrorLogs();
    }
}