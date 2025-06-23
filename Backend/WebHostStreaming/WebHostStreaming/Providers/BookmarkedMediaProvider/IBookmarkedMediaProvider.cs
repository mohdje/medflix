
using MoviesAPI.Services.Content.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Models;

namespace WebHostStreaming.Providers
{
    public interface IBookmarkedMediaProvider
    {
        Task<IEnumerable<LiteContentDto>> GetBookmarkedMoviesAsync();
        Task SaveMovieBookmarkAsync(LiteContentDto movieToBookmark);
        Task DeleteMovieBookmarkAsync(string movieId);
        Task<bool> MovieBookmarkExistsAsync(string movieId);
        Task<IEnumerable<LiteContentDto>> GetBookmarkedSeriesAsync();
        Task SaveSerieBookmarkAsync(LiteContentDto serieToBookmark);
        Task DeleteSerieBookmarkAsync(string serieId);
        Task<bool> SerieBookmarkExistsAsync(string serieId);

        Task InitDownloadBookmarkedMoviesAsync();
    }
}
