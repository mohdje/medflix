import "../../../style/media-intermediate-list.css";

import MediaIntermediatePresentation from "../presentation/MediaIntermediatePresentation";
import CircularProgressBar from "../../common/CircularProgressBar";
import { useEffect } from 'react';

function MediasIntermediatePresentationList({ title, medias, centerToMedia, loadingProgressVisible, onClick }) {

    useEffect(() => {
        if (centerToMedia) {
            var index = medias.findIndex(m => m.id === centerToMedia.id);
            var elem = document.getElementById("mediaintermediatepresentation" + index);
            if (elem) {
                elem.scrollIntoView({
                    block: "nearest",
                    inline: "nearest"
                });
            }
        }
    }, [medias, centerToMedia]);

    return (
        <div>
            <CircularProgressBar color={'white'} size={'80px'} position={"center"} visible={loadingProgressVisible} />          
            <div className="media-intermediate-list-title-page">{title}</div>
            <div className="media-intermediate-list-content">
                {medias.map((media, index) =>
                (<div id={"mediaintermediatepresentation" + index} key={index}>
                    <MediaIntermediatePresentation
                        media={media}
                        onClick={() => onClick(media)}/>
                </div>))}
            </div>
        </div>
    )
}

export default MediasIntermediatePresentationList;