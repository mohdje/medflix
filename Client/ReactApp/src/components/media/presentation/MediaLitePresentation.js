
import "../../../style/css/media-lite-presentation.css";
import "../../../style/css/animations.css";

import Rating from "../../common/Rating";
import ProgressionBar from "../../common/ProgressionBar.js";
import { useRef } from "react";
import { useRippleEffect } from "../../../helpers/customHooks.js";

function MediaLitePresentation({ media, onMediaClick }) {
    const mediaLitePresentationRef = useRef(null);
    useRippleEffect(mediaLitePresentationRef);

    if (!Boolean(media?.coverImageUrl))
        return null;

    const handleClick = () => {
        setTimeout(() => {
            if (onMediaClick)
                onMediaClick();
        }, 300);
    }

    return (
        <div className="fade-in">
            <div ref={mediaLitePresentationRef} className="media-lite-presentation" onClick={() => handleClick()}>
                <div className="media-lite-presentation-img" style={{ backgroundImage: 'url(' + media.coverImageUrl + ')' }}>
                    <h3 className="title">{media.title}</h3>
                    <div className="year-container">
                        <h4 className="year">{media.year}</h4>
                    </div>
                    <div className="rating-container">
                        <Rating rating={media.rating ? media.rating.toString().replace(',', '.') : ''} size="small" />
                    </div>
                </div>
            </div>
            {!!media.progress ? <div style={{ marginTop: '5px' }}><ProgressionBar value={media.progress} width={'100%'} /></div> : null}
        </div >
    )
}

export default MediaLitePresentation;