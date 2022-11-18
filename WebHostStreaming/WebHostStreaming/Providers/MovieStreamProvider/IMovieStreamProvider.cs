using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Models;

namespace WebHostStreaming.Providers
{
    public interface IMovieStreamProvider
    {
        Task<StreamDto> GetStreamAsync(string torrentUri, int offset, string videoFormat);
        DownloadingState GetStreamDownloadingState(string torrentUri);
    }
}
