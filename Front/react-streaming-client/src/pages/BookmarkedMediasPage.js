import MoviesAPI from "../services/moviesAPI.js";
import AppMode from "../services/appMode.js";
import MediasIntermediatePresentationList from "../components/movies/list/MediasIntermediatePresentationList";
import { useEffect, useState } from 'react';

var lastClickedMedia = null;

function BookmarkedMediasPage({ centerToLastClickedMedia, onMediaClick }) {

    const [mediasBookmarks, setMediasBookmarks] = useState([]);
    const [loadingVisible, setLoadingVisible] = useState(true);

    useEffect(() => {
        if(!centerToLastClickedMedia)
            lastClickedMedia = null;

        MoviesAPI.getBookmarkedMovies((bookmarkedMedias) => {
            setLoadingVisible(false);
            if (bookmarkedMedias && bookmarkedMedias.length > 0)
                setMediasBookmarks(bookmarkedMedias);
        }, () => {
            setLoadingVisible(false);
        });
    }, []);

    const handleMediaClick = (media) => {
        lastClickedMedia = media;
        onMediaClick(media.id);
    }

    const mediaLabel = AppMode.getActiveMode().label.toLowerCase();

    return (
        <MediasIntermediatePresentationList
            title={mediasBookmarks && mediasBookmarks.length > 0 ? "You can see here the " + mediaLabel + " you have bookmarked" : "No bookmarked " + mediaLabel }
            loadingProgressVisible={loadingVisible}
            medias={mediasBookmarks}
            centerToMedia={lastClickedMedia}
            onClick={(media) => handleMediaClick(media)}/>
    )
}


export default BookmarkedMediasPage;