using System.Text;
using System;

namespace WebHostStreaming.Helpers
{
    public static class AppLogger
    {
        public static void LogInfo(string message)
        {
            var logmessage = $"-{DateTime.Now.ToString("MM/dd/yyyy H:mm")} : {message}";

            Console.WriteLine(logmessage);
        }
    }
}
