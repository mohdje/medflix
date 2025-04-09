import AppMode from "../../services/appMode";
import CircularProgressBar from "../common/CircularProgressBar";
import ModalWindow from "../modal/ModalWindow";
import { useEffect } from "react";

export default function NativeMediaPlayer({
    mediaDetails,
    currentTime,
    shouldRestart,
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
                        currentTime: shouldRestart ? 0 : currentTime,
                        episodeNumber,
                        seasonNumber,
                        videoSource,
                    },
                    mediaSources,
                    subtitlesSources
                };
            }
            window.getAppMode = () => AppMode.getActiveMode().urlKey;
            window.closeNativeMediaPlayer = () => onCloseClick();
            window.location.href = `http://playMedia`;
        }
        return () => {
            delete window.closeNativeMediaPlayer;
            delete window.getMediaPlayerParameters;
            delete window.getAppMode;
        }
    }, [visible]);

    return <ModalWindow content={<CircularProgressBar size="x-large" visible position="center" />} visible={visible} />
}