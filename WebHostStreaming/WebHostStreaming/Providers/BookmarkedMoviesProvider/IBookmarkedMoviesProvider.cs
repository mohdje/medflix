using MoviesAPI.Services.Movies.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Models;

namespace WebHostStreaming.Providers
{
    public interface IBookmarkedMoviesProvider
    {
        IEnumerable<LiteMovieDto> GetBookmarkedMovies();
        void SaveMovieBookmark(LiteMovieDto movieToBookmark);
        void DeleteMovieBookmark(string movieId);
        bool MovieBookmarkExists(string movieId);
    }
}
