

import MediasCategorieList from "../components/media/list/MediasCategorieList";
import AppServices from "../services/AppServices";


function MediasListGenrePage({ genre, loadFromCache, onMediaClick }) {

    return (
        <MediasCategorieList 
            categorie={genre} 
            loadFromCache={loadFromCache}
            searchOperation={(genreId, pageIndex, onSuccess, onFail) => AppServices.searchMediaService.getMediasByGenre(genreId, pageIndex, onSuccess, onFail)}
            onMediaClick={(mediaId) => onMediaClick(mediaId)}/>
    );
}

export default MediasListGenrePage;