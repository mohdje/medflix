import SearchMediaApiService from './api/searchMediaApiService';
import BookmarkApiService from './api/bookmarkApiService';

const AppServices = {
    init(){
        this.searchMediaService = new SearchMediaApiService();
        this.bookmarkApiService = new BookmarkApiService();
    }
}

export default AppServices;