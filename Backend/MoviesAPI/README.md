# MOVIES API 
This library provides services that you can use to do different search operations to find movies, series, subtitles (english or french) and torrents (original version and french version).

# How to use it
To search for movies and series you need a TMDB api key, as it uses behind the scenes TMDB api. Then
use it like this:
```C#

var movieSearcher = MoviesAPI.Services.MoviesAPIFactory.Instance.CreateMovieSearcher(apiKey);
var searchMovies = await movieSearcher.SearchMoviesAsync("Encanto");
var popularNetflixMovies = await movieSearcher.GetPopularNetflixMoviesAsync();

var seriesSearcher = MoviesAPIFactory.Instance.CreateSeriesSearcher(apiKey);
var searchSeris = await seriesSearcher.SearchSeriesAsync("Breaking Bad");
var popularNetflixSeries = await seriesSearcher.GetPopularNetflixSeriesAsync();

var subtitlesSearchManager = MoviesAPIFactory.Instance.CreateSubstitlesSearchManager(AppContext.BaseDirectory);
var subtitlesMovie = await GetMovieSubtitlesAsync(imdbId, SubtitlesLanguage.French);
var subtitlesSerie = await GetSerieSubtitlesAsync(1, 3, imdbId, SubtitlesLanguage.English);

var  torrentSearchManager = MoviesAPIFactory.Instance.CreateTorrentSearchManager();
var torrentsVfMovie = await torrentSearchManager.SearchVfTorrentsMovieAsync("Le Robot Sauvage", 2024);
var torrentsVoMovie = await torrentSearchManager.SearchVoTorrentsMovieAsync("The Wild Robot", 2024);
var torrentsVfSerie = await torrentSearchManager.SearchVfTorrentsSerieAsync("Breaking Bad", 1, 3);
var torrentsVoSerie = await torrentSearchManager.SearchVoTorrentsSerieAsync("Breaking Bad", 1, 3);

```
Look at MoviesAPISample project to see all available methods and how use those.
 
