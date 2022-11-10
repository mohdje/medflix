import MoviesAPI from "../js/moviesAPI.js";
import MoviesIntermediatePresentationList from "../components/movies/list/MoviesIntermediatePresentationList";
import { useEffect, useState } from 'react';

var lastClickedMovie = null;

function BookmarkedMoviesView({ centerToLastClickedMovie, onMovieClick }) {

    const [moviesBookmarks, setMoviesBookmarks] = useState([]);
    const [loadingVisible, setLoadingVisible] = useState(true);

    useEffect(() => {
        if(!centerToLastClickedMovie)
            lastClickedMovie = null;

        MoviesAPI.getBookmarkedMovies((bookmarkedMovies) => {
            setLoadingVisible(false);
            if (bookmarkedMovies && bookmarkedMovies.length > 0)
                setMoviesBookmarks(bookmarkedMovies);
        }, () => {
            setLoadingVisible(false);
        });
    }, []);

    const handleMovieClick = (movie) => {
        lastClickedMovie = movie;
        onMovieClick(movie.id);
    }

    return (
        <MoviesIntermediatePresentationList
            title={moviesBookmarks && moviesBookmarks.length > 0 ? "You can see here the movies you have bookmarked" : "No bookmarked movies"}
            loadingProgressVisible={loadingVisible}
            movies={moviesBookmarks}
            centerToMovie={lastClickedMovie}
            onClick={(movie) => handleMovieClick(movie)}/>
    )
}


export default BookmarkedMoviesView;