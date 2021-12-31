using MoviesAPI.Services.VOMovies.YtsApi;
using MoviesAPI.Services.VOMovies.YtsHtml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.VOMovies
{
    
    public class VOMovieSearcherFactory : ServiceFactory<VOMovieService, IVOMovieSearcher>
    {
        public override IVOMovieSearcher GetService(VOMovieService VOMovieService)
        {
            switch (VOMovieService)
            {
                case VOMovieService.YtsApiMx:
                    return new YtsApiService(new YtsApiUrlMxProvider());
                case VOMovieService.YtsApiLtd:
                    return new YtsApiService(new YtsApiUrlLtdProvider());
                case VOMovieService.YtsHtmlMx:
                    return new YtsHtmlService(new YtsHtmlMxUrlProvider());
                case VOMovieService.YtsHtmlLtd:
                    return new YtsHtmlService(new YtsHtmlLtdUrlProvider());
                case VOMovieService.YtsHtmlOne:
                    return new YtsHtmlService(new YtsHtmlOneUrlProvider());
                case VOMovieService.YtsHtmlPm:
                    return new YtsHtmlService(new YtsHtmlPmUrlProvider());
                default:
                    return null;
            }
        }
    }
}
