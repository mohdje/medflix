import AppMode from "./appMode";

const MoviesAPI = {
    apiUrl(route) { return 'http://localhost:5000/' + route },
    apiMoviesUrl(route) { return this.apiUrl( this.getActiveMode() + '/' + route) },
    apiTorrentUrl(route) { return this.apiUrl( 'torrent/' + route) },
    apiWatchedMoviesUrl() { return this.apiMoviesUrl('watchedmedia') },
    apiWatchedMovieUrl(id) { return this.apiWatchedMoviesUrl() + '/' + id },
    apiBookmarkedMoviesUrl(route) { return route ? this.apiMoviesUrl('bookmarks/' + route)  : this.apiMoviesUrl('bookmarks')},
    apiStreamUrl(url, fileName) { return this.apiTorrentUrl('stream?url=' + url + (Boolean(fileName) ? '&fileName=' + fileName : ''))},
    apiSubtitlesUrl(route) { return this.apiUrl('subtitles/' + route) },
    apiServicesUrl(route) { return this.apiUrl('services/' + route) },
    apiApplicationUrl(route) { return this.apiUrl('application/' + route) },

    getActiveMode(){
        return AppMode.getActiveMode().urlKey;
    },
    getMoviesGenres(onSuccess, onFail) {
        var url = this.apiMoviesUrl('genres');
        this.sendRequest(url, [], true, onSuccess, onFail);
    },

    getPopularMoviesByGenre(genreId, onSuccess, onFail) {
        var url = this.apiMoviesUrl('genre/' + genreId);
        this.sendRequest(url, [], true, onSuccess, onFail);
    },

    getMoviesOfToday(onSuccess, onFail) {
        var url = this.apiMoviesUrl('moviesoftoday')
        this.sendRequest(url, [], true, onSuccess, onFail);
    },

    getMoviesByGenre(genreId, page, onSuccess, onFail) {
        var url = this.apiMoviesUrl('genre/' + genreId + '/' + page);
        this.sendRequest(url, [], true, onSuccess, onFail);
    },
    getMovieDetails(id, onSuccess, onFail) {
        var url = this.apiMoviesUrl('details/' + id);
        this.sendRequest(url, [], true, onSuccess, onFail);
    },

    searchMovies(text, onSuccess, onFail) {
        var url = this.apiMoviesUrl('search/' + text);
        this.sendRequest(url, [], true, onSuccess, onFail);
    },

    getPopularNetflixMovies(onSuccess, onFail) {
        var url = this.apiMoviesUrl('netflix');
        this.sendRequest(url, [], true, onSuccess, onFail);
    },

    getPopularDisneyPlusMovies(onSuccess, onFail) {
        var url = this.apiMoviesUrl('disneyplus');
        this.sendRequest(url, [], true, onSuccess, onFail);
    },

    getPopularAmazonPrimeMovies(onSuccess, onFail) {
        var url = this.apiMoviesUrl('amazonprime');
        this.sendRequest(url, [], true, onSuccess, onFail);
    },

    getPopularMovies(onSuccess, onFail) {
        var url = this.apiMoviesUrl('popular');
        this.sendRequest(url, [], true, onSuccess, onFail);
    },

    getRecommandedMovies(movieId, onSuccess, onFail) {
        var url = this.apiMoviesUrl('recommandations');
        var parameters = [
            {
                name: 'id',
                value: movieId ? movieId : ''
            }
        ];
        this.sendRequest(url, parameters, true, onSuccess, onFail);
    },

    getEpisodes(serieId, seasonNumber, onSuccess, onFail){
        var url = this.apiMoviesUrl('episodes/'+ serieId + '/' + seasonNumber);
        this.sendRequest(url, [], true, onSuccess, onFail);
    },

    searchVFSources(movieId, movieTitle, movieYear, onSuccess, onFail) {
        var url = this.apiTorrentUrl('vf');
        var parameters = [
            {
                name: 'movieId',
                value: movieId
            },
            {
                name: 'originalTitle',
                value: movieTitle
            },
            {
                name: 'year',
                value: movieYear
            }
        ];

        this.sendRequest(url, parameters, true, onSuccess, onFail);
    },

    searchVOSources(movieTitle, movieYear, onSuccess, onFail) {
        var url = this.apiTorrentUrl('vo');
        var parameters = [
            {
                name: 'title',
                value: movieTitle
            },
            {
                name: 'year',
                value: movieYear
            }
        ];

        this.sendRequest(url, parameters, true, onSuccess, onFail);
    },

    getAvailableSubtitles(imdbId, onSuccess, onFail) {
        var url = this.apiSubtitlesUrl('available/' + imdbId);
        this.sendRequest(url, [], true, onSuccess, onFail);
    },

    getSubtitlesApiUrl(subtitlesSourceUrl) {
        return this.apiSubtitlesUrl("?sourceUrl=" + subtitlesSourceUrl);
    },

    getWatchedMovies(onSuccess, onFail) {
        this.sendRequest(this.apiWatchedMoviesUrl(), [], true, onSuccess, onFail);
    },

    getWatchedMovie(movieId, onSuccess, onFail) {
        this.sendRequest(this.apiWatchedMovieUrl(movieId), [], true, onSuccess, onFail);
    },

    saveWacthedMovie(watchedMovie) {
        var xhttp = new XMLHttpRequest();
        xhttp.open("PUT", this.apiWatchedMoviesUrl(), true);
        xhttp.setRequestHeader("Content-Type", "application/json");
        xhttp.send(JSON.stringify(watchedMovie));
    },

    getBookmarkedMovies(onSuccess, onFail) {
        this.sendRequest(this.apiBookmarkedMoviesUrl(), [], true, onSuccess, onFail);
    },

    addBookmarkedMovie(movie, onSuccess) {
        var xhttp = new XMLHttpRequest();
        xhttp.open("PUT", this.apiBookmarkedMoviesUrl(), true);
        xhttp.setRequestHeader("Content-Type", "application/json");
        xhttp.send(JSON.stringify(movie));

        xhttp.onreadystatechange = function () {
            if (this.readyState === 4)
                if (this.status === 200)
                    onSuccess();
        }
    },

    deleteBookmarkedMovie(movie, onSuccess) {
        var xhttp = new XMLHttpRequest();
        xhttp.open("DELETE", this.apiBookmarkedMoviesUrl(), true);
        xhttp.setRequestHeader("Content-Type", "application/json");
        xhttp.send(JSON.stringify(movie));

        xhttp.onreadystatechange = function () {
            if (this.readyState === 4)
                if (this.status === 200)
                    onSuccess();
        }
    },

    isMovieBookmarked(movieId, onSuccess, onFail) {
        var parameters = [
            {
                name: 'movieId',
                value: movieId
            }
        ];
        this.sendRequest(this.apiBookmarkedMoviesUrl('exists'), parameters, false, onSuccess, onFail);
    },

    getMovieDownloadState(streamUrl, onSuccess, onFail) {
        const streamUrlParams = new URLSearchParams(streamUrl.replace(this.apiTorrentUrl('stream')+'?',''));
        var parameters = [
            {
                name: 'torrentUrl',
                value: streamUrlParams.get('url')
            }
        ];

        this.sendRequest(this.apiTorrentUrl('streamdownloadstate'), parameters, true, onSuccess, onFail);
    },

    getTorrentHistory(onSuccess, onFail) {
        this.sendRequest(this.apiTorrentUrl('history'), null, true, onSuccess, onFail);
    },

    playWithVlc(streamUrl, onSuccess, onFail){

        var parameters = [
            {
                name: 'data',
                value: btoa(streamUrl)
            }
        ];

        this.sendRequest(this.apiApplicationUrl('startvlc'), parameters, false, onSuccess, onFail);
    },

    getTorrentFiles(torrentUrl, onSuccess, onFail){
        var parameters = [
            {
                name: 'torrentUrl',
                value: torrentUrl
            }
        ];

        this.sendRequest(this.apiTorrentUrl('files'), parameters, true, onSuccess, onFail);
    },

    isDesktopApplication(onSuccess, onFail) {
        if (this.isDesktopApp) onSuccess(this.isDesktopApp);
        else {
            const self = this;
            this.sendRequest(self.apiApplicationUrl('platform'), null, true, (response) => {
                self.isDesktopApp = response.isDesktopApplication;
                onSuccess(response.isDesktopApplication);
            }, onFail);
        }
    },

    sendRequest(url, parameters, deserializeResult, onSuccess, onFail) {
        var xhttp = new XMLHttpRequest();
        xhttp.onreadystatechange = function () {
            if (this.readyState === 4) {
                if (this.status === 200) {
                    var result = deserializeResult && this.response ? JSON.parse(this.response) : this.response;
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
    },

    performPost(url, parameters, onSuccess, onFail) {
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
        xhttp.open("POST", url, true);
        xhttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        xhttp.send(parameters);
    }
}

export default MoviesAPI;