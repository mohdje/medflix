﻿using MoviesAPI.Services.Subtitles.OpenSubtitlesHtml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Subtitles
{
   

    public class SubtitlesSearcherFactory : ServiceFactory<SubtitlesService, SubtitlesSearcher>
    {
        public override SubtitlesSearcher GetService(SubtitlesService serviceType)
        {
            switch (serviceType)
            {
                case SubtitlesService.OpenSubtitlesHtml:
                    return new OpenSubtitlesHtmlService();
                case SubtitlesService.YtsSubs:
                    return new YtsSubsService();
                default:
                    return null;
            }
        }
    }
}