﻿using HtmlAgilityPack;
using MoviesAPI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace MoviesAPI.Services.Torrent
{
    internal class TorrentVFMovieSearchRequest : TorrentVFSearchRequest
    {
        public string FrenchMovieName { get; }
        public int Year { get; }

        public bool UseOriginalNameForSearch { get; }

        public override string MediaName => FrenchMovieName;

        public TorrentVFMovieSearchRequest(string frenchMovieName, int year)
        {
            FrenchMovieName = frenchMovieName;
            Year = year;
        }

        protected override bool CheckTorrentTitle(string torrentTitle)
        {
            return torrentTitle.CustomStartsWith(FrenchMovieName)
                    && (torrentTitle.Contains("FRENCH") || torrentTitle.Contains("TRUEFRENCH"))
                    && torrentTitle.EndsWith(Year.ToString())
                    && !torrentTitle.Contains("MD")
                    && !torrentTitle.Contains("VOSTFR")
                    && (torrentTitle.Contains("720p") || torrentTitle.Contains("1080p") || torrentTitle.Contains("DVDRIP") || torrentTitle.Contains("WEBRIP"));
        }
    }
}
