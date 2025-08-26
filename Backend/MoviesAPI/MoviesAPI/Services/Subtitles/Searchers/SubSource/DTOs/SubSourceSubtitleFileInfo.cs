namespace MoviesAPI.Services.Subtitles.Searchers.SubSource.DTOs
{
    internal class SubSourceSubtitleFileInfo
    {
        public SubSourceFileInfo Subtitle { get; set; }
    }

    internal class SubSourceFileInfo
    {
        public string DownloadToken { get; set; }
    }
}
