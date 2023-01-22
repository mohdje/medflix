import SearchMediaApiService from './api/searchMediaApiService';

const AppServices = {
    init(){
        this.searchMediaService = new SearchMediaApiService();
    }
}

export default AppServices;