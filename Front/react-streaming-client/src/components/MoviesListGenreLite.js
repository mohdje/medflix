import "../style/movies-list.css";
import MoviesAPI from "../js/moviesAPI.js";
import MoviesListLite from "./MoviesListLite";
import { useEffect, useState } from 'react';
import fadeTransition from "../js/customStyles.js";

function MoviesListGenreLite({ genre, visible, onMoreClick, onMovieClick}) {
    const [movies, setMovies] = useState([]);

    useEffect(() => {
        MoviesAPI.getLastMoviesByGenre(genre,
            (moviesOfGenre) => {
                if (moviesOfGenre && moviesOfGenre.length > 0)
                    setMovies(moviesOfGenre);
            },
            () => console.log('fail'));
    }, []);

    return (
        <div style={fadeTransition(movies && movies.length > 0  && visible, 1)} className="movies-list-genre-container">
            <div className="movies-list-header">
                <div className="movies-list-genre">{genre}</div>
                <div className="standard-button red" onClick={() => onMoreClick()}>More</div>
            </div>
            <MoviesListLite movies={movies} onMovieClick={(movieId)=>onMovieClick(movieId)}/>
        </div>
    );
}
export default MoviesListGenreLite;