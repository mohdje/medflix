import "../../../style/movie-intermediate-presentation.css";

import MovieLitePresentation from "./MovieLitePresentation";
import ProgressionBar from "../../common/ProgressionBar";
import SecondaryInfo from "../../common/text/SecondaryInfo";

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
                    <SecondaryInfo text={truncateText(movie.synopsis)} />
                    {movie.progression > 0 ? <ProgressionBar value={movie.progression * 100} width="100%"/> : null}
                </div>
            </div>
        </div>);
}

export default MovieIntermediatePresentation;