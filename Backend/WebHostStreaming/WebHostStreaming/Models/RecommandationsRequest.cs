namespace WebHostStreaming.Models
{
    public class RecommandationsRequest
    {
        public string[] GenreIds { get; }
        public string MinDate { get; }
        public string MaxDate { get; }
        public string[] ExcludedMediasIds { get; }
        public RecommandationsRequest(string[] genreIds, string minDate, string maxDate, string[] excludedMediasIds)
        {
            this.GenreIds = genreIds;
            this.MaxDate = maxDate;
            this .MinDate = minDate;
            this.ExcludedMediasIds = excludedMediasIds;
        }

    }
}
