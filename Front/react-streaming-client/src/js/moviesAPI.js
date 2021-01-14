const MoviesAPI = {
    apiBaseUrl: 'https://localhost:5001/movies/',
    apiSubtitlesUrl: 'https://localhost:5001/movies/subtitles/',
    apiStreamUrl: 'https://localhost:5001/movies/stream?url=',


    getMoviesGenres(onSuccess, onFail){
        var url = this.apiBaseUrl + 'genres';
        this.sendRequest(url, [], onSuccess, onFail);
    },

    getLastMoviesByGenre(genre, onSuccess, onFail) {
        var url = this.apiBaseUrl + 'genre/' + genre;
        this.sendRequest(url, [], onSuccess, onFail);
    },

    getSuggestedMovies(onSuccess, onFail) {
        var url = this.apiBaseUrl + 'suggested'
        this.sendRequest(url, [], onSuccess, onFail);
    },

    getMoviesByGenre(genre, page, onSuccess, onFail) {
        var url = this.apiBaseUrl + 'genre/' + genre + '/' + page;
        this.sendRequest(url, [], onSuccess, onFail);
    },
    getMovieDetails(id, onSuccess, onFail) {
        var url = this.apiBaseUrl + 'details/' + id;
        this.sendRequest(url, [], onSuccess, onFail);
    },

    searchMovies(text, onSuccess, onFail) {
        var url = this.apiBaseUrl + 'search/' + text
        this.sendRequest(url, [], onSuccess, onFail);
    },

    getAvailableSubtitles(imdbId, onSuccess, onFail){
        var url = this.apiSubtitlesUrl + 'available/' + imdbId;
        this.sendRequest(url, [], onSuccess, onFail);
    },

    getAvailableMovieServices(onSuccess, onFail){
        var url = this.apiBaseUrl + 'services';
        this.sendRequest(url, [], onSuccess, onFail);
    },

    changeMovieService(serviceName, onSuccess, onFail){
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
        xhttp.open("POST", this.apiBaseUrl + 'services', true);
        xhttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        xhttp.send("serviceName="+ serviceName);
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