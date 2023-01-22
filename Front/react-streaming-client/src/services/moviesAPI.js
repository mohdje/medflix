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

   
 




    
    playWithVlc(streamUrl, onSuccess, onFail){

        var parameters = [
            {
                name: 'data',
                value: btoa(streamUrl)
            }
        ];

        this.sendRequest(this.apiApplicationUrl('startvlc'), parameters, false, onSuccess, onFail);
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