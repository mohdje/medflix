![Home page](/Screenshots/home_page.PNG)
# MEDFLIX

Medflix is a streaming platform I made to have fun with React and .Net Core. With this web application you can watch movies and series for free. 
There are three parts in this project :
* Frontend: it is made with React.
* Backend: I implemented it in .Net Core. It provides APIs so the Frontend can interact with it to get information about movies, doing search operations and to get a stream to watch movies and series. This backend appliation uses a home-made library (MoviesAPI) to do searches and MonoTorrent to download movies and get a stream from a torrent.
* MoviesAPI:  A library I made to search for movies, series and subtitles on the web. More info about it in the MoviesAPI folder.

I did not deploy the backend on a server in order to not expose myself to legal problems. You can use the full app on your local machine to watch movies and series (see below 'how to use it') but if you decide to put the whole app on the web you will expose yourself to troubles and moreover I disagree with that.

## How to use it
If you want to use the full version of Medflix to watch movies and series, donwload a release from the Releases section (Windows and Macos available). You can also use Medflix as a web application using the docker image  available on docker hub (read Releases section for more info). Once docker image mounted, you can access it through your web browser from any device connected to the same network.

## How it looks
![Movies genre page](/Screenshots/movies_of_genre.PNG)
![Movies search](/Screenshots/movie_search.PNG)
![Movie presentation](/Screenshots/media_presentation.PNG)
![Movie presentation](/Screenshots/serie_presentation.PNG)
![Movie player](/Screenshots/video_player.PNG)

