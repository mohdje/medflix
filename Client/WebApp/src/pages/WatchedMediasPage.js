import AppServices from "../services/AppServices";
import MediasIntermediatePresentationList from "../components/media/list/MediasIntermediatePresentationList";
import { useEffect, useState } from 'react';
import AppMode from "../services/appMode.js";

var watchedMediaLastClicked = null;

function WatchedMediasPage({centerToLastClickedMedia, onMediaClick }) {

    const [medias, setMedias] = useState([]);
    const [loadingVisible, setLoadingVisible] = useState(true);

    useEffect(() => {
        if(!centerToLastClickedMedia)
            watchedMediaLastClicked = null;

            AppServices.watchedMediaApiService.getWatchedMedias((medias) => {
            setLoadingVisible(false);
            if (medias && medias.length > 0)
                setMedias(medias);
        }, () => {
            setLoadingVisible(false);
        });
    }, []);

    const handleClick = (media) => {
        watchedMediaLastClicked = media;
        onMediaClick(media.id);
    }

    const mediaLabel = AppMode.getActiveMode().label.toLowerCase();

    return (
        <MediasIntermediatePresentationList
            title={medias && medias.length > 0 ? "You can see here the " + mediaLabel + " you have watched" : "No " + mediaLabel + " watched"}
            loadingProgressVisible={loadingVisible}
            medias={medias}
            centerToMedia={watchedMediaLastClicked}
            onClick={(media) => handleClick(media)} />
    )
}


export default WatchedMediasPage;