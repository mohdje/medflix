using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesAPI.Services
{
    public enum MovieServiceType
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
        public static IVOMovieSearcher GetMovieService(MovieServiceType movieServiceType)
        {
            switch (movieServiceType)
            {
                case MovieServiceType.YtsApiMx:
                    return new YtsApi.YtsApiService(new YtsApi.YtsApiUrlMxProvider());
                case MovieServiceType.YtsApiLtd:
                    return new YtsApi.YtsApiService(new YtsApi.YtsApiUrlLtdProvider());
                case MovieServiceType.YtsHtmlMx:
                    return new YtsHtml.YtsHtmlService(new YtsHtml.YtsHtmlMxUrlProvider());
                case MovieServiceType.YtsHtmlLtd:
                    return new YtsHtml.YtsHtmlService(new YtsHtml.YtsHtmlLtdUrlProvider());
                case MovieServiceType.YtsHtmlOne:
                    return new YtsHtml.YtsHtmlService(new YtsHtml.YtsHtmlOneUrlProvider());
                case MovieServiceType.YtsHtmlPm:
                    return new YtsHtml.YtsHtmlService(new YtsHtml.YtsHtmlPmUrlProvider());
                default:
                    return null;
            }
        }

        public static IEnumerable<string> GetAvailableMovieServices()
        {
            foreach (MovieServiceType val in Enum.GetValues(typeof(MovieServiceType)))
            {
                yield return val.ToString();
            }
        }
    }
}
