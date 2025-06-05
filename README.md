![Logo](/Screenshots/logo.PNG)
# MEDFLIX

Medflix is a streaming platform I made with React and .Net. With this application you can watch movies and series for free. 
Architecture of this project :
* Client: it is made with React and MAUI (desktop and phone applications embed a webview disaplaying React app). Client app is available for Android (phone and tv), Windows and macOS soon.
* Backend: 
    - WebHostStreaming: made with .Net. It provides APIs so the Client can get information about movies, doing search operations and to get a stream to watch movies and series. It uses a home-made library (MoviesAPI) to do searches and MonoTorrent to download medias and get stream from torrents without waiting for it to be fully downloaded to watch it.
    - MoviesAPI:  A library I made to search for movies, series, subtitles and torrents on the web. More info about it in the MoviesAPI folder.

# How it looks
## Desktop:
![Home desktop](/Screenshots/home_desktop.PNG)
![Search desktop](/Screenshots/search_desktop.PNG)
![List desktop](/Screenshots/list_desktop.PNG)
![Movie Desktop](/Screenshots/movie_desktop.PNG)
![Serie Desktop](/Screenshots/serie_desktop.PNG)

## Phone:
![Home phone](/Screenshots/home_phone.jpg)
![List phone](/Screenshots/list_phone.jpg)
![Movie phone](/Screenshots/movie_phone.jpg)
![Serie phone](/Screenshots/serie_phone.jpg)

## Tv:
![Home tv](/Screenshots/home_tv.jpg)
![List tv](/Screenshots/list_tv.jpg)
![Movie tv](/Screenshots/movie_tv.jpg)

## Video Player:
![Video Player](/Screenshots/video_player.PNG)

## How to use it
The backend web server should be run on a machine using docker. Then donwload client application (for Andoird, Windows or Macos) from the Releases section and launch it. The first time the client application is launched the user will be asked to enter IP adress of the machine running the web server and the port (5000 by default).





