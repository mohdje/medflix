![Home page](/Screenshots/home_page.PNG)
# MEDFLIX

Medflix is a streaming platform I made to have fun with React and .Net Core. With this web application you can watch movies for free. 
There are three parts in this project :
* Frontend: it is made with React and you can see a demo version here https://medflix-mohdje.vercel.app/. You wont be able to watch movies on this link but you can browse into the app to test it.
* Backend: I implemented it in .Net Core. It provides APIs so the Frontend can interact with it to get information about movies, doing search operations and to get a stream to watch movies. This backend appliation uses a home-made library (MoviesAPI) to do searches and MonoTorrent to download movies and get a stream from a torrent.
* MoviesAPI:  A library I made to search for movies and subtitles on the web. More info about it in the MoviesAPI folder.

I did not deploy the backend on a server in order to not expose myself to legal problems. I only made accessible the frontend as a demo version so you can see how it looks. You can use the full app on your local machine to watch movies (see below 'how to use it') but if you decide to put the whole app on the web you will expose yourself to troubles and moreover I disagree with that.

## How to use it
If you want to use the full version of Medflix to watch movies, donwload a release from the Releases section. There is a web application (to access it throw web browser and share it to other devices of the same network), a Windows application and a MacOS application.

## How it looks
![Movies genre page](/Screenshots/movies_of_genre.PNG)
![Movies search](/Screenshots/movie_search.PNG)
![Movie presentation](/Screenshots/spiderman_presentation.PNG)
![Movie player](/Screenshots/video_player.PNG)
