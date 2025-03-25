import CircularProgressBar from "../common/CircularProgressBar";
import ModalWindow from "../modal/ModalWindow";
import { useEffect } from "react";

export default function NativeMediaPlayer({
    mediaDetails,
    currentTime,
    episodeNumber,
    seasonNumber,
    videoSource,
    mediaSources,
    subtitlesSources,
    visible,
    onCloseClick }) {

    useEffect(() => {
        if (visible) {
            window.getMediaPlayerParameters = () => {
                return {
                    watchMedia: {
                        media: mediaDetails,
                        currentTime,
                        episodeNumber,
                        seasonNumber,
                        videoSource,
                    },
                    mediaSources,
                    subtitlesSources
                };
            }
            window.closeNativeMediaPlayer = () => onCloseClick();
            window.location.href = `http://playMedia`;
        }
        return () => {
            delete window.closeNativeMediaPlayer;
            delete window.getMediaPlayerParameters;
        }
    }, [visible]);

    return <ModalWindow content={<CircularProgressBar size="x-large" visible position="center" />} visible={visible} />
}