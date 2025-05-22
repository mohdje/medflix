
using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace MoviesAPI.Services.Tmdb.Dtos
{
    internal class TmdbCredits
    {
        public Cast[] Cast { get; set; }
        public Crew[] Crew { get; set; }

        public string DirectorName => Crew?.FirstOrDefault(c => c.Job.Equals("director", StringComparison.OrdinalIgnoreCase))?.Name ?? Crew?.FirstOrDefault(c => c.Departement.Equals("Directing", StringComparison.OrdinalIgnoreCase))?.Name;
    }

    internal class Cast
    {
        public string Name { get; set; }
    }

    internal class Crew
    {
        public string Job { get; set; }
        public string Name { get; set; }

        [JsonPropertyName("known_for_department")]
        public string Departement { get; set; }
    }
}
