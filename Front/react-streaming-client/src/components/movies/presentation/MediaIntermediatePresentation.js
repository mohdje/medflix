import "../../../style/media-intermediate-presentation.css";

import MediaLitePresentation from "./MediaLitePresentation";
import ProgressionBar from "../../common/ProgressionBar";
import SecondaryInfo from "../../common/text/SecondaryInfo";

function MovieIntermediatePresentation({ media, onClick }) {
    const truncateText = (text) =>{
        if(text.length > 400) return text.substring(0, 300) + '...';
        else return text;
    };

    return (
        <div className="media-intermediate-presentation-container" onClick={() => onClick()}>
            <MediaLitePresentation media={media} />
            <div className="media-intermediate-presentation-content">
                <div className="media-intermediate-presentation-info">
                    <SecondaryInfo text={truncateText(media.synopsis)} />
                    {media.progression > 0 ? <ProgressionBar value={media.progression * 100} width="100%"/> : null}
                </div>
            </div>
        </div>);
}

export default MovieIntermediatePresentation;