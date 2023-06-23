

import MediasCategorieList from "../components/media/list/MediasCategorieList";
import AppServices from "../services/AppServices";


function MediasListGenrePage({ platform, loadFromCache, onMediaClick }) {

    return (
        <MediasCategorieList 
            categorie={platform} 
            loadFromCache={loadFromCache}
            searchOperation={(platformId, pageIndex, onSuccess, onFail) => AppServices.searchMediaService.getMediasByPlatform(platformId, pageIndex, onSuccess, onFail)}
            onMediaClick={(mediaId) => onMediaClick(mediaId)}/>
    );
}

export default MediasListGenrePage;