import "../../../style/css/media-vertical-list.css";
import PlusIcon from "../../../assets/plus.svg";

import MediaLitePresentation from "../presentation/MediaLitePresentation";
import CircularProgressBar from "../../common/CircularProgressBar";
import Button from "../../common/Button";

import { useState, useEffect, useRef } from 'react';

function MediasVerticalList({ title, medias, loadingProgressVisible, moreButtonVisible, onMediaClick, onMoreButtonClick }) {
    const [mediaListWidth, setMediaListWidth] = useState(0);
    const mediaListContainer = useRef(null);

    useEffect(() => {
        if (mediaListContainer?.current) {
            computeListWidth();
            window.addEventListener("resize", computeListWidth);
        }

        return () => {
            window.removeEventListener("resize", computeListWidth);
        }
    }, [mediaListContainer])

    const computeListWidth = () => {
        const mediaLitePresentationWidth = getMediaLitePresentationWidth();
        const gap = getComputedStyleValue(mediaListContainer?.current.querySelector(".media-vertical-list"), "gap");
        const scrollbarWidth = 5;
        const numberOfItemsPerLine = Math.floor((mediaListContainer.current.offsetWidth + scrollbarWidth) / (mediaLitePresentationWidth + (gap / 2)));
        setMediaListWidth((numberOfItemsPerLine * mediaLitePresentationWidth) + ((numberOfItemsPerLine - 1) * gap));
    }

    const getMediaLitePresentationWidth = () => {
        const tempElement = document.createElement('div');
        tempElement.className = "media-lite-presentation";
        tempElement.style.display = 'none';
        document.body.appendChild(tempElement);
        const value = getComputedStyleValue(tempElement, "width");
        document.body.removeChild(tempElement);
        return value;
    }

    const getComputedStyleValue = (htmlElement, propertyName) => {
        const value = window.getComputedStyle(htmlElement).getPropertyValue(propertyName);
        return parseInt(value.replace('px', ''));
    }

    return (
        <div ref={mediaListContainer} className="media-vertical-list-container">
            <CircularProgressBar size="x-large" position={"center"} visible={loadingProgressVisible} />
            <h2>{title}</h2>
            <div className="media-vertical-list-wrapper">
                <div className="media-vertical-list" style={{ maxWidth: `${mediaListWidth}px` }}>
                    {medias.map((media) =>
                        <MediaLitePresentation
                            key={media.id}
                            media={media}
                            onMediaClick={() => { onMediaClick(media) }} />
                    )}
                </div>
                <div className={`more-button-container ${moreButtonVisible ? 'visible' : 'hidden'}`}><Button imgSrc={PlusIcon} color="gray" onClick={() => onMoreButtonClick()} /></div>
            </div>
        </div >
    )
}

export default MediasVerticalList;