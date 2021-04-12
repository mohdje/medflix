
import "../../../style/movie-lite-presentation.css";

import { useState, useEffect } from 'react';
import fadeTransition from "../../../js/customStyles.js";

function MovieLitePresentation({ movie, onMovieClick, hoverEffect }) {
    const [dataLoaded, setDataLoaded] = useState(false);

    const truncate = (text, maxLength) => {
        if (text && text.length > maxLength)
            return text.substring(0, maxLength) + '...';
        else return text;
    }

    useEffect(() => {
        setDataLoaded(movie?.coverImageUrl);
    }, [movie]);

    const handleClick = () => {
        setTimeout(()=>{
            onMovieClick(movie?.id);
        }, 150);   
    }

    return (
        <div style={fadeTransition(dataLoaded)}>
            <div className={"movie-lite-presentation " + (hoverEffect ? "hover-effect": "")} onClick={() => handleClick()}>
                <div className={"movie-lite-presentation-img"}  style={{ backgroundImage: 'url(' + movie.coverImageUrl + ')' }}>
                    <div className="rating">{movie.rating}</div>
                </div>
                <div className="infos">
                    <div className="title">{truncate(movie.title, 25)}</div>
                    <div className="year">{movie.year}</div>
                </div>
            </div>
        </div>
    )
}

export default MovieLitePresentation;