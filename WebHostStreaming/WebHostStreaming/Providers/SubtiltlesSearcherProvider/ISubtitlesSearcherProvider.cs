﻿using MoviesAPI.Services.CommonDtos;
using MoviesAPI.Services.Subtitles;
using System.Collections.Generic;

namespace WebHostStreaming.Providers
{
    public interface ISubtitlesSearcherProvider
    {
        SubtitlesSearcher GetActiveSubtitlesSearcher();
        IEnumerable<ServiceInfo> GetSubtitlesServicesInfo();
        void UpdateSelectedSubtitlesSearcher(int selectedSubtitleServiceId);
    }
}