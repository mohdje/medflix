﻿using System.Text;
using System;

namespace WebHostStreaming.Helpers
{
    public static class AppLogger
    {
        public static void LogInfo(string message)
        {
            var logmessage = $"-{GetDateTime()} : {message}";

            Console.WriteLine(logmessage);
        }

        public static void LogInfo(string clientAppId, string message)
        {
            var logmessage = $"-{GetDateTime()}, Client-App-Id {clientAppId} : {message}";

            Console.WriteLine(logmessage);
        }

        public static void LogError(string functionName, Exception exception)
        {
            var logmessage = $"-{GetDateTime()} : Error in {functionName}, {exception.GetBaseException().Message}";

            Console.WriteLine(logmessage);
        }

        public static void LogError(string clientAppId, string functionName, Exception exception)
        {
            var logmessage = $"-{GetDateTime()}, Client-App-Id {clientAppId} : Error in {functionName}, {exception.GetBaseException().Message}";

            Console.WriteLine(logmessage);
        }

        private static string GetDateTime()
        {
            return DateTime.Now.ToString("MM/dd/yyyy H:mm:ss.fff");
        }
    }
}
