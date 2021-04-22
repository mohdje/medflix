import MoviesAPI from "../../../js/moviesAPI.js";
import MoviesIntermediatePresentationList from "../list/MoviesIntermediatePresentationList";
import { useEffect, useState } from 'react';

var lastSeenMovieLastClicked = null;

function LastSeenMoviesView({centerToLastClickedMovie, onMoreClick }) {

    const [moviesBookmarks, setMoviesBookmarks] = useState([]);
    const [loadingVisible, setLoadingVisible] = useState(true);

    useEffect(() => {
        if(!centerToLastClickedMovie)
            lastSeenMovieLastClicked = null;

        MoviesAPI.getLastSeenMovies((lastSeenMovies) => {
            setLoadingVisible(false);
            if (lastSeenMovies && lastSeenMovies.length > 0)
                setMoviesBookmarks(lastSeenMovies);
        }, () => {
            setLoadingVisible(false);
        });
    }, []);

    const handleMoreClick = (movieBookmark) => {
        lastSeenMovieLastClicked = movieBookmark;
        onMoreClick(movieBookmark.movie.id);
    }

    return (
        <MoviesIntermediatePresentationList
            title={"You can see here the last movies you played"}
            loadingProgressVisible={loadingVisible}
            moviesBookmarks={moviesBookmarks}
            centerToMovieBookmark={lastSeenMovieLastClicked}
            onMoreClick={(movieBookmark) => handleMoreClick(movieBookmark)} />
    )
}


export default LastSeenMoviesView;