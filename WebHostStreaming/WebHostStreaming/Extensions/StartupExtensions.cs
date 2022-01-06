using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Providers;

namespace WebHostStreaming.StartupExtensions
{
    public static class StartupExtensions
    {
        public static void AddProviders(this IServiceCollection services)
        {
            services.AddSingleton<IVOMovieSearcherProvider, VOMovieSearcherProvider>();
            services.AddSingleton<IVFMovieSearcherProvider, VFMovieSearcherProvider>();
            services.AddSingleton<ISubtitlesSearcherProvider, SubtitlesSearcherProvider>();
            services.AddSingleton<IMovieStreamProvider, MovieStreamProvider>();
            services.AddSingleton<ISeenMovieBookmarkProvider, SeenMovieBookmarkProvider>();
            services.AddSingleton<IMovieToSeeBookmarkProvider, MovieToSeeBookmarkProvider>();
        }
    }
}
