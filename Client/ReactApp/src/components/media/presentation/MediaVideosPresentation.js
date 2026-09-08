import "../../../style/css/media-videos-presentation.css";
import { formatFileSize } from "../../../helpers/formatHelper";
import { useState } from "react";

function MediaVideosPresentation({ mediaVideos, onVideoSelectionChanged }) {
    const isSerie = mediaVideos.isSerie;
    return (
        <div className="media-videos-presentation">
            <img className="cover" src={mediaVideos.coverImageUrl} alt={mediaVideos.title || ""} />
            <div className="details">
                <h3 className="title">{mediaVideos.title}</h3>
                <p className="year">{mediaVideos.year}</p>
                {isSerie ? (
                    <MediaVideosBySeason videos={mediaVideos.videos} onVideoSelectionChanged={onVideoSelectionChanged} />
                ) : (
                    mediaVideos.videos.map((video) => (
                        <MediaVideoFile key={video.id} video={video} onSelectionChanged={(isSelected) => onVideoSelectionChanged(video.id, isSelected)} />
                    ))
                )}
            </div>
        </div>
    );
}

export default MediaVideosPresentation;

function MediaVideoFile({ video, onSelectionChanged }) {
    const [isSelected, setIsSelected] = useState(false);
    const buildFileLabel = (video) => {
        const infos = [];
        if (video.language === 0)
            infos.push("EN");
        else if (video.language === 1)
            infos.push("FR");

        infos.push(video.quality);

        if (video.filePath.includes('h265') || video.filePath.includes('x265'))
            infos.push("HEVC");

        if (video.bytesLength)
            infos.push(formatFileSize(video.bytesLength));

        return infos.filter(Boolean).join(" | ");
    }

    const handleClick = () => {
        setIsSelected(!isSelected);
        onSelectionChanged && onSelectionChanged(!isSelected);
    }

    return (
        <div className={`video-file ${isSelected ? "selected" : ""}`} onClick={handleClick}>
            <p><span className="icon">{isSelected ? "🔴" : "📀"}</span>{buildFileLabel(video)}</p>
        </div>
    );
}

function MediaVideosBySeason({ videos, onVideoSelectionChanged }) {
    const seasons = [...new Set(videos.map(video => video.seasonNumber))].sort((a, b) => a - b);
    const [expandedSeasons, setExpandedSesons] = useState([]);

    const toggleSeason = (seasonNumber) => {
        if (expandedSeasons.includes(seasonNumber)) {
            setExpandedSesons(expandedSeasons.filter(s => s !== seasonNumber));
        } else {
            setExpandedSesons([...expandedSeasons, seasonNumber]);
        }
    }

    return (
        <div className="media-video-by-season">
            {seasons.map(seasonNumber => (
                <div key={seasonNumber} className="season-container">
                    <h3 className="season-title">Season {seasonNumber}<span className="expand-icon" onClick={() => toggleSeason(seasonNumber)}>{expandedSeasons.includes(seasonNumber) ? "-" : "+"}</span></h3>
                    <div className={`episodes-list ${expandedSeasons.includes(seasonNumber) ? "expanded" : ""}`}>
                        <MediaVideosByEpisode videos={videos.filter(video => video.seasonNumber === seasonNumber)} onVideoSelectionChanged={onVideoSelectionChanged} />
                    </div>
                </div>
            ))}
        </div>
    );
}


function MediaVideosByEpisode({ videos, onVideoSelectionChanged }) {
    const episodes = [...new Set(videos.map(video => video.episodeNumber))].sort((a, b) => a - b);

    return (
        <>
            {episodes.map(episodeNumber => (
                <div key={episodeNumber} >
                    <h3 className="episode-title">Episode {episodeNumber}</h3>
                    <div className="episode-video-files">
                        {
                            videos.filter(video => video.episodeNumber === episodeNumber).map((video, i) => (
                                <MediaVideoFile key={i} video={video} onSelectionChanged={(isSelected) => onVideoSelectionChanged(video.id, isSelected)} />
                            ))
                        }
                    </div>

                </div>
            ))}
        </>
    );
}