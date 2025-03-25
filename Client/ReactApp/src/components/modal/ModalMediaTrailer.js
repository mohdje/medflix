import "../../style/css/modal-media-trailer.css";
import ModalWindow from "./ModalWindow";

function ModalMediaTrailer({ visible, youtubeTrailerUrl, onCloseClick }) {

    const content = () => {
        return (
            <iframe className="media-trailer-container" src={visible ? youtubeTrailerUrl : null}></iframe>
        );
    };

    return (
        <ModalWindow visible={visible} content={content()} onCloseClick={() => onCloseClick()} />
    );
}

export default ModalMediaTrailer; 