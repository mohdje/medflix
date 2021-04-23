# MEDFLIX

Medflix is a streaming platform I made to have fun with react and .Net Core. With this web application you can watch movies for free. 
There are three parts in this project :
* Frontend: it is made in react and you can see here https://medflix-mohdje.vercel.app/ a demo version. You wont be able to watch movies on this link but you can browse into the app to test it.
* Backend: I implemented it in .Net Core. It provides APIs so the Frontend can interact with it to get information about movies, doing search operations and to get a stream to read to watch movies. This backend appliation uses a home-made library (MoviesAPI) to do all those operations.
* MoviesAPI: This is a .Net Standard library. I choose .Net Standard as a framewrok in order to be able to use it in every kind of projects (ASP .Net Core for web site, Xamarin for mobile, and even for desktop application). It provides services you can choose to search and watch movies while they are downloading. You can consider it as a torrent client as behind the scene this is what is used to download movies. More info about it in the MoviesAPI folder.

I did not deploy the backend on a server in order to not expose myself to legal problems. I only made accessible the frontend as a demo version so you can see how it looks. You can use the full app on local to watch movies (see below 'how to use it') but if you decide to put the whole app on the web you will expose yourself to troubles and moreover I disagree with that.

# How to use it
If you want to use the full version of Medflix to watch movies, pull the backend project and build it in Release mode. Then run the Medflix.exe file that will be generated in the Release folder. A console will open (this is the backend application launching)  and then your default browser will open a window that will load the frontend part of the application. 
