![Home page](/Screenshots/home_page.PNG)
# MEDFLIX

Medflix is a streaming platform I made with React and .Net. With this application you can watch movies and series for free. 
Architecture of this project :
* Client: it is made with React and MAUI (desktop and phone applications embed a webview disaplaying React app).
* Backend: 
    - WebHostStreaming: made with .Net. It provides APIs so the Client can get information about movies, doing search operations and to get a stream to watch movies and series. It uses a home-made library (MoviesAPI) to do searches and MonoTorrent to download medias and get stream from torrents without waiting for it to be fully downloaded to watch it.
    - MoviesAPI:  A library I made to search for movies, series, subtitles and torrents on the web. More info about it in the MoviesAPI folder.

## How it looks
![Movies genre page](/Screenshots/movies_of_genre.PNG)
![Movies search](/Screenshots/movie_search.PNG)
![Movie presentation](/Screenshots/media_presentation.PNG)
![Movie presentation](/Screenshots/serie_presentation.PNG)
![Movie player](/Screenshots/video_player.PNG)

## How to use it
The backend web server should be run on a machine using docker. Then donwload client application (for Andoird, Windows or Macos) from the Releases section and launch it. The first time the client application is launched the user will be asked to enter IP adress of the machine running the web server and the port (5000 by default).





