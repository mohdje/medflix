using System;

namespace WebHostStreaming.Models
{
    public class ErrorInfo
    {
        public DateTime DateTime { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string RequestUrl { get; set; }
        public string RequestHeaders { get; set; }
    }
}
