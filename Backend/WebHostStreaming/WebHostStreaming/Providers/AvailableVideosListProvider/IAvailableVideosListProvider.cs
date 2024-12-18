﻿using System.Threading.Tasks;
using WebHostStreaming.Models;

namespace WebHostStreaming.Providers.AvailableVideosListProvider
{
    public interface IAvailableVideosListProvider
    {
        string[] VideosSourcesList { get; }
        Task<bool> AddMediaSource(string videoFilePath);
        bool RemoveMediaSource(string videoFilePath);
        Task<string> GetVoMovieSource(string movieName, int year);
        Task<string> GetVoSerieSource(string serieName, int seasonNumber, int episodeNumber);
        Task<string> GetVfMovieSource(string orignalMovieName, string frenchMovieName, int year);
        Task<string> GetVfSerieSource(string originalSerieName, string frenchSerieName, int seasonNumber, int episodeNumber);
    }
}
