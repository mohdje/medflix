import "../../../style/movie-intermediate-presentation.css";

import MovieLitePresentation from "./MovieLitePresentation";
import MoreButton from "../../common/buttons/MoreButton";

function MovieIntermediatePresentation({ movie, onClick }) {
    const truncateText = (text) =>{
        if(text.length > 400) return text.substring(0, 300) + '...';
        else return text;
    };

    return (
        <div className="movie-intermediate-presentation-container" onClick={() => onClick()}>
            <MovieLitePresentation movie={movie} />
            <div className="movie-intermediate-presentation-content">
                <div className="movie-intermediate-presentation-info">
                    <div className="info"><span className="info-title">Synopsis:</span> {truncateText(movie.synopsis)}</div>
                </div>
            </div>
        </div>);
}

export default MovieIntermediatePresentation;