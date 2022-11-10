import "../../../style/movies-list.css";
import MoviesListLite from "./MoviesListLite";
import fadeTransition from "../../../js/customStyles.js";

function MoviesListLiteWithTitle({ listTitle, movies, visible, onMovieClick}) {
    return (
        <div style={fadeTransition(movies && movies.length > 0  && visible, 1)} className="movies-list-genre-container">
            <div className="movies-list-header">
                <div className="movies-list-genre">{listTitle}</div>
            </div>
            <MoviesListLite movies={movies} onMovieClick={(movieId)=>onMovieClick(movieId)}/>
        </div>
    );
}
export default MoviesListLiteWithTitle;

