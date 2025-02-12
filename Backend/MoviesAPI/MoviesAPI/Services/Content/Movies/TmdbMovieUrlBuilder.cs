
namespace MoviesAPI.Services.Tmdb
{
    internal class TmdbMovieUrlBuilder : TmdbUrlBuilder
    {
        protected override string ContentMode => "movie";
        protected override string SortBy => "release_date";
        public TmdbMovieUrlBuilder(string apiKey) : base(apiKey)
        {

        }
    }
}
