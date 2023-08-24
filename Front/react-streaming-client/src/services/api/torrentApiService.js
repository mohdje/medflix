import BaseApiService from "./baseApiService";
import AppMode from "../appMode";

class TorrentApiService extends BaseApiService {
    buildTorrentUrl(url) {
        return this.buildUrl('torrent/' + (url ? url : ''));
    }

    buildStreamUrl(url, fileName, seasonNumber, episodeNumber) {
        const base64Url = window.btoa(url);

        if (fileName)
            return this.buildTorrentUrl('stream/file?base64Url=' + base64Url + '&fileName=' + fileName);
        else if (AppMode.getActiveMode().urlKey === "series")
            return this.buildTorrentUrl('stream/series?base64Url=' + base64Url + '&seasonNumber=' + seasonNumber + '&episodeNumber=' + episodeNumber);
        else
            return this.buildTorrentUrl('stream/movies?base64Url=' + base64Url);
    }

    searchVFSources(mediaId, title, year, seasonNumber, episodeNumber, onSuccess, onFail) {
        var url = this.buildTorrentUrl(AppMode.getActiveMode().urlKey + '/vf');

        var parameters = [
            {
                name: 'mediaId',
                value: mediaId
            },
            {
                name: 'title',
                value: encodeURIComponent(title)
            },
            {
                name: 'year',
                value: year
            },
            {
                name: 'season',
                value: seasonNumber
            },
            {
                name: 'episode',
                value: episodeNumber
            },
        ];
        this.getRequest(url, parameters, true, onSuccess, onFail);
    }

    searchVOSources(title, year, imdbid, seasonNumber, episodeNumber, onSuccess, onFail) {
        var url = this.buildTorrentUrl(AppMode.getActiveMode().urlKey + '/vo');
        var parameters = [
            {
                name: 'title',
                value: encodeURIComponent(title)
            },
            {
                name: 'year',
                value: year
            },
            {
                name: 'season',
                value: seasonNumber
            },
            {
                name: 'episode',
                value: episodeNumber
            },
            {
                name: 'imdbid',
                value: imdbid
            }
        ];

        this.getRequest(url, parameters, true, onSuccess, onFail);
    }

    getDownloadState(streamUrl, onSuccess, onFail) {
        const url = new URL(streamUrl);

        var parameters = [
            {
                name: 'base64TorrentUrl',
                value: url.searchParams.get('base64Url')
            }
        ];

        this.getRequest(this.buildTorrentUrl('streamdownloadstate'), parameters, true, onSuccess, onFail);
    }

    getTorrentFiles(torrentUrl, onSuccess, onFail) {
        const base64Url = window.btoa(torrentUrl);

        var parameters = [
            {
                name: 'base64torrentUrl',
                value: base64Url
            }
        ];

        this.getRequest(this.buildTorrentUrl('files'), parameters, true, onSuccess, onFail);
    }

    getTorrentHistory(onSuccess, onFail) {
        this.getRequest(this.buildTorrentUrl('history'), null, true, onSuccess, onFail);
    }
}

export default TorrentApiService;
