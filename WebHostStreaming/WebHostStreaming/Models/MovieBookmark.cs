using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoviesAPI.Services.CommonDtos;

namespace WebHostStreaming.Models
{
    public class MovieBookmark
    {
        public MovieDto Movie { get; set; }
        public string ServiceName { get; set; }
    }
}
