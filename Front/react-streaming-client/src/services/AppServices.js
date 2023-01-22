import SearchMediaApiService from './api/searchMediaApiService';
import BookmarkApiService from './api/bookmarkApiService';
import WatchedMediaApiService from './api/watchedMediaApiService';
import TorrentApiService from './api/torrentApiService';
import SubtitlesApiService from './api/subtitlesApiService';
import AppInfoApiService from './api/appInfoApiService';

const AppServices = {
    init(){
        this.searchMediaService = new SearchMediaApiService();
        this.bookmarkApiService = new BookmarkApiService();
        this.watchedMediaApiService = new WatchedMediaApiService();
        this.torrentApiService = new TorrentApiService();
        this.subtitlesApiService = new SubtitlesApiService();
        this.appInfoApiService = new AppInfoApiService();
    }
}

export default AppServices;