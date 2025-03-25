![Home page](/Screenshots/home_page.PNG)
# MEDFLIX

Medflix is a streaming platform I made to have fun with React and .Net Core. With this web application you can watch movies and series for free. 
There are three parts in this project :
* Frontend: it is made with React.
* Backend: I implemented it in .Net Core. It provides APIs so the Frontend can interact with it to get information about movies, doing search operations and to get a stream to watch movies and series. This backend appliation uses a home-made library (MoviesAPI) to do searches and MonoTorrent to download movies and get a stream from a torrent.
* MoviesAPI:  A library I made to search for movies, series and subtitles on the web. More info about it in the MoviesAPI folder.

## How it looks
![Movies genre page](/Screenshots/movies_of_genre.PNG)
![Movies search](/Screenshots/movie_search.PNG)
![Movie presentation](/Screenshots/media_presentation.PNG)
![Movie presentation](/Screenshots/serie_presentation.PNG)
![Movie player](/Screenshots/video_player.PNG)

## How to use it
If you want to use the full version of Medflix to watch movies and series, donwload a release from the Releases section (Windows and Macos available). You can also use Medflix as a web application using the docker image  available on docker hub (read Releases section for more info). Once docker image mounted, you can access it through your web browser from any device connected to the same network.

# Warning
Before running following commands, make sure Docker Desktop is launched

# To build docker image
docker build --rm -t medflix-app:latest .

# To run medlix docker image
docker run -p 5000:5000 -e ASPNETCORE_HTTP_PORT=5000 -e ASPNETCORE_URLS="http://+:5000" --mount source=medflixVol,target=/medflix-app/storage djemo/medflix:latest

# Make a new image that is linked to the docker hub repo
docker image tag medflix-app:latest djemo/medflix:latest

# Push the image to the repo
docker image push djemo/medflix:latest



