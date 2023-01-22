import BaseApiService from "./baseApiService";

class TorrentApiService extends BaseApiService {
    buildTorrentUrl(url){
        return this.buildUrl('torrent/' + (url ? url : ''));
    }

    buildStreamUrl(url, fileName){
        return this.buildTorrentUrl('stream?url=' + url + (Boolean(fileName) ? '&fileName=' + fileName : ''));
    }

    searchVFSources(movieId, movieTitle, movieYear, onSuccess, onFail) {
        var url = this.buildTorrentUrl('vf');
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

        this.getRequest(url, parameters, true, onSuccess, onFail);
    }

    searchVOSources(movieTitle, movieYear, onSuccess, onFail) {
        var url = this.buildTorrentUrl('vo');
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

        this.getRequest(url, parameters, true, onSuccess, onFail);
    }

    getDownloadState(streamUrl, onSuccess, onFail) {
        const streamUrlParams = new URLSearchParams(streamUrl.replace(this.buildTorrentUrl('stream')+'?',''));
        var parameters = [
            {
                name: 'torrentUrl',
                value: streamUrlParams.get('url')
            }
        ];

        this.getRequest(this.buildTorrentUrl('streamdownloadstate'), parameters, true, onSuccess, onFail);
    }

    getTorrentFiles(torrentUrl, onSuccess, onFail){
        var parameters = [
            {
                name: 'torrentUrl',
                value: torrentUrl
            }
        ];

        this.getRequest(this.buildTorrentUrl('files'), parameters, true, onSuccess, onFail);
    }

    getTorrentHistory(onSuccess, onFail) {
        this.getRequest(this.buildTorrentUrl('history'), null, true, onSuccess, onFail);
    }
}

export default TorrentApiService;
  