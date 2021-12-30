using MoviesAPI.Services.Subtitles.OpenSubtitlesHtml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Subtitles
{
   

    public static class SubtitlesProviderFactory
    {
        public static ISubtitlesProvider GetSubtitlesProvider(SubtitlesService subtitlesService)
        {
            switch (subtitlesService)
            {
                case SubtitlesService.OpenSubtitlesHtml:
                    return new OpenSubtitlesHtmlService();                   
                default:
                    return null;
            }
        }
    }
}
