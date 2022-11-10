using MoviesAPI.Services.Movies.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebHostStreaming.Helpers;

namespace WebHostStreaming.Providers
{
    public class WatchedMoviesProvider : IWatchedMoviesProvider
    {
        private const int MaxLimit = 30;

        private string FilePath = AppFiles.WatchedMovies;

        public IEnumerable<LiteMovieDto> GetWatchedMovies()
        {
            var filePath = FilePath;
            if (!System.IO.File.Exists(filePath))
                return null;

            try
            {
                return JsonHelper.DeserializeFromFile<LiteMovieDto[]>(filePath);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void SaveWatchedMovie(LiteMovieDto movieToSave)
        {
            var watchedMovies = GetWatchedMovies();

            if (watchedMovies != null)
            {
                if (watchedMovies.Any(m => m.Id == movieToSave.Id))
                    return;

                var watchedMoviesList = watchedMovies.ToList();
                if (watchedMovies.Count() == MaxLimit)
                    watchedMoviesList.RemoveAt(0);

                watchedMoviesList.Add(movieToSave);
                watchedMovies = watchedMoviesList.ToArray();
            }
            else
                watchedMovies = new LiteMovieDto[] { movieToSave };

            if (!Directory.Exists(AppFolders.DataFolder))
                Directory.CreateDirectory(AppFolders.DataFolder);

            JsonHelper.SerializeToFileAsync(FilePath, watchedMovies);
        }
    }
}
