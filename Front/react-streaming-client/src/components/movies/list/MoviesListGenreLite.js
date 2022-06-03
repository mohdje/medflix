import "../../../style/movies-list.css";
import MoviesAPI from "../../../js/moviesAPI.js";
import MoviesListLite from "./MoviesListLite";
import { useEffect, useState } from 'react';
import fadeTransition from "../../../js/customStyles.js";


const moviesListGenreLiteCache = [];

function MoviesListGenreLite({ genre, visible, onMovieClick}) {
    const [movies, setMovies] = useState([]);

    useEffect(() => {
        let cacheOfGenre = moviesListGenreLiteCache.find(c => c.genre === genre);

        if(cacheOfGenre){
            setMovies(cacheOfGenre.movies);
        }
        else{
            MoviesAPI.getLastMoviesByGenre(genre,
                (moviesOfGenre) => {
                    if (moviesOfGenre && moviesOfGenre.length > 0){
                        moviesListGenreLiteCache.push({
                            genre: genre,
                            movies: moviesOfGenre
                        });
                        setMovies(moviesOfGenre);
                    }
                       
                });
        }
        
    }, []);

    return (
        <div style={fadeTransition(movies && movies.length > 0  && visible, 1)} className="movies-list-genre-container">
            <div className="movies-list-header">
                <div className="movies-list-genre">{genre} movies selection</div>
            </div>
            <MoviesListLite movies={movies} onMovieClick={(movieId)=>onMovieClick(movieId)}/>
        </div>
    );
}
export default MoviesListGenreLite;