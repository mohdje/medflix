namespace MoviesAPI.Services.Tmdb
{
    internal class TmdbSeriesUrlBuilder : TmdbUrlBuilder
    {
        public TmdbSeriesUrlBuilder(string apiKey) : base(apiKey)
        {

        }

        protected override string ContentMode => "tv";

        protected override string SortBy => "first_air_date";
    }
}
