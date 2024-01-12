
import "../../style/video-player-window.css";
import "../../style/button.css";
import AppServices from "../../services/AppServices";
import AppMode from "../../services/appMode";
import VideoPlayer from "./VideoPlayer";
import Paragraph from "../common/text/Paragraph";
import BaseButton from "../common/buttons/BaseButton";
import ModalWindow from "../modal/ModalWindow";
import { ToTimeFormat } from "../../helpers/timeFormatHelper";

import { useEffect, useState } from 'react';

export function VideoPlayerWindow({ sources, subtitles, visible, onCloseClick, onWatchedTimeUpdate, goToTime, mediaDetails }) {
    const [subtitlesOptions, setSubtitlesOptions] = useState([]);
    const [videoQualitiesOptions, setVideoQualitiesOptions] = useState([]);
    const [resumeMessageVisible, setResumeMessageVisible] = useState(true);
    const [videoTime, setVideoTime] = useState(0);
    const [isDesktopApp, setIsDesktopApp] = useState(false);
    const [showLoadingModal, setShowLoadingModal] = useState(false);


    useEffect(() => {
        if (visible) {
            AppServices.appInfoApiService.isDesktopApplication((isDesktopApp) => {
                setIsDesktopApp(isDesktopApp);
                if (isDesktopApp) {
                    setShowLoadingModal(true);

                    const options = {
                        sources: sources.map(source => {
                            return {
                                quality: source.quality.trim(),
                                selected: source.selected,
                                url: AppServices.torrentApiService.buildStreamUrl(source.downloadUrl, source.fileName, source.seasonNumber, source.episodeNumber)
                            }
                        }),
                        subtitles: subtitles ?? [],
                        resumeToTime: goToTime,
                        watchedMedia: mediaDetails && {
                            title: mediaDetails.title,
                            id: mediaDetails.id,
                            genres: mediaDetails.genres,
                            coverImageUrl: mediaDetails.coverImageUrl,
                            rating: mediaDetails.rating,
                            totalDuration: mediaDetails.duration * 60,
                            synopsis: mediaDetails.synopsis,
                            year: mediaDetails.year,
                            episodeNumber: sources[0].episodeNumber,
                            seasonNumber: sources[0].seasonNumber,
                            resumeToTime: mediaDetails.currentTime
                        },
                        mediaType: AppMode.getActiveMode().urlKey
                    }

                    var optionsJSON = JSON.stringify(options);

                    if (window.chrome) {
                        window.chrome.webview.addEventListener("message", onDesktopVideoPlayerClosed);
                        window.chrome.webview.postMessage(optionsJSON);
                    }
                    else if (window.webkit) {
                        window.__dispatchMessageCallback = () => onDesktopVideoPlayerClosed();
                        window.webkit.messageHandlers.webview.postMessage(optionsJSON);
                    }
                }
            });
        }

    }, [visible]);

    useEffect(() => {
        return () => {
            if (isDesktopApp) {
                if (window.chrome) {
                    window.chrome.webview.removeEventListener("message", onDesktopVideoPlayerClosed);
                }
                else if (window.webkit) {
                    window.__dispatchMessageCallback = null;
                }
            }
        }
    }, [])

    const onDesktopVideoPlayerClosed = () => {
        setShowLoadingModal(false);
        onCloseClick();//trigger close click to notify parent that video player is closed
    }

    const buildVideoQualitiesOptions = (sources) => {
        var options = [];
        if (!sources) return;

        sources.sort((source1, source2) => {
            const [quality1, quality2] = [source1.quality.toLowerCase().trim(), source2.quality.toLowerCase().trim()]
            return quality1 === quality2 ? 0 : (quality1 < quality2 ? -1 : 1);
        });

        sources.forEach(source => {
            var qualities = options.filter(o => o.label.startsWith(source.quality));

            var option = {
                label: qualities && qualities.length > 0 ? source.quality + ' (' + (qualities.length + 1) + ')' : source.quality,
                selected: source.selected,
                data: {
                    url: AppServices.torrentApiService.buildStreamUrl(source.downloadUrl, source.fileName, source.seasonNumber, source.episodeNumber)
                }
            }
            options.push(option);
        });

        if (options.filter(o => o.selected).length === 0)
            options[0].selected = true;

        setVideoQualitiesOptions(options);
    }

    const buildSubtitlesOptions = (subtitles) => {
        var newSubtitlesOptions = [];
        newSubtitlesOptions.push(
            {
                label: 'Off',
                selected: true
            });

        if (subtitles && subtitles.length > 0) {
            subtitles.forEach(sub => {
                var subtitlesOption = {
                    label: sub.language,
                    selected: false,
                    subOptions: []
                };
                sub.subtitlesSourceUrls.forEach((sourceUrl, index) => {
                    subtitlesOption.subOptions.push({
                        label: sub.language + ' ' + (index + 1),
                        selected: false,
                        data: {
                            url: AppServices.subtitlesApiService.getSubtitlesApiUrl(sourceUrl)
                        }
                    });
                })
                newSubtitlesOptions.push(subtitlesOption);
            });
        }

        setSubtitlesOptions(newSubtitlesOptions);
    }

    useEffect(() => {
        if (visible && sources && sources.length > 0) {
            buildVideoQualitiesOptions(sources);
        }
    }, [sources, visible]);

    useEffect(() => {
        if (visible && subtitles && subtitles.length > 0) {
            buildSubtitlesOptions(subtitles);
        }
    }, [subtitles, visible]);

    useEffect(() => {
        if (goToTime > 0) {
            setResumeMessageVisible(true);
        }
    }, [goToTime]);


    const content = (
        <div className="video-player-window-container">
            <ResumeMessage
                visible={goToTime > 0 && resumeMessageVisible}
                resumeTime={goToTime}
                onNoClick={() => setResumeMessageVisible(false)}
                onYesClick={() => { setVideoTime(goToTime); setResumeMessageVisible(false) }} />
            <VideoPlayerContainer
                visible={visible}
                videoQualitiesOptions={videoQualitiesOptions}
                subtitlesOptions={subtitlesOptions}
                onWatchedTimeUpdate={(currentTime, duration, sourceUrl) => onWatchedTimeUpdate(currentTime, duration, sourceUrl)}
                videoTime={videoTime} />
        </div>
    )
    return (
        isDesktopApp ?
            <ModalWindow visible={showLoadingModal} />
            : <ModalWindow visible={visible} content={content} onCloseClick={() => onCloseClick()} />
    );
}

function ResumeMessage({ visible, resumeTime, onYesClick, onNoClick }) {
    const containerStyle = {
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center'
    }
    if (visible) {
        return (
            <div style={containerStyle}>
                <Paragraph text={"Do you want to resume the video to " + ToTimeFormat(resumeTime / 60) + " ?"} />
                <BaseButton color="red" content="Yes" onClick={() => onYesClick()} />
                <BaseButton color="grey" content="No" onClick={() => onNoClick()} />
            </div>
        );
    }
    else
        return null;
}

function VideoPlayerContainer({ visible, videoQualitiesOptions, subtitlesOptions, onWatchedTimeUpdate, videoTime }) {

    if (visible) {
        return <div className="video-player-container">
            <VideoPlayer
                videoQualitiesOptions={videoQualitiesOptions}
                videoSubtitlesOptions={subtitlesOptions}
                onWatchedTimeUpdate={(currentTime, duration, sourceUrl) => onWatchedTimeUpdate(currentTime, duration, sourceUrl)}
                videoTime={videoTime} />
        </div>;
    }
    else
        return null;
}