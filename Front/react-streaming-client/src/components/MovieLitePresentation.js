
import "../style/movie-lite-presentation.css";

import { useState, useEffect } from 'react';
import fadeTransition from "../js/customStyles.js";

function MovieLitePresentation({ movie, onMovieClick }) {
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
            <div className={"movie-lite-presentation"} onClick={() => handleClick()}>
                <div className={"movie-lite-presentation-img"}>
                    <img alt="" src={movie.coverImageUrl} />
                </div>
                <div className="infos">
                    <div className="title">{truncate(movie.title, 35)}</div>
                    <div style={{ width: '100%', display: 'flex', justifyContent: 'space-between' }}>
                        <div className="year">{movie.year}</div>
                        <div className="rating">{movie.rating}</div>
                    </div>
                </div>
            </div>
        </div>
    )
}

export default MovieLitePresentation;