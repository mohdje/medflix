# MOVIES API 
This library provides services that you can use to do different search operations to find movies (in original and french version) and subtitles.

To make a search operation you first have to choose a "service", which behind the scene corresponds to the site (or api) the library will use to the search. Having several services allows you to switch from one to another if one of them is down for a moment (these endpoints provide non legal torrents links to download movies, so the servers could be down sometimes for different reasons). Moreover, you can find movies on a service that are not present on another.

MoviesAPI also provides a service to get subtitles for a movie. Here again you can chose the service you will use in order to do te search. This library will look for available English and French subtitles of a given movie. It provides then a method to get a SubtitlesDto object from a subtitles .srt file.

# How to use it

It is pretty simple. In your project add a reference to this library. Use the static method GetMovieService of the MovieServiceFactory class. As a parameter pass the MovieServiceType you want. It will retrieves an IMovieService corresponding to the MovieServiceType. 

```C#

using MoviesAPI.Services;

public IMovieService GetMovieService()
{
    return MovieServiceFactory.GetMovieService(MovieServiceType.YtsApiMx);
}
```

IMovieService exposes few methods to search movies and to have details about a movie. 

```C#

var movieService = MovieServiceFactory.GetMovieService(MovieServiceType.YtsApiMx);
var movies = await movieService.GetMoviesByNameAsync(text);
var movieDetails = await movieService.GetMoviesDetailsAsync(id);

```
 
