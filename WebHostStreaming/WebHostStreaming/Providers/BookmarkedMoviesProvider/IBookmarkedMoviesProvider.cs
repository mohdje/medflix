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
        Task<IEnumerable<LiteMovieDto>> GetBookmarkedMoviesAsync();
        Task SaveMovieBookmarkAsync(LiteMovieDto movieToBookmark);
        Task DeleteMovieBookmarkAsync(string movieId);
        Task<bool> MovieBookmarkExistsAsync(string movieId);
    }
}
