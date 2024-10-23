using MoviesAPI.Services.Content.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebHostStreaming.Providers
{
    public interface IRecommandationsProvider
    {
        Task<IEnumerable<LiteContentDto>> GetMoviesRecommandationsAsync();
        Task<IEnumerable<LiteContentDto>> GetSeriesRecommandationsAsync();

    }
}
