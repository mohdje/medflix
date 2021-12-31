using MoviesAPI.Services.Subtitles;
using MoviesAPI.Services.VFMovies;
using MoviesAPI.Services.VOMovies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesApiSample
{
    internal static class MoviesAPIServiceFactories
    {
        public static VOMovieSearcherFactory VOMovieSearcherFactory => new VOMovieSearcherFactory();
        public static VFMovieSearcherFactory VFMovieSearcherFactory => new VFMovieSearcherFactory();
        public static SubtitlesProviderFactory SubtitlesProviderFactory => new SubtitlesProviderFactory();
    }
}
