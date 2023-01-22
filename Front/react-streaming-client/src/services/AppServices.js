import SearchMediaApiService from './api/searchMediaApiService';
import BookmarkApiService from './api/bookmarkApiService';
import WatchedMediaApiService from './api/watchedMediaApiService';

const AppServices = {
    init(){
        this.searchMediaService = new SearchMediaApiService();
        this.bookmarkApiService = new BookmarkApiService();
        this.watchedMediaApiService = new WatchedMediaApiService();
    }
}

export default AppServices;