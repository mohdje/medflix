import BaseApiService from "./baseApiService";
import AppMode from "../appMode";

class WatchedMediaApiService extends BaseApiService {
    buildSearchUrl(url){
      return this.buildUrl(AppMode.getActiveMode().urlKey + '/watchedmedia/' + (url ? url : ''));
    }

    getWatchedMedias(onSuccess, onFail) {
        this.getRequest(this.buildSearchUrl(), [], true, onSuccess, onFail);
    }

    getWatchedMedia(mediaId, onSuccess, onFail) {
        this.getRequest(this.buildSearchUrl(mediaId), [], true, onSuccess, onFail);
    }

    saveWacthedMedia(watchedMedia) {
        this.putRequest(this.buildSearchUrl(), watchedMedia);
    }
}

export default WatchedMediaApiService;
  