namespace WebHostStreaming.Models
{
    public class DownloadingState
    {
        public static DownloadingState ReadyToPlaySoon => new DownloadingState("Download in progress, ready to play soon (6/6)");
        public static DownloadingState MediaDownloadStarted => new DownloadingState("Download has started (5/6)");
        public static DownloadingState MediaDownloadAboutToStart => new DownloadingState("Download is about to start (4/6)");
        public static DownloadingState WaitMediaDownloadToStart => new DownloadingState("Download initialization (3/6)");
        public static DownloadingState TorrentFileDownloaded => new DownloadingState("Resources found (2/6)");
        public static DownloadingState DownloadingTorrentFile => new DownloadingState("Searching resources (1/6)");
        public static DownloadingState TorrentFileDownloadFailed => new DownloadingState("Downloading resources failed", true);
        public static DownloadingState NoMediaFileFoundInTorrent => new DownloadingState("No supported video file found", true);
        public static DownloadingState NotFound => new DownloadingState("Unable to get downloading status", true);
        public static DownloadingState TorrentFileOpeningFailed => new DownloadingState("Failed to open resources", true);
        public static DownloadingState Loading => new DownloadingState("Loading");
        public string Message { get; }

        public bool Error { get; }

        private DownloadingState(string message, bool isError = false)
        {
            Message = message;
            Error = isError;
        }
    }
}
