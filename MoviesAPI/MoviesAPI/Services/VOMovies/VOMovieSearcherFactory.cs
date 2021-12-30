using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesAPI.Services.VOMovies
{
    public enum VOMovieService
    {
        YtsApiMx = 0,
        YtsApiLtd = 1,
        YtsHtmlMx = 2,
        YtsHtmlLtd = 3,
        YtsHtmlOne = 4,
        YtsHtmlPm = 5
    }

    public class VOMovieSearcherFactory
    {
        public static IVOMovieSearcher GetMovieService(VOMovieService VOMovieService)
        {
            switch (VOMovieService)
            {
                case VOMovieService.YtsApiMx:
                    return new YtsApi.YtsApiService(new YtsApi.YtsApiUrlMxProvider());
                case VOMovieService.YtsApiLtd:
                    return new YtsApi.YtsApiService(new YtsApi.YtsApiUrlLtdProvider());
                case VOMovieService.YtsHtmlMx:
                    return new YtsHtml.YtsHtmlService(new YtsHtml.YtsHtmlMxUrlProvider());
                case VOMovieService.YtsHtmlLtd:
                    return new YtsHtml.YtsHtmlService(new YtsHtml.YtsHtmlLtdUrlProvider());
                case VOMovieService.YtsHtmlOne:
                    return new YtsHtml.YtsHtmlService(new YtsHtml.YtsHtmlOneUrlProvider());
                case VOMovieService.YtsHtmlPm:
                    return new YtsHtml.YtsHtmlService(new YtsHtml.YtsHtmlPmUrlProvider());
                default:
                    return null;
            }
        }

        public static IEnumerable<string> GetAvailableMovieServices()
        {
            foreach (VOMovieService val in Enum.GetValues(typeof(VOMovieService)))
            {
                yield return val.ToString();
            }
        }
    }
}
