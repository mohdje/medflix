import "../../../style/css/media-videos-presentation.css";

import MediaVideosPresentation from "../presentation/MediaVideosPresentation";

function MediasVideosList({ mediasVideos, onVideoSelectionChanged }) {
    if (!Array.isArray(mediasVideos) || mediasVideos.length === 0) {
        return <p>No media videos available.</p>;
    }
    return (
        <div className="medias-videos-list">
            {mediasVideos.map((mediaVideos) => (
                <MediaVideosPresentation
                    key={mediaVideos.title + mediaVideos.year}
                    mediaVideos={mediaVideos}
                    onVideoSelectionChanged={(videoId, isSelected) => onVideoSelectionChanged(videoId, isSelected)} />
            ))}
        </div>
    );
}

export default MediasVideosList;
