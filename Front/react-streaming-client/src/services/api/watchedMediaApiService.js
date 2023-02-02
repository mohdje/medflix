import BaseApiService from "./baseApiService";
import AppMode from "../appMode";

class WatchedMediaApiService extends BaseApiService {
    buildWatchedMediaUrl(url){
      return this.buildUrl(AppMode.getActiveMode().urlKey + '/watchedmedia/' + (url ? url : ''));
    }

    getWatchedMedias(onSuccess, onFail) {
        this.getRequest(this.buildWatchedMediaUrl(), [], true, onSuccess, onFail);
    }

    getWatchedMedia(mediaId, seasonNumber, episodeNumber, onSuccess, onFail) {
        const parameters = [
            {
                name: 'seasonNumber',
                value: seasonNumber
            },
            {
                name: 'episodeNumber',
                value: episodeNumber
            }
        ]

        this.getRequest(this.buildWatchedMediaUrl(mediaId), parameters, true, onSuccess, onFail);
    }

    getWatchedEpisodes(mediaId, seasonNumber, onSuccess, onFail) {
        this.getRequest(this.buildWatchedMediaUrl(mediaId + '/' + seasonNumber), [], true, onSuccess, onFail);
    }

    saveWacthedMedia(mediaDetails, currentTime, duration, seasonNumber, episodeNumber) {
        const watchedMedia = {... mediaDetails};
        watchedMedia.currentTime = currentTime ? currentTime : 0;
        watchedMedia.totalDuration = duration ? duration : 0;

        if(AppMode.getActiveMode().urlKey === 'series'){
            watchedMedia.seasonNumber = seasonNumber;
            watchedMedia.episodeNumber = episodeNumber;
        }

        this.putRequest(this.buildWatchedMediaUrl(), watchedMedia);
    }
}

export default WatchedMediaApiService;
  