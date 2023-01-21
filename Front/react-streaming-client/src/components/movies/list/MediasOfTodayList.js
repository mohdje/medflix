

import PlayArrowIcon from '@material-ui/icons/PlayArrow';
import PauseIcon from '@material-ui/icons/Pause';
import MediaPicturesPresentation from "../presentation/MediaPicturesPresentation";
import { useEffect, useState } from 'react';
import "../../../style/media-pictures-presentation.css";


function MediasOfToday({ medias, onClick }) {
    const [mediaIndexVisible, setMediaIndexVisible] = useState(0);
    const [carouselPlay, setCarouselPlay] = useState(true);

    useEffect(() => {
        var changeMedia;
        if (carouselPlay) {
            changeMedia = setTimeout(() => {
                if (mediaIndexVisible === medias?.length - 1)
                    setMediaIndexVisible(0)
                else setMediaIndexVisible(mediaIndexVisible + 1);
            }, 5000);
        }

        return () => {
            clearTimeout(changeMedia);
        }
    }, [mediaIndexVisible, carouselPlay]);


    return (
        <div className="medias-pictures-container">
            {medias?.map((media, index) =>
                <MediaPicturesPresentation
                    key={index}
                    media={media}
                    visible={index === mediaIndexVisible}
                    onClick={() => onClick(media.id)} />)}
            <div className="medias-pictures-nav-controls">
                <MediaPicturesCarousel
                    nbMedias={medias?.length}
                    selectedIndex={mediaIndexVisible}
                    onNavElementClick={(index) => setMediaIndexVisible(index)} />
                <div className="medias-pictures-play-pause" onClick={() => setCarouselPlay(!carouselPlay)}>
                    <PlayArrowIcon className="icon" style={{ display: carouselPlay ? 'none' : '' }} />
                    <PauseIcon className="icon" style={{ display: !carouselPlay ? 'none' : '' }} />
                </div>
            </div>
        </div>
    );
}

export default MediasOfToday;

function MediaPicturesCarousel({ nbMedias, selectedIndex, onNavElementClick }) {

    const navElements = [];

    for (let i = 0; i <= nbMedias - 1; i++)
        navElements.push(i);

    return (
        <div className="medias-pictures-nav">
            {navElements.map((i) =>
                <div key={i}
                    onClick={() => onNavElementClick(i)}
                    className={"nav-elem" + (selectedIndex === i ? ' selected' : '')}></div>)}
        </div>
    );
}