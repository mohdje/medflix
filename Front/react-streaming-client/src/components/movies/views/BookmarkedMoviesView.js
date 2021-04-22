import MoviesAPI from "../../../js/moviesAPI.js";
import MoviesIntermediatePresentationList from "../list/MoviesIntermediatePresentationList";
import { useEffect, useState } from 'react';

var bookmarkedMovieLastClicked = null;

function BookmarkedMoviesView({ centerToLastClickedBookmark, onMoreClick }) {

    const [moviesBookmarks, setMoviesBookmarks] = useState([]);
    const [loadingVisible, setLoadingVisible] = useState(true);

    useEffect(() => {
        if(!centerToLastClickedBookmark)
            bookmarkedMovieLastClicked = null;

        MoviesAPI.getBookmarkedMovies((bookmarkedMovies) => {
            setLoadingVisible(false);
            if (bookmarkedMovies && bookmarkedMovies.length > 0)
                setMoviesBookmarks(bookmarkedMovies);
        }, () => {
            setLoadingVisible(false);
        });
    }, []);

    const deleteBookmark = (movieBookmark) => {
        MoviesAPI.deleteBookmarkedMovie(movieBookmark, () => {
            var newMovieBookmarksList = moviesBookmarks.filter(m => !(m.movie.id === movieBookmark.movie.id && m.serviceName === movieBookmark.serviceName));
            setMoviesBookmarks(newMovieBookmarksList);
        });
    };

    const handleMoreClick = (movieBookmark) => {
        bookmarkedMovieLastClicked = movieBookmark;
        onMoreClick(movieBookmark.movie.id);
    }

    return (
        <MoviesIntermediatePresentationList
            title={"You can see here the movies you have bookmarked"}
            loadingProgressVisible={loadingVisible}
            moviesBookmarks={moviesBookmarks}
            showDeleteButton={true}
            centerToMovieBookmark={bookmarkedMovieLastClicked}
            onMoreClick={(movieBookmark) => handleMoreClick(movieBookmark)}
            onDeleteClick={(movieBookmark) => deleteBookmark(movieBookmark)} />
    )
}


export default BookmarkedMoviesView;