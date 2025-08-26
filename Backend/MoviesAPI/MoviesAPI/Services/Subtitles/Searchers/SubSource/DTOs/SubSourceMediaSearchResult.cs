
namespace MoviesAPI.Services.Subtitles.Searchers.SubSource.DTOs
{
    internal class SubSourceMediaSearchResult
    {
        public SubSourceMedia[] Results { get; set; }

    }

    internal class SubSourceMedia
    {
        public string Link { get; set; }
    }
}
