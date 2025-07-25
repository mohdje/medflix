﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebHostStreaming.Helpers
{
    public static class AppFiles
    {
        public static string WatchedMovies => Path.Combine(AppFolders.DataFolder, "watchedmovies.json");
        public static string WatchedSeries => Path.Combine(AppFolders.DataFolder, "watchedseries.json");
        public static string BookmarkedMovies => Path.Combine(AppFolders.DataFolder, "bookmarkedmovies.json");
        public static string BookmarkedSeries => Path.Combine(AppFolders.DataFolder, "bookmarkedseries.json");
        public static string TorrentHistory => Path.Combine(AppFolders.DataFolder, "torrenthistory.json");
        public static string AvailableMediaSources => Path.Combine(AppFolders.DataFolder, "available_media_sources.txt");
        public static string VideosInfos => Path.Combine(AppFolders.DataFolder, "videosinfos.json");
        public static string UploadHtmlPage => Path.Combine(AppFolders.ManageViewFolder, "index.html");
    }
}
