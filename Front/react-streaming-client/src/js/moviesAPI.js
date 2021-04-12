import { movies } from "./fakeData";
const MoviesAPI = {
    apiMoviesUrl: 'https://localhost:5001/movies/',
    apiStreamUrl: 'https://localhost:5001/movies/stream?url=',
    apiSubtitlesUrl: 'https://localhost:5001/subtitles/',
    apiServicesUrl: 'https://localhost:5001/services/',
    apiLastSeenMoviesUrl: 'https://localhost:5001/lastseenmovies/',


    getMoviesGenres(onSuccess, onFail) {
        var url = this.apiMoviesUrl + 'genres';
        this.sendRequest(url, [], onSuccess, onFail);
    },

    getLastMoviesByGenre(genre, onSuccess, onFail) {
        var url = this.apiMoviesUrl + 'genre/' + genre;
        this.sendRequest(url, [], onSuccess, onFail);
    },

    getSuggestedMovies(onSuccess, onFail) {
        var url = this.apiMoviesUrl + 'suggested'
        this.sendRequest(url, [], onSuccess, onFail);
    },

    getMoviesByGenre(genre, page, onSuccess, onFail) {
        var url = this.apiMoviesUrl + 'genre/' + genre + '/' + page;
        this.sendRequest(url, [], onSuccess, onFail);
    },
    getMovieDetails(id, onSuccess, onFail) {
        var url = this.apiMoviesUrl + 'details/' + id;
        this.sendRequest(url, [], onSuccess, onFail);
    },

    searchMovies(text, onSuccess, onFail) {
        var url = this.apiMoviesUrl + 'search/' + text
        this.sendRequest(url, [], onSuccess, onFail);
    },

    getAvailableSubtitles(imdbId, onSuccess, onFail) {
        var url = this.apiSubtitlesUrl + 'available/' + imdbId;
        this.sendRequest(url, [], onSuccess, onFail);
    },

    getAvailableMovieServices(onSuccess, onFail) {
        this.sendRequest(this.apiServicesUrl, [], onSuccess, onFail);
    },

    changeMovieService(serviceName, onSuccess, onFail) {

        // setTimeout(()=>{
        //     onSuccess();
        // },3000);

        var xhttp = new XMLHttpRequest();
        xhttp.onreadystatechange = function () {
            if (this.readyState === 4) {
                if (this.status === 200) {
                    if (onSuccess)
                        onSuccess();
                }
                else {
                    if (onFail)
                        onFail();
                }
            }
        };
        xhttp.open("POST", this.apiServicesUrl, true);
        xhttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        xhttp.send("serviceName=" + serviceName);
    },

    getLastSeenMovies(onSuccess, onFail) {

        // setTimeout(()=>{
        //     onSuccess(movies);
        // },5000);
        this.sendRequest(this.apiMoviesUrl + "lastseenmovies", [], onSuccess, onFail);
    },

    saveLastSeenMovie(movie) {
        var xhttp = new XMLHttpRequest();
        xhttp.open("PUT", this.apiMoviesUrl + "lastseenmovies", true);
        xhttp.setRequestHeader("Content-Type", "application/json");
        xhttp.send(JSON.stringify(movie));
    },

    sendRequest(url, parameters, onSuccess, onFail) {
        var xhttp = new XMLHttpRequest();
        xhttp.onreadystatechange = function () {
            if (this.readyState === 4) {
                if (this.status === 200) {
                    var result = JSON.parse(this.response);
                    if (onSuccess)
                        onSuccess(result);
                }
                else {
                    if (onFail)
                        onFail();
                }
            }
        };

        if (parameters && parameters.length > 0) {
            url += '?';
            for (let i = 0; i < parameters.length; i++) {
                const parameter = parameters[i];
                url += parameter.name + '=' + parameter.value;
                if (i !== parameters.length - 1)
                    url += '&'
            }
        }

        xhttp.open("GET", url, true);
        xhttp.send();
    }
}



export default MoviesAPI;