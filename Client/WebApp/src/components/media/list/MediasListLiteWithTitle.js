import "../../../style/medias-list.css";
import MediasListLite from "./MediasListLite";

function MediasListLiteWithTitle({ listTitle, medias, alignLeft, visible, onMediaClick }) {
    if (!visible) return null;

    return (
        <div className="medias-list-genre-container">
            <div className="medias-list-header">
                <div className="medias-list-categorie">{listTitle}</div>
            </div>
            <MediasListLite medias={medias} alignLeft={alignLeft} onMediaClick={(mediaId) => onMediaClick(mediaId)} />
        </div>
    );
}
export default MediasListLiteWithTitle;

