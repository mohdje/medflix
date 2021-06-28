using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebHostStreaming.Providers
{
    public interface IMovieStreamProvider
    {
        Stream GetStream(string torrentUri, int offset);
        string GetStreamDownloadingState(string torrentUri);
    }
}
