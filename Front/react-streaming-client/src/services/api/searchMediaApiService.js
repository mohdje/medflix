import BaseApiService from "./baseApiService";
import AppMode from "../appMode";

class SearchMediaApiService extends BaseApiService {
    buildSearchUrl(url){
      return this.buildUrl(AppMode.getActiveMode().urlKey + '/' + url);
    }

    getMediaGenres(onSuccess, onFail) {
      var url = this.buildSearchUrl('genres');
      this.getRequest(url, [], true, onSuccess, onFail);
    }

    getPopularMediasByGenre(genreId, onSuccess, onFail) {
      var url = this.buildSearchUrl('genre/' + genreId);
      this.getRequest(url, [], true, onSuccess, onFail);
    }

    getMediasOfToday(onSuccess, onFail) {
        var url = this.buildSearchUrl('mediasoftoday')
        this.getRequest(url, [], true, onSuccess, onFail);
    }

    getMediasByGenre(genreId, page, onSuccess, onFail) {
        var url = this.buildSearchUrl('genre/' + genreId + '/' + page);
        this.getRequest(url, [], true, onSuccess, onFail);
    }

    getMediaDetails(id, onSuccess, onFail) {
        var url = this.buildSearchUrl('details/' + id);
        this.getRequest(url, [], true, onSuccess, onFail);
    }

    searchMedias(text, onSuccess, onFail) {
        var url = this.buildSearchUrl('search');
        var parameters = [
            {
                name: 't',
                value: text
            }
        ]
        this.getRequest(url, parameters, true, onSuccess, onFail);
    }

    getPopularNetflixMedias(onSuccess, onFail) {
        var url = this.buildSearchUrl('netflix');
        this.getRequest(url, [], true, onSuccess, onFail);
    }

    getPopularDisneyPlusMedias(onSuccess, onFail) {
        var url = this.buildSearchUrl('disneyplus');
        this.getRequest(url, [], true, onSuccess, onFail);
    }

    getPopularAmazonPrimeMedias(onSuccess, onFail) {
        var url = this.buildSearchUrl('amazonprime');
        this.getRequest(url, [], true, onSuccess, onFail);
    }

    getPopularAppleTvMedias(onSuccess, onFail) {
        var url = this.buildSearchUrl('appletv');
        this.getRequest(url, [], true, onSuccess, onFail);
    }

    getPopularMedias(onSuccess, onFail) {
        var url = this.buildSearchUrl('popular');
        this.getRequest(url, [], true, onSuccess, onFail);
    }

    getRecommandedMedias(onSuccess, onFail) {
        var url = this.buildSearchUrl('recommandations');
        this.getRequest(url, [], true, onSuccess, onFail);
    }

    getSimilarMedias(mediaId, onSuccess, onFail) {
        var url = this.buildSearchUrl('similar/'+ mediaId);
        this.getRequest(url, [], true, onSuccess, onFail);
    }

    getEpisodes(serieId, seasonNumber, onSuccess, onFail){
        var url = this.buildSearchUrl('episodes/'+ serieId + '/' + seasonNumber);
        this.getRequest(url, [], true, onSuccess, onFail);
    }

}

export default SearchMediaApiService;
  