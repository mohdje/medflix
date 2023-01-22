import BaseApiService from "./baseApiService";
import AppMode from "../appMode";

class WatchedMediaApiService extends BaseApiService {
    buildWatchedMediaUrl(url){
      return this.buildUrl(AppMode.getActiveMode().urlKey + '/watchedmedia/' + (url ? url : ''));
    }

    getWatchedMedias(onSuccess, onFail) {
        this.getRequest(this.buildWatchedMediaUrl(), [], true, onSuccess, onFail);
    }

    getWatchedMedia(mediaId, onSuccess, onFail) {
        this.getRequest(this.buildWatchedMediaUrl(mediaId), [], true, onSuccess, onFail);
    }

    saveWacthedMedia(watchedMedia) {
        this.putRequest(this.buildWatchedMediaUrl(), watchedMedia);
    }
}

export default WatchedMediaApiService;
  