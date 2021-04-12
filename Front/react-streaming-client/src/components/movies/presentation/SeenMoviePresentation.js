import "../../../style/seen-movie-presentation.css";

import MovieLitePresentation from "./MovieLitePresentation";
import PlayButton from "../../common/PlayButton";
import MoreButton from "../../common/MoreButton";

function SeenMoviePresentation({ seenMovie, onPlayClick, onMoreClick }) {
    const truncateText = (text) =>{
        if(text.length > 400) return text.substring(0, 300) + '...';
        else return text;
    };
    
    return (
        <div className="seen-movie-presentation-container">
            <MovieLitePresentation movie={seenMovie.movie} />
            <div className="seen-movie-presentation-content">
                <div className="seen-movie-presentation-info">
                    <div className="info"><span className="info-title">Service:</span> {seenMovie.serviceName}</div>
                    <div className="info"><span className="info-title">Synopsis:</span> {truncateText(seenMovie.movie.synopsis)}</div>
                </div>
                <div className="seen-movie-presentation-actions">
                    <MoreButton onClick={() => onMoreClick()} color="grey" />
                    <PlayButton onClick={() => onPlayClick()} />
                </div>
            </div>
        </div>);
}

export default SeenMoviePresentation;