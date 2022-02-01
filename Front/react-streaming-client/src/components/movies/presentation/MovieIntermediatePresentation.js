import "../../../style/movie-intermediate-presentation.css";

import MovieLitePresentation from "./MovieLitePresentation";
import PlayButton from "../../common/Buttons/PlayButton";
import MoreButton from "../../common/Buttons/MoreButton";
import DeleteButton from "../../common/Buttons/DeleteButton";

function MovieIntermediatePresentation({ movieBookmark, deleteButtonAvailable, onPlayClick, onMoreClick, onDeleteClick }) {
    const truncateText = (text) =>{
        if(text.length > 400) return text.substring(0, 300) + '...';
        else return text;
    };

    return (
        <div className="movie-intermediate-presentation-container">
            <MovieLitePresentation movie={movieBookmark.movie} />
            <div className="movie-intermediate-presentation-content">
                <div className="movie-intermediate-presentation-info">
                    <div className="info"><span className="info-title">Service:</span> {movieBookmark.serviceName}</div>
                    <div className="info"><span className="info-title">Synopsis:</span> {truncateText(movieBookmark.movie.synopsis)}</div>
                </div>
                <div className="movie-intermediate-presentation-actions">
                    <MoreButton onClick={() => onMoreClick()} color="grey" />
                    <DeleteButton onClick={() => onDeleteClick()} visible={deleteButtonAvailable}/>
                </div>
            </div>
        </div>);
}

export default MovieIntermediatePresentation;