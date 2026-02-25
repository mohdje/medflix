
using MoviesAPI.Services.Content.Dtos;
using System;
using System.Collections.Generic;

namespace WebHostStreaming.Providers
{
    public interface IBookmarkedMoviesProvider
    {
        IEnumerable<LiteContentDto> GetBookmarkedMovies();
        void AddMovieBookmark(LiteContentDto movieToBookmark);
        void DeleteMovieBookmark(string movieId);
        bool MovieBookmarkExists(string movieId);
        event EventHandler<LiteContentDto> MovieBookmarkAdded;
        event EventHandler<string> MovieBookmarkDeleted;
    }
}
