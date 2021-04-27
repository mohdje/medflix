# WebHostStreaming

This is a web server application implemented using .Net Core. It provides APIs that a client can use to search movies and get a stream to watch a movie. 
This app has been designed to work for a single user usage on a local machine (for the movie playing part) so it can not be easily deployed on a server for a public usage (which would mean legal problems).

## How it works 

Built files from the UI part are embedded in View.zip file which is in the Resources folder. In release mode, at execution time this file is unzip and the default web browser of the local machine is launched and load the page https://localhost:5001/view/index.html to display the UI of the application. 
