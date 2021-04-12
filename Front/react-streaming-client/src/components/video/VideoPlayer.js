import "../../style/video-player.css";

import CircularProgressBar from "../common/CircularProgressBar";

import PlayArrowIcon from '@material-ui/icons/PlayArrow';
import PauseIcon from '@material-ui/icons/Pause';
import VolumeUp from '@material-ui/icons/VolumeUp';
import VolumeOffIcon from '@material-ui/icons/VolumeOff';
import SettingsIcon from '@material-ui/icons/Settings';
import SubtitlesIcon from '@material-ui/icons/Subtitles';
import FullscreenIcon from '@material-ui/icons/Fullscreen';
import FullscreenExitIcon from '@material-ui/icons/FullscreenExit';
import SortByAlphaIcon from '@material-ui/icons/SortByAlpha';
import RestoreIcon from '@material-ui/icons/Restore';

import { useEffect, useState, useRef } from 'react';
import { useOnClickOutside } from '../../js/customHooks';

function VideoPlayer({ videoQualitiesOptions, videoSubtitlesOptions, mustPauseVideo }) {
    const videoRef = useRef(null);
    const videoPlayerContainerRef = useRef(null);

    const [showErrorMessage, setshowErrorMessage] = useState(false);

    const [videoSource, setVideoSource] = useState('');
    const [videoIsLoading, setVideoIsLoading] = useState(false);

    const [subtitlesUrl, setSubtitlesUrl] = useState('');
    const [subtitlesSize, setSubtitlesSize] = useState(30);
    const [subtitlesAdjustTime, setSubtitlesAdjustTime] = useState(0);

    const [showVideoControls, setShowVideoControls] = useState(false);
    const lastTimeMouseMovedRef = useRef(0);

    const changeVideoSource = (url) => {
        var videoPlaying = !videoRef.current.paused;
        setVideoSource(url);
        var currentTime = videoRef.current.currentTime;
        videoRef.current.load();
        videoRef.current.currentTime = currentTime;
        if (videoPlaying) videoRef.current.play();
        setVideoIsLoading(true);
        setshowErrorMessage(false);
    }

    useEffect(() => {
        const displayControls = () => {
            setShowVideoControls(true);
            lastTimeMouseMovedRef.current = Date.now();
            const waitingTime = 3000;
            setTimeout(() => {
                if (videoPlayerContainerRef?.current && Date.now() > lastTimeMouseMovedRef.current + waitingTime)
                    setShowVideoControls(false);
            }, waitingTime + 500);
        };
        videoPlayerContainerRef.current.addEventListener("mousemove", displayControls);

        return () => {
            if (videoPlayerContainerRef?.current) videoPlayerContainerRef.current.removeEventListener("mousemove", displayControls);
        }
    }, [videoPlayerContainerRef]);

    useEffect(() => {
        const videoErrorHandler = () => {
            if (videoRef.current.childNodes[0].attributes.src.value) {
                setshowErrorMessage(true);
                setVideoIsLoading(false);
            }
        }
        videoRef.current.addEventListener('error', videoErrorHandler, true);

        return () => {
            if (videoRef?.current) videoRef.current.removeEventListener("error", videoErrorHandler);
        }
    }, [videoRef]);

    useEffect(() => {
        if (videoRef?.current) setVideoIsLoading(videoRef.current.readyState < 3);   
    }, [videoRef?.current?.readyState]);

    useEffect(() => {
        if (videoQualitiesOptions) {
            const videoQuality = videoQualitiesOptions.find(op => op.selected);
            if (videoQuality?.data?.url) changeVideoSource(videoQuality.data.url);
        }
    }, [videoQualitiesOptions]);

    return (
        <div ref={videoPlayerContainerRef} className="video-player-content" >
            <CircularProgressBar color={'white'} size={'80px'} position={"center"} visible={videoSource && videoIsLoading} />
            <div className="video-player-error-message" style={{ display: showErrorMessage ? '' : 'none' }}>An error occured during the loading</div>
            <video ref={videoRef} className="video-player">
                <source src={videoSource} title='1080p' type='video/mp4' />
            </video>
            <div className="video-player-bottom">
                <VideoSubtitles
                    videoPlayer={videoRef.current}
                    urlSource={subtitlesUrl}
                    size={subtitlesSize}
                    subtitlesAdjustTime={subtitlesAdjustTime} />
                <div className="video-player-controls-container" style={{ display: showVideoControls ? '' : 'none' }}>
                    <TimeController videoPlayer={videoRef.current} />
                    <div className="video-player-controls">
                        <LeftControlsGroup videoPlayer={videoRef.current} mustPauseVideo={mustPauseVideo} />
                        <RightControlsGroup
                            videoPlayerContainer={videoPlayerContainerRef.current}
                            subtitlesOptions={videoSubtitlesOptions}
                            qualityOptions={videoQualitiesOptions}
                            onSubtitleSizeChange={(pixelsToAdd) => setSubtitlesSize(subtitlesSize + pixelsToAdd <= 60 && subtitlesSize + pixelsToAdd >= 20 ? subtitlesSize + pixelsToAdd : subtitlesSize)}
                            onSubtitlesChange={(newUrlSource) => setSubtitlesUrl(newUrlSource ? newUrlSource : '')}
                            onAdjustSubtitleTimeChange={(time) => setSubtitlesAdjustTime(time ? time : 0)}
                            onVideoQualityChange={(newUrlSource) => changeVideoSource(newUrlSource)} />
                    </div>
                </div>
            </div>
        </div>
    );
}
export default VideoPlayer;

function VideoSubtitles({ videoPlayer, urlSource, size, subtitlesAdjustTime }) {
    const [currentSubtitles, setCurrentSubtitles] = useState('');
    const [subtitles, setSubtitles] = useState([]);
    const [textItalic, setTextItalic] = useState(false);

    const applySubtitles = () => {
        if (!subtitles || subtitles.length === 0)
            return;
        const currentTime = videoPlayer.currentTime;
        var subtitle = subtitles.find(s =>
            (s.startTime + subtitlesAdjustTime) <= currentTime && currentTime <= (s.endTime + subtitlesAdjustTime));

        if (subtitle?.text) {
            var text = subtitle.text;
            setTextItalic(false);
            if (text.includes('<i>') || text.includes('</i>')) {
                setTextItalic(true);
                text = text.replace('<i>', '').replace('</i>', '');
            }
            setCurrentSubtitles(text);
        }
        else setCurrentSubtitles('');
    }

    useEffect(() => {
        if (urlSource) {
            var xhttp = new XMLHttpRequest();
            xhttp.onreadystatechange = function () {
                if (this.readyState == 4 && this.status == 200 && this.responseText) {
                    setSubtitles(JSON.parse(this.response));
                }
            };
            xhttp.open("GET", urlSource, true);
            xhttp.send();
        }

        return () => {
            setSubtitles([]);
            setCurrentSubtitles('');
        }
    }, [urlSource]);

    useEffect(() => {
        videoPlayer?.addEventListener('timeupdate', applySubtitles);
        return () => {
            videoPlayer?.removeEventListener('timeupdate', applySubtitles);
        };
    }, [videoPlayer, subtitles, subtitlesAdjustTime]);

    return (<div className="video-player-subtitles"
        style={{ fontSize: size + 'px', fontStyle: textItalic ? 'italic' : '' }}>
        {currentSubtitles}
    </div>);
}

function LeftControlsGroup({ videoPlayer, mustPauseVideo }) {
    const [videoIsPlaying, setVideoIsPlaying] = useState(false);

    const playVideo = () => {
        if (!videoPlayer) return;
        videoPlayer.play();
    }

    const pauseVideo = () => {
        if (!videoPlayer) return;
        videoPlayer.pause();
    }

    useEffect(() => {
        if (videoPlayer) {
            videoPlayer.onplay = () => { setVideoIsPlaying(true) }
            videoPlayer.onpause = () => { setVideoIsPlaying(false) }
        }
        return () => {
            if (videoPlayer) {
                videoPlayer.onplay = null;
                videoPlayer.onpause = null;
            }
        }
    }, [videoPlayer]);

    useEffect(() => {
        if (mustPauseVideo) pauseVideo();
    }, [mustPauseVideo]);

    return (
        <div className="controls-group">
            <PlayArrowIcon className="icon" style={{ display: videoIsPlaying ? 'none' : '' }} onClick={() => playVideo()} />
            <PauseIcon className="icon" style={{ display: videoIsPlaying ? '' : 'none' }} onClick={() => pauseVideo()} />
            <VolumeController videoPlayer={videoPlayer} />
        </div>)
}

function RightControlsGroup({
    videoPlayerContainer,
    subtitlesOptions,
    qualityOptions,
    onSubtitleSizeChange,
    onSubtitlesChange,
    onAdjustSubtitleTimeChange,
    onVideoQualityChange }) {

    const [videoInFullscreen, setVideoInFullscreen] = useState(false);
    const [screenPlayer, setScreenPlayer] = useState(null);

    const toFullScreen = () => {
        if (!screenPlayer) return;

        if (screenPlayer.requestFullscreen) screenPlayer.requestFullscreen();
        else if (screenPlayer.mozRequestFullScreen) screenPlayer.mozRequestFullScreen();
        else if (screenPlayer.webkitRequestFullscreen) screenPlayer.webkitRequestFullscreen();
        else if (screenPlayer.msRequestFullscreen) screenPlayer.msRequestFullscreen();
    }

    const leaveFullScreen = () => {
        document.exitFullscreen();
    }

    useEffect(() => {
        setScreenPlayer(videoPlayerContainer);
    }, [videoPlayerContainer]);

    useEffect(() => {
        const toggleVideoFullScreenInfo = () => {
            setVideoInFullscreen(!videoInFullscreen);
        }
        document.addEventListener('fullscreenchange', toggleVideoFullScreenInfo);
        return () => {
            document.removeEventListener('fullscreenchange', toggleVideoFullScreenInfo);
        };
    }, [videoInFullscreen]);

    return (
        <div className="controls-group">
            <div className="controls-sub-group">
                <VideoOptions options={subtitlesOptions}
                    icon={<SubtitlesIcon className="icon" />}
                    onOptionChanged={(option) => onSubtitlesChange(option?.data?.url ? option.data.url : '')}
                />
                <AdjustSubtitleTimeOption onAdjustSubtitleTimeChange={(time) => onAdjustSubtitleTimeChange(time)} />
                <VideoOptionsButton icon={<SortByAlphaIcon className="icon big" />} onClick={() => onSubtitleSizeChange(2)} />
                <VideoOptionsButton icon={<SortByAlphaIcon className="icon small" />} onClick={() => onSubtitleSizeChange(-2)} />
            </div>

            <VideoOptions options={qualityOptions}
                icon={<SettingsIcon className="icon" />}
                onOptionChanged={(option) => onVideoQualityChange(option?.data?.url ? option.data.url : '')}
            />

            <VideoOptionsButton icon={<FullscreenIcon className="icon big" />} hide={videoInFullscreen} onClick={() => toFullScreen()} />
            <VideoOptionsButton icon={<FullscreenExitIcon className="icon big" />} hide={!videoInFullscreen} onClick={() => leaveFullScreen()} />
        </div>)
}

function AdjustSubtitleTimeOption({ onAdjustSubtitleTimeChange }) {

    const [showMenu, setShowMenu] = useState(false);
    const menuRef = useRef(null);

    useOnClickOutside(menuRef, () => setShowMenu(false));

    return (
        <div ref={menuRef} className="adjust-subtitle-time-container">
            <div className={"video-options-menu" + (showMenu ? '' : ' hidden')}>
                <div className="label">Adj. Sub.</div>
                <div className="adjust-subtitle-time-input-container">
                    <input type="number" step="0.01" onChange={(e) => onAdjustSubtitleTimeChange(e.target.valueAsNumber)} /><span className="label"> s</span>
                </div>
            </div>
            <VideoOptionsButton icon={<RestoreIcon className="icon" />} onClick={() => setShowMenu(!showMenu)} />
        </div>
    );
}

function TimeController({ videoPlayer }) {
    const [videoCurrentTimeLabel, setVideoCurrentTimeLabel] = useState('00:00');
    const [videoTotalTimeLabel, setVideoTotalTimeLabel] = useState('00:00');
    const [currentPourcentageTimeVideo, setcurrentPourcentageTimeVideo] = useState(0);
    const [tooltip, setTooltip] = useState(null);

    const getTimeLabel = (duration) => {
        let hours = Math.floor(duration / 3600);
        let minutes = Math.floor((duration - (hours * 3600)) / 60);
        let seconds = Math.floor(duration - (hours * 3600) - (minutes * 60));

        let hourValues = '';
        if (hours > 0) {
            hourValues = hours < 10 ? '0' + hours : hours;
            hourValues += ':';
        }
        let minuteValue = minutes < 10 ? '0' + minutes : minutes;
        let secondValue = seconds < 10 ? '0' + seconds : seconds;
        return hourValues + minuteValue + ':' + secondValue;
    };

    const changeTimeVideo = (percentage) => {
        if (videoPlayer?.duration) {
            videoPlayer.currentTime = Math.floor((percentage * videoPlayer.duration) / 100);
            setVideoCurrentTimeLabel(getTimeLabel(videoPlayer.currentTime));
        }
    }

    const updateTimeVideo = () => {
        setVideoCurrentTimeLabel(getTimeLabel(videoPlayer.currentTime));
        setcurrentPourcentageTimeVideo((videoPlayer.currentTime) * 100 / videoPlayer.duration);
    }

    const setTimeControllerTooltip = (percentage) => {
        if (videoPlayer.duration > 0) {
            setTooltip({
                text: getTimeLabel(Math.floor((percentage * videoPlayer.duration) / 100)),
                position: percentage
            })
        }
    }

    useEffect(() => {
        if (videoPlayer)
            videoPlayer.addEventListener('timeupdate', updateTimeVideo);

        return () => {
            if (videoPlayer) videoPlayer.removeEventListener('timeupdate', updateTimeVideo);
        }
    }, [videoPlayer]);

    useEffect(() => {
        if (videoPlayer?.duration)
            setVideoTotalTimeLabel(getTimeLabel(videoPlayer.duration));
    }, [videoPlayer?.duration]);

    return (
        <div className="video-time-controller-container">
            <div className="video-time-label">{videoCurrentTimeLabel}</div>
            <VideoPlayerSlider value={currentPourcentageTimeVideo}
                onCursorEndMove={(percentage) => changeTimeVideo(percentage)}
                onMouseOverSlider={(percentage) => setTimeControllerTooltip(percentage)}
                tooltip={tooltip}
                progressColor={'#ed0f0f'}
                cursorColor={'#b59e9e'} />
            <div className="video-time-label">{videoTotalTimeLabel}</div>
        </div>
    );
}

function VolumeController({ videoPlayer }) {

    const defaultVolume = 70;
    const [currentVolume, setCurrentVolume] = useState(defaultVolume);
    const [previousVolume, setPreviousVolume] = useState(defaultVolume);

    const changeVolume = (percentage) => {
        if (videoPlayer) {
            videoPlayer.volume = percentage / 100;
            setCurrentVolume(percentage);
        }
    };

    const muteVideo = () => {
        setPreviousVolume(currentVolume);
        changeVolume(0);
    }

    useEffect(() => {
        if (videoPlayer) changeVolume(defaultVolume);
    }, [videoPlayer]);

    return (<div className="video-volume-controller">
        <VolumeUp className="icon" onClick={() => { muteVideo() }} style={{ display: currentVolume > 0 ? '' : 'none' }} />
        <VolumeOffIcon className="icon" onClick={() => changeVolume(previousVolume)} style={{ display: currentVolume > 0 ? 'none' : '' }} />
        <VideoPlayerSlider
            value={currentVolume}
            onCursorMoved={(percentage) => changeVolume(percentage)}
            onCursorEndMove={(percentage) => changeVolume(percentage)}
            width={'120px'}
            height={'4px'} />
    </div>)
}

function VideoOptions({ options, icon, onOptionChanged }) {

    const [menuOptions, setMenuOptions] = useState([]);
    const [showMenu, setShowMenu] = useState(false);
    const [optionsDisplay, setOptionsDisplay] = useState('');
    const optionsWindowRef = useRef(null);

    const updateMenuOptions = (selectedOption, selectedSubOption) => {

        const oldSelectedOption = menuOptions.find(op => op.selected);
        const oldSelectedSubOption = selectedOption?.subOptions?.find(subOp => subOp.selected);

        if (selectedOption?.subOptions && !selectedSubOption)
            return;

        if (selectedOption?.subOptions
            && oldSelectedSubOption?.label === selectedSubOption?.label)
            return;

        if (!selectedOption?.subOptions && oldSelectedOption?.label === selectedOption.label)
            return;

        var updatedOptions = [];
        menuOptions.forEach(op => {
            var updatedOption = {
                label: op.label,
                selected: op.label === selectedOption.label,
                data: op.data
            };
            var updatedSubOptions = [];
            if (op.subOptions) {
                op.subOptions.forEach(subOp => {
                    var updatedsubOption =
                    {
                        label: subOp.label,
                        data: subOp.data,
                        selected: subOp.label === selectedSubOption?.label
                    }
                    updatedSubOptions.push(updatedsubOption);
                })
                updatedOption.subOptions = updatedSubOptions;
            }
            updatedOptions.push(updatedOption);
        });

        setMenuOptions(updatedOptions);

        setShowMenu(false);

        var option = updatedOptions.find(op => op.selected);
        if (option.subOptions) onOptionChanged(option.subOptions.find(subOp => subOp.selected));
        else onOptionChanged(option);
    }

    const setSubOptionsVisibility = (option, visible) => {
        var element = document.getElementById('suboption-' + option.label)
        if (!element)
            return;

        if (visible) element.classList.remove("hidden");
        else element.classList.add("hidden");
    }

    const getOptionsDisplay = (options) => {
        return options.map(option =>
        (<div key={option.label} className={"option " + (option.selected ? 'selected' : '')}
            onClick={() => updateMenuOptions(option)}
            onMouseOver={() => setSubOptionsVisibility(option, true)}
            onMouseLeave={() => setSubOptionsVisibility(option, false)}>
            {option.label}
            {getSubOptionsDisplay(option)}
        </div>))
    }

    const getSubOptionsDisplay = (option) => {
        if (!option?.subOptions)
            return null;
        else {
            return (
                <div id={'suboption-' + option.label} className="video-options-menu suboptions hidden">
                    {option.subOptions.map(subOption =>
                    (<div key={subOption.label} className={"option " + (subOption.selected ? 'selected' : '')}
                        onClick={(event) => { updateMenuOptions(option, subOption); event.stopPropagation() }}>
                        {subOption.label}
                    </div>)
                    )}
                </div>
            );
        }
    }

    useEffect(() => {
        if (options) {
            setMenuOptions(options);
        }
    }, [options]);

    useEffect(() => {
        if (menuOptions?.length && menuOptions.length > 0) {
            setOptionsDisplay(getOptionsDisplay(menuOptions));
        }
    }, [menuOptions]);

    useOnClickOutside(optionsWindowRef, () => setShowMenu(false));

    return (
        <div ref={optionsWindowRef} className="video-options-container">
            <div className={"video-options-menu " + (showMenu ? '' : 'hidden')}>
                {optionsDisplay}
            </div>
            <VideoOptionsButton icon={icon} onClick={() => setShowMenu(!showMenu)} />
        </div>
    )
}

function VideoOptionsButton({ icon, hide, onClick }) {
    return (
        <div className="video-options-btn" style={{ display: hide ? 'none' : '' }} onClick={() => onClick()}>
            {icon}
        </div>
    )
}

function VideoPlayerSlider({ value, width, height, progressColor, cursorColor, tooltip, onCursorMoved, onCursorEndMove, onMouseOverSlider }) {
    const [cursorPosition, setCursorPosition] = useState(0);
    const [showToolTip, setShowToolTip] = useState(false);

    const positionToPercentage = (XPosition, railBoundings) => {
        if (XPosition < railBoundings.left)
            return 0;
        else if (XPosition > railBoundings.right)
            return 100;
        else
            return (XPosition - railBoundings.left) * 100 / (railBoundings.right - railBoundings.left);
    }

    const initiateCursorMove = () => {
        document.addEventListener('mousemove', moveCursor);
        document.addEventListener('mouseup', endMove);
    }

    const moveCursor = (event) => {
        var percentage = positionToPercentage(event.clientX, event.target.parentElement.getBoundingClientRect());
        setCursorPosition(percentage);
        if (onCursorMoved) onCursorMoved(percentage);
    }

    const endMove = (event) => {
        if (onCursorEndMove) {
            var percentage = positionToPercentage(event.clientX, event.target.parentElement.getBoundingClientRect());
            onCursorEndMove(percentage);
        }
        document.removeEventListener('mouseup', endMove);
        document.removeEventListener('mousemove', moveCursor);
    }

    const onSliderClick = (event) => {
        var percentage = positionToPercentage(event.clientX, event.target.parentElement.getBoundingClientRect());
        setCursorPosition(percentage);
        if (onCursorEndMove) onCursorEndMove(percentage);
    }

    const onMouseMoveOverSlider = (event) => {
        var percentage = positionToPercentage(event.clientX, event.target.parentElement.getBoundingClientRect());
        if (onMouseOverSlider) onMouseOverSlider(percentage);
    }

    const onMouseLeaveSlider = () => {
        setShowToolTip(false);
    }

    useEffect(() => {
        setCursorPosition(value);
    }, [value])

    useEffect(() => {
        if (tooltip) setShowToolTip(true);
    }, [tooltip])

    return (
        <div className="video-player-slider-container"
            style={{ width: width, height: height }}
            onClick={(event) => onSliderClick(event)}
            onMouseMove={(event) => onMouseMoveOverSlider(event)}
            onMouseLeave={() => onMouseLeaveSlider()}>
            <div className="complete-part" style={{ width: cursorPosition + "%", backgroundColor: progressColor }}></div>
            <div className="remaining-part" style={{ width: (100 - cursorPosition) + "%" }}></div>
            <div className="cursor" style={{ left: cursorPosition + '%', backgroundColor: cursorColor }} onMouseDown={() => initiateCursorMove()}></div>
            <div className="tooltip" style={{ left: tooltip?.position + '%', display: showToolTip ? '' : 'none' }}>{tooltip?.text}</div>
        </div>)
}
