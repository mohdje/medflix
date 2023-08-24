import "../../../style/medias-list.css";
import MediasListLite from "./MediasListLite";
import fadeTransition from "../../../helpers/customStyles.js";

function MediasListLiteWithTitle({ listTitle, medias, alignLeft, visible, onMediaClick }) {
    return (
        <div style={fadeTransition(medias && medias.length > 0 && visible, 1)} className="medias-list-genre-container">
            <div className="medias-list-header">
                <div className="medias-list-categorie">{listTitle}</div>
            </div>
            <MediasListLite medias={medias} alignLeft={alignLeft} onMediaClick={(mediaId) => onMediaClick(mediaId)} />
        </div>
    );
}
export default MediasListLiteWithTitle;

