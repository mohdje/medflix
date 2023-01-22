import BaseApiService from "./baseApiService";
import AppMode from "../appMode";

class BookmarkApiService extends BaseApiService {
    buildSearchUrl(url){
      return this.buildUrl(AppMode.getActiveMode().urlKey + '/bookmarks/' + (url ? url : ''));
    }

    getBookmarkedMedias(onSuccess, onFail) {
        this.getRequest(this.buildSearchUrl(), [], true, onSuccess, onFail);
    }

    addBookmarkedMedia(mediaDto, onSuccess) {
        this.putRequest(this.buildSearchUrl(), mediaDto, onSuccess);
    }

    deleteBookmarkedMedia(mediaDto, onSuccess) {
        this.deleteRequest(this.buildSearchUrl(), mediaDto, onSuccess);
    }

    isMediaBookmarked(mediaId, onSuccess, onFail) {
        var parameters = [
            {
                name: 'id',
                value: mediaId
            }
        ];
        this.getRequest(this.buildSearchUrl('exists'), parameters, false, onSuccess, onFail);
    }
}

export default BookmarkApiService;
  