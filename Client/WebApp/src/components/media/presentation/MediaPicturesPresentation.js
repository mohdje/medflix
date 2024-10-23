
function MediaPicturesPresentation({ media, visible, onClick }) {
    return (
        <div className={"media-pictures " + (!visible ? "hidden" : "")} onClick={() => onClick(media.id)}>
            <div className="logo-title">
                <img src={media?.logoImageUrl}></img>
            </div>
            <img className="background-image" src={media?.backgroundImageUrl}></img>
        </div>
    );
}

export default MediaPicturesPresentation;