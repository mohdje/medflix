
import "../../../style/media-lite-presentation.css";

import Rating from "../../common/Rating";

import { useState, useEffect } from 'react';
import fadeTransition from "../../../js/customStyles.js";

function MediaLitePresentation({ media, onMediaClick, hoverEffect }) {
    const [dataLoaded, setDataLoaded] = useState(false);

    useEffect(() => {
        setDataLoaded(media?.coverImageUrl);
    }, [media]);

    if(!Boolean(media?.coverImageUrl))
        return null;

    const truncate = (text, maxLength) => {
        if (text && text.length > maxLength)
            return text.substring(0, maxLength) + '...';
        else return text;
    }

    const handleClick = () => {
        setTimeout(()=>{
            if(onMediaClick)
                onMediaClick(media?.id);
        }, 150);   
    }

    const formatMediaRating = (rating) => {
        return rating ? rating.toString().replace(',', '.') : '';
    };

    return (
        <div style={fadeTransition(dataLoaded)}>
            <div className={"media-lite-presentation " + (hoverEffect ? "hover-effect": "")} onClick={() => handleClick()}>
                <div className={"media-lite-presentation-img"}  style={{ backgroundImage: 'url(' + media.coverImageUrl + ')' }}>                
                    <div className="rating">
                        <Rating rating={formatMediaRating(media.rating)} size="small"/>
                    </div>
                </div>
                <div className="infos">
                    <div className="title">{truncate(media.title, 25)}</div>
                    <div className="year">{media.year}</div>
                </div>
            </div>
        </div>
    )
}

export default MediaLitePresentation;