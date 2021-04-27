# MOVIES API 
I implemented this library in .Net Standard in order to use it in every kind of projects (web server, mobile app and desktop). 
This library provides services that you can use to do different search operations to find movies. These are the available services :
* YtsApiMx : it uses the public api https://yts.mx/api to search movies and provides torrents url
* YtsApiLtd : it uses the public api https://yts.unblockit.ltd/api/v2/ to search movies and provides torrents url
* YtsHtmlMx : it performs searches throw the web site https://yts.mx to search movies and provides torrents url
* YtsHtmlLtd : it performs searches throw the web site https://yts.unblockit.ltd to search movies and provides torrents url
* YtsHtmlOne : it performs searches throw the web site https://yts.one to search movies and provides torrents url
* YtsHtmlPm : it performs searches throw the web site https://yts.pm to search movies and provides torrents url

Having several services allows you to switch from one to another if one of them is down for a moment (these endpoints provide non legal torrents links to download movies, so the servers could be down sometimes for different reasons). Moreover, you can find movies on a service that are not present on another.

MoviesAPI also provides a service to get subtitles for a movie. Behind the scene it searchs throw https://www.opensubtitles.org for available English and French subtitles of a given movie. It provides then a method to get a SubtitlesDto object from a subtitles .srt file.

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
 
