import "../style/suggested-movie.css";
import "../style/button.css";


function SuggestedMovie({movie, visible, onMoreClick}){

    const truncateText = (text)=>{
        if(!text) return '';

        var maxTextlength = 500;

        return text.substring(0, maxTextlength) + (text && text.length > maxTextlength ? '...' : '');
    }

    return(
        <div className={"suggested-movie " + (!visible ? "hidden" : "")} style={{backgroundImage: 'url('+ movie.backgroundImageUrl+')'}}>
            <div className="suggested-movie-infos">
                <img alt="" className="suggested-movie-cover-image" src={movie.coverImageUrl}/>
                <div className="suggested-movie-rating">
                    {movie.rating}
                </div>                        
            </div>
            <div className="suggested-movie-infos">
                <div className="suggested-movie-title">{movie.title}</div>
                <div className="suggested-movie-year">{movie.year}</div>
                <div className="suggested-movie-summary">{truncateText(movie.synopsis)}</div>
                <div className="standard-button red center" onClick={()=>onMoreClick()}>More</div>
            </div>
           
        </div>
    );
}

export default SuggestedMovie;