using MoviesAPI.Services.Subtitles.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Subtitles
{
    internal interface ISubtitlesFileProvider
    {
        Task<string> GetSubtitlesFileAsync(string subtitlesSourceUrl, IEnumerable<KeyValuePair<string, string>> httpRequestHeaders);
    }
}
