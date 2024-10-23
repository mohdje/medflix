import BaseApiService from "./baseApiService";
import AppMode from "../appMode";

class SubtitlesApiService extends BaseApiService {
    buildSubtitlesUrl(url){
        return this.buildUrl('subtitles/' + (url ? url : ''));
    }

    getAvailableSubtitles(imdbCode, seasonNumber, episodeNumber, onSuccess, onFail) {
        var url = this.buildSubtitlesUrl(AppMode.getActiveMode().urlKey);
        var parameters = [
            {
                name: 'imdbCode',
                value: imdbCode
            },
            {
                name: 'seasonNumber',
                value: seasonNumber
            },
            {
                name: 'episodeNumber',
                value: episodeNumber
            }
        ];
        this.getRequest(url, parameters, true, onSuccess, onFail);
    }
    
    getSubtitlesApiUrl(subtitlesSourceUrl) {
        return this.buildSubtitlesUrl("?sourceUrl=" + subtitlesSourceUrl);
    }
}

export default SubtitlesApiService;
  