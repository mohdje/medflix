import SearchMediaApiService from './api/searchMediaApiService';
import BookmarkApiService from './api/bookmarkApiService';
import WatchedMediaApiService from './api/watchedMediaApiService';
import TorrentApiService from './api/torrentApiService';

const AppServices = {
    init(){
        this.searchMediaService = new SearchMediaApiService();
        this.bookmarkApiService = new BookmarkApiService();
        this.watchedMediaApiService = new WatchedMediaApiService();
        this.torrentApiService = new TorrentApiService();
    }
}

export default AppServices;