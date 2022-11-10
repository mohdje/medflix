import MoviesAPI from "../js/moviesAPI.js";
import MoviesIntermediatePresentationList from "../components/movies/list/MoviesIntermediatePresentationList";
import { useEffect, useState } from 'react';

var watchedMovieLastClicked = null;

function LastSeenMoviesView({centerToLastClickedMovie, onMovieClick }) {

    const [movies, setMovies] = useState([]);
    const [loadingVisible, setLoadingVisible] = useState(true);

    useEffect(() => {
        if(!centerToLastClickedMovie)
            watchedMovieLastClicked = null;

        MoviesAPI.getWatchedMovies((movies) => {
            setLoadingVisible(false);
            if (movies && movies.length > 0)
                setMovies(movies);
        }, () => {
            setLoadingVisible(false);
        });
    }, []);

    const handleClick = (movie) => {
        watchedMovieLastClicked = movie;
        onMovieClick(movie.id);
    }

    return (
        <MoviesIntermediatePresentationList
            title={movies && movies.length > 0 ? "You can see here the movies you have watched" : "No movies watched"}
            loadingProgressVisible={loadingVisible}
            movies={movies}
            centerToMovie={watchedMovieLastClicked}
            onClick={(movie) => handleClick(movie)} />
    )
}


export default LastSeenMoviesView;