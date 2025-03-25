import PlayIcon from '../../../assets/play.svg';
import PauseIcon from '../../../assets/pause.svg';
import { useEffect, useState } from 'react';
import "../../../style/css/medias-carousel.css";

function MediasCarousel({ medias, onClick }) {
    const [mediaIndexVisible, setMediaIndexVisible] = useState(0);
    const [carouselPlay, setCarouselPlay] = useState(false);

    useEffect(() => {
        if (medias && medias.length > 0)
            setCarouselPlay(true);
    }, [medias]);

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
                    logoTitleImgUrl={media.logoImageUrl}
                    backgroundImageUrl={media.backgroundImageUrl}
                    visible={index === mediaIndexVisible}
                    onClick={() => { onClick(media) }} />)}
            <div className="medias-pictures-nav-controls">
                <MediaPicturesCarousel
                    nbMedias={medias?.length}
                    selectedIndex={mediaIndexVisible}
                    onNavElementClick={(index) => setMediaIndexVisible(index)} />
                <div className="carousel-play-pause-btn" onClick={() => setCarouselPlay(!carouselPlay)}>
                    <img alt="play-pause" src={carouselPlay ? PauseIcon : PlayIcon}></img>
                </div>
            </div>
        </div>
    );
}

export default MediasCarousel;


function MediaPicturesPresentation({ logoTitleImgUrl, backgroundImageUrl, visible, onClick }) {
    return (
        <div className={"media-pictures " + (!visible ? "hidden" : "")} style={{ backgroundImage: `url(${backgroundImageUrl})` }} onClick={() => onClick()}>
            <div className="logo-title-container">
                <img src={logoTitleImgUrl}></img>
            </div>
        </div>
    );
}

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