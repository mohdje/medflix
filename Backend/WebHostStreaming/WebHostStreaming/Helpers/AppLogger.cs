using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;

namespace WebHostStreaming.Helpers
{
    public static class AppLogger
    {
        private const int MaxLogCount = 100;
        private static readonly Channel<string> Logs = Channel.CreateBounded<string>(
            new BoundedChannelOptions(MaxLogCount)
            {
                FullMode = BoundedChannelFullMode.DropOldest,
                SingleReader = false,
                SingleWriter = false
            });

        public static void LogInfo(string message)
        {
            var logmessage = $"-{GetDateTime()} : {message}";

            WriteLog(logmessage);
        }

        public static void LogInfo(string clientAppId, string message)
        {
            var logmessage = $"-{GetDateTime()}, Client-App-Id {clientAppId} : {message}";

            WriteLog(logmessage);
        }

        public static void LogError(string functionName, Exception exception)
        {
            var logmessage = $"-{GetDateTime()} : Error in {functionName}, {exception.GetBaseException().Message}";

            WriteLog(logmessage);
        }

        public static void LogError(string clientAppId, string functionName, Exception exception)
        {
            var logmessage = $"-{GetDateTime()}, Client-App-Id {clientAppId} : Error in {functionName}, {exception.GetBaseException().Message}";

            WriteLog(logmessage);
        }

        private static void WriteLog(string logmessage)
        {
            Logs.Writer.TryWrite(logmessage);

            Console.WriteLine(logmessage);
        }

        public static async IAsyncEnumerable<string> GetLogs(
                [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                bool hasLogs;

                try
                {
                    hasLogs = await Logs.Reader.WaitToReadAsync(ct);
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    yield break;
                }

                if (!hasLogs)
                    yield break;

                while (Logs.Reader.TryRead(out var log))
                    yield return log;
            }
        }

        private static string GetDateTime()
        {
            return DateTime.Now.ToString("MM/dd/yyyy H:mm:ss.fff");
        }
    }
}
