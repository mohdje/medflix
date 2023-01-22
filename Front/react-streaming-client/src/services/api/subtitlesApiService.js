import BaseApiService from "./baseApiService";

class SubtitlesApiService extends BaseApiService {
    buildSubtitlesUrl(url){
        return this.buildUrl('subtitles/' + (url ? url : ''));
    }

    getAvailableSubtitles(imdbId, onSuccess, onFail) {
        var url = this.buildSubtitlesUrl('available/' + imdbId);
        this.getRequest(url, [], true, onSuccess, onFail);
    }
    
    getSubtitlesApiUrl(subtitlesSourceUrl) {
        return this.buildSubtitlesUrl("?sourceUrl=" + subtitlesSourceUrl);
    }
}

export default SubtitlesApiService;
  