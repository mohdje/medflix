import { movies } from "./fakeData";
const MoviesAPI = {
    apiMoviesUrl: 'https://localhost:5001/movies/',
    apiStreamUrl: 'https://localhost:5001/movies/stream?url=',
    apiSubtitlesUrl: 'https://localhost:5001/subtitles/',
    apiServicesUrl: 'https://localhost:5001/services/',
    apiLastSeenMoviesUrl: 'https://localhost:5001/movies/lastseenmovies/',
    apiBookmarkedMoviesUrl: 'https://localhost:5001/movies/bookmarks/',
    apiMovieDownloadStateUrl: 'https://localhost:5001/movies/streamdownloadstate/',


    getMoviesGenres(onSuccess, onFail) {
        var url = this.apiMoviesUrl + 'genres';
        this.sendRequest(url, [], true, onSuccess, onFail);
    },

    getLastMoviesByGenre(genre, onSuccess, onFail) {
        var url = this.apiMoviesUrl + 'genre/' + genre;
        this.sendRequest(url, [], true, onSuccess, onFail);
    },

    getSuggestedMovies(onSuccess, onFail) {
        var url = this.apiMoviesUrl + 'suggested'
        this.sendRequest(url, [], true, onSuccess, onFail);
    },

    getMoviesByGenre(genre, page, onSuccess, onFail) {
        var url = this.apiMoviesUrl + 'genre/' + genre + '/' + page;
        this.sendRequest(url, [], true, onSuccess, onFail);
    },
    getMovieDetails(id, onSuccess, onFail) {
        var url = this.apiMoviesUrl + 'details/' + id;
        this.sendRequest(url, [], true, onSuccess, onFail);
    },

    searchMovies(text, onSuccess, onFail) {
        var url = this.apiMoviesUrl + 'search/' + text
        this.sendRequest(url, [], true, onSuccess, onFail);
    },

    searchVFSources(title, year, onSuccess, onFail){
        var url = this.apiMoviesUrl + 'vf';
        var parameters = [
            {
                name: 'title',
                value: title
            },
            {
                name: 'year',
                value: year
            }
        ];

        this.sendRequest(url, parameters, true, onSuccess, onFail);
    },

    getAvailableSubtitles(imdbId, onSuccess, onFail) {
        var url = this.apiSubtitlesUrl + 'available/' + imdbId;
        this.sendRequest(url, [], true, onSuccess, onFail);
    },

    getActiveVOMovieService(onSuccess, onFail){
        this.sendRequest(this.apiServicesUrl + "vo/selected", [], false, onSuccess, onFail);
    },

    getVOMovieServices(onSuccess, onFail){
        this.sendRequest(this.apiServicesUrl + "vo", [], true, onSuccess, onFail);
    },

    selectVOMovieService(serviceId, onSuccess, onFail){
        this.selectService(this.apiServicesUrl + "vo", serviceId, onSuccess, onFail);
    },

    getVFMovieServices(onSuccess, onFail){
        this.sendRequest(this.apiServicesUrl + "vf", [], true, onSuccess, onFail);
    },

    selectVFMovieService(serviceId, onSuccess, onFail){
        this.selectService(this.apiServicesUrl + "vf", serviceId, onSuccess, onFail);
    },

    getSubtitlesMovieServices(onSuccess, onFail){
        this.sendRequest(this.apiServicesUrl + "subtitles", [], true, onSuccess, onFail);
    },

    selectSubtitlesMovieService(serviceId, onSuccess, onFail){
        this.selectService(this.apiServicesUrl + "vf", serviceId, onSuccess, onFail);
    },

    selectService(serviceUrl, selectedServiceId, onSuccess, onFail) {
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
        xhttp.open("POST", serviceUrl, true);
        xhttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        xhttp.send("selectedServiceId=" + selectedServiceId);
    },

    getLastSeenMovies(onSuccess, onFail) {
        this.sendRequest(this.apiLastSeenMoviesUrl, [], true, onSuccess, onFail);
    },

    saveLastSeenMovie(movie) {
        var xhttp = new XMLHttpRequest();
        xhttp.open("PUT", this.apiLastSeenMoviesUrl, true);
        xhttp.setRequestHeader("Content-Type", "application/json");
        xhttp.send(JSON.stringify(movie));
    },

    getBookmarkedMovies(onSuccess, onFail){
        this.sendRequest(this.apiBookmarkedMoviesUrl, [], true, onSuccess, onFail);
    },

    addBookmarkedMovie(movie, onSuccess) {
        var xhttp = new XMLHttpRequest();
        xhttp.open("PUT", this.apiBookmarkedMoviesUrl, true);
        xhttp.setRequestHeader("Content-Type", "application/json");
        xhttp.send(JSON.stringify(movie));

        xhttp.onreadystatechange = function () {
            if (this.readyState === 4) 
                if (this.status === 200) 
                    onSuccess();
            }
    },

    deleteBookmarkedMovie(movieBookmark, onSuccess) {
        var xhttp = new XMLHttpRequest();
        xhttp.open("DELETE", this.apiBookmarkedMoviesUrl, true);
        xhttp.setRequestHeader("Content-Type", "application/json");
        xhttp.send(JSON.stringify(movieBookmark));

        xhttp.onreadystatechange = function () {
            if (this.readyState === 4) 
                if (this.status === 200) 
                    onSuccess();
            }
    },

    isMovieBookmarked(movieId, serviceName, onSuccess, onFail){
        var parameters = [
            {
                name: 'movieId',
                value: movieId
            },
            {
                name: 'serviceName',
                value: serviceName
            }
        ];

        this.sendRequest(this.apiBookmarkedMoviesUrl + 'exists', parameters, false, onSuccess, onFail);
    },

    getMovieDownloadState(streamUrl, onSuccess, onFail){
        var parameters = [
            {
                name: 'torrentUrl',
                value: streamUrl.replace(this.apiStreamUrl, '')
            }
        ];

        this.sendRequest(this.apiMovieDownloadStateUrl, parameters, false, onSuccess, onFail);
    },

    sendRequest(url, parameters, deserializeResult, onSuccess, onFail) {
        var xhttp = new XMLHttpRequest();
        xhttp.onreadystatechange = function () {
            if (this.readyState === 4) {
                if (this.status === 200) {
                    var result = deserializeResult ? JSON.parse(this.response) : this.response;
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