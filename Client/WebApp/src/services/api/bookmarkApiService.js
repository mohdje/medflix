import BaseApiService from "./baseApiService";
import AppMode from "../appMode";

class BookmarkApiService extends BaseApiService {
    buildBookmarkUrl(url){
      return this.buildUrl(AppMode.getActiveMode().urlKey + '/bookmarks/' + (url ? url : ''));
    }

    getBookmarkedMedias(onSuccess, onFail) {
        this.getRequest(this.buildBookmarkUrl(), [], true, onSuccess, onFail);
    }

    addBookmarkedMedia(mediaDto, onSuccess) {
        this.putRequest(this.buildBookmarkUrl(), mediaDto, onSuccess);
    }

    deleteBookmarkedMedia(mediaDto, onSuccess) {
        this.deleteRequest(this.buildBookmarkUrl(), mediaDto, onSuccess);
    }

    isMediaBookmarked(mediaId, onSuccess, onFail) {
        var parameters = [
            {
                name: 'id',
                value: mediaId
            }
        ];
        this.getRequest(this.buildBookmarkUrl('exists'), parameters, false, onSuccess, onFail);
    }
}

export default BookmarkApiService;
  