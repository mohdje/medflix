namespace WebHostStreaming.Models
{
    public class DownloadingState
    {
        public string Message { get; }

        public bool Error { get; }

        public DownloadingState(string message, bool isError = false)
        {
            Message = message;
            Error = isError;
        }
    }
}
