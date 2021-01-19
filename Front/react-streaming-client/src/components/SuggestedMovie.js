import "../style/suggested-movie.css";
import "../style/button.css";


function SuggestedMovie({ movie, visible, onMoreClick }) {

    const truncateText = (text) => {
        if (!text) return '';
        var maxTextlength = 300;
        return text.substring(0, maxTextlength) + (text && text.length > maxTextlength ? '...' : '');
    }

    return (
        <div className={"suggested-movie " + (!visible ? "hidden" : "")} >
            <div className="suggested-movie-infos">
                <div className="header">
                    <div>
                        <div className="suggested-movie-title">{movie.title}</div>
                        <div className="suggested-movie-year">{movie.year}</div>
                    </div>
                    <div className="suggested-movie-rating">{movie.rating}</div>
                </div>
                <div className="suggested-movie-summary">{truncateText(movie.synopsis)}</div>
                <div className="standard-button red center" onClick={() => onMoreClick()}>More</div>
            </div>
          
            <div className="suggested-movie-pictures" style={{ backgroundImage: 'url(' + movie.backgroundImageUrl + ')' }}>
                <img alt="" className="suggested-movie-cover-image" src={movie.coverImageUrl} />
            </div>
        </div>
    );
}

export default SuggestedMovie;