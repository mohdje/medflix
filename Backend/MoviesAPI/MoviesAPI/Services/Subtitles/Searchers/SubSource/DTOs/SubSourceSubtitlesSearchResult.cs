namespace MoviesAPI.Services.Subtitles.Searchers.SubSource.DTOs
{
    internal class SubSourceSubtitlesSearchResult
    {
        public SubSourceSubtitle[] Subtitles { get; set; }
    }

    internal class SubSourceSubtitle
    {
        public int Id { get; set; }
        public string[] UploaderBadges { get; set; }
        public string Link { get; set; }
        public string Rating { get; set; }
        public string ReleaseInfo { get; set; }
    }
}
