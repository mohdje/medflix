import BaseApiService from "./baseApiService";
import AppMode from "../appMode";

class TorrentApiService extends BaseApiService {
    buildTorrentUrl(url){
        return this.buildUrl('torrent/' + (url ? url : ''));
    }

    buildStreamUrl(url, fileName, seasonNumber, episodeNumber){
        if(fileName)
            return this.buildTorrentUrl('stream/file?url=' + url + '&fileName=' + fileName);
        else if(seasonNumber && episodeNumber)
            return this.buildTorrentUrl('stream/series?url=' + url + '&seasonNumber=' + seasonNumber + '&episodeNumber=' + episodeNumber);
        else
            return this.buildTorrentUrl('stream/movies?url=' + url);
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
                value: title
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
                value: title
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
        const streamUrlParams = new URLSearchParams(streamUrl.replace(this.buildTorrentUrl('stream/'+ AppMode.getActiveMode().urlKey)+'?',''));

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
  