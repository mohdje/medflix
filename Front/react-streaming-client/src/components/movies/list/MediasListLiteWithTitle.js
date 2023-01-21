import "../../../style/medias-list.css";
import MediasListLite from "./MediasListLite";
import fadeTransition from "../../../js/customStyles.js";

function MediasListLiteWithTitle({ listTitle, medias, visible, onMediaClick}) {
    return (
        <div style={fadeTransition(medias && medias.length > 0  && visible, 1)} className="medias-list-genre-container">
            <div className="medias-list-header">
                <div className="medias-list-genre">{listTitle}</div>
            </div>
            <MediasListLite medias={medias} onMediaClick={(mediaId)=>onMediaClick(mediaId)}/>
        </div>
    );
}
export default MediasListLiteWithTitle;

