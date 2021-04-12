using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebHostStreaming.Providers
{
    public interface IMovieStreamProvider
    {
        Task<Stream> GetMovieStreamAsync(string torrentUri, long offset);
    }
}
