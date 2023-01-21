import MoviesAPI from "../js/moviesAPI.js";
import MediasIntermediatePresentationList from "../components/movies/list/MediasIntermediatePresentationList";
import { useEffect, useState } from 'react';
import AppMode from "../js/appMode.js";

var watchedMediaLastClicked = null;

function WatchedMediasPage({centerToLastClickedMedia, onMediaClick }) {

    const [medias, setMedias] = useState([]);
    const [loadingVisible, setLoadingVisible] = useState(true);

    useEffect(() => {
        if(!centerToLastClickedMedia)
            watchedMediaLastClicked = null;

        MoviesAPI.getWatchedMovies((medias) => {
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