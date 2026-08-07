using System.Collections.Generic;
using System.Threading.Tasks;
using WebHostStreaming.Models;

namespace WebHostStreaming.Converters
{
    public interface ISubtitlesConverter
    {
        Task<IEnumerable<SubtitlesDto>> ToSubtitlesDtoAsync(string subtitlesFilePath);
    }
}
