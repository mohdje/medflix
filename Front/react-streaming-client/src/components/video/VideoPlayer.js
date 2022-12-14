import "../../style/video-player.css";

import fadeTransition from "../../js/customStyles";

import CircularProgressBar from "../common/CircularProgressBar";

import VideoSubtitles from './videoPlayerComponents/VideoSubtitles';
import TimeController from './videoPlayerComponents/TimeController';
import Controls from './videoPlayerComponents/Controls';

import MoviesAPI from "../../js/moviesAPI";

import { useEffect, useState, useRef } from 'react';

function VideoPlayer({ videoQualitiesOptions, videoSubtitlesOptions, mustPauseVideo }) {
    const videoRef = useRef(null);
    const videoPlayerContainerRef = useRef(null);

    const [isFullScreen, setIsFullScreen] = useState(false);
    const [currentTime, setCurrentTime] = useState(0);

    const [errorMessage, setErrorMessage] = useState();

    const checkDownloadStateRef = useRef(false);
    const [videoDownloadState, setVideoDownloadState] =  useState('Loading');
    
    const [videoSource, setVideoSource] = useState('');
    const [videoIsLoading, setVideoIsLoading] = useState(false);
    const [videoIsPlaying, setVideoIsPlaying] = useState(false);

    const [subtitlesUrl, setSubtitlesUrl] = useState('');
    const [subtitlesSize, setSubtitlesSize] = useState(30);
    const [subtitlesAdjustTime, setSubtitlesAdjustTime] = useState(0);

    const [showVideoControls, setShowVideoControls] = useState(false);
    const lastTimeMouseMovedRef = useRef(0);

    const checkMovieDownloadState = (url) => {
        if (checkDownloadStateRef.current && videoRef.current) {      
            MoviesAPI.getMovieDownloadState(url, (response) => { 
                if(decodeURIComponent(url) === decodeURIComponent(videoRef.current?.src)){
                    if(response.error) {
                        checkDownloadStateRef.current = false;
                        setErrorMessage(response.message);
                    }                
                    else{
                        setVideoDownloadState(response.message ? response.message : 'Loading'); 
                        setTimeout(() => {
                            checkMovieDownloadState(url);
                        }, 3000);
                    }                
                }
            })
        }
    };

    const changeVideoSource = (url) => {       
        var currentTime = videoRef.current.currentTime;
        setVideoSource(url);

        videoRef.current.autoplay = !videoRef.current.paused;
        videoRef.current.load();
       
        videoRef.current.currentTime = currentTime;
        setCurrentTime(currentTime);
      
        setVideoIsLoading(true);
        checkDownloadStateRef.current = true;
        setErrorMessage('');
        checkMovieDownloadState(url);
    }

    const changeVideoTime = (newTime) =>{
        videoRef.current.currentTime = newTime;
    }

    const displayControls = () => {
        setShowVideoControls(true);
        videoPlayerContainerRef.current.style.cursor = 'auto';
        lastTimeMouseMovedRef.current = Date.now();
        const waitingTime = 3000;
        setTimeout(() => {
            if (videoPlayerContainerRef?.current && Date.now() > lastTimeMouseMovedRef.current + waitingTime){
                setShowVideoControls(false);
                videoPlayerContainerRef.current.style.cursor = 'none';
            }
        }, waitingTime + 500);
    };

    const onFullScreenStateChanged = (fullScreen) => {
        if(fullScreen){
            if (!videoPlayerContainerRef.current) return;

            if (videoPlayerContainerRef.current.requestFullscreen) videoPlayerContainerRef.current.requestFullscreen();
            else if (videoPlayerContainerRef.current.mozRequestFullScreen) videoPlayerContainerRef.current.mozRequestFullScreen();
            else if (videoPlayerContainerRef.current.webkitRequestFullscreen) videoPlayerContainerRef.current.webkitRequestFullscreen();
            else if (videoPlayerContainerRef.current.msRequestFullscreen) videoPlayerContainerRef.current.msRequestFullscreen();
        }
        else
            document.exitFullscreen();     
    }

    const playVideo =()=>{
        if(videoRef?.current)
            videoRef.current.play();
    }

    const pauseVideo = ()=>{
        if(videoRef?.current)
            videoRef.current.pause();
    }

    const changeVideoVolume = (newVolume) => {
        if(videoRef?.current)
            videoRef.current.volume = newVolume;
    }

    useEffect(() => {
        videoPlayerContainerRef.current.addEventListener("mousemove", displayControls);

        return () => {
            if (videoPlayerContainerRef?.current) videoPlayerContainerRef.current.removeEventListener("mousemove", displayControls);
        }
    }, [videoPlayerContainerRef]);

    useEffect(() => {
        if(!Boolean(videoRef?.current))
            return;
            
        const videoErrorHandler = (e) => {
            if (videoRef?.current?.attributes.src.value) {
                setErrorMessage(errorMessage ? errorMessage : "An error occured");
                setVideoIsLoading(false);
            }
        }
        videoRef.current.addEventListener('error', videoErrorHandler, true);

        const onVideoReady = () => {        
            checkDownloadStateRef.current = false;
            setVideoDownloadState('');
            setVideoIsLoading(false);
        };
        videoRef.current.addEventListener('canplay', onVideoReady, true);

        const onVideoPlaying = () => {        
            onVideoReady();
            setVideoIsPlaying(true);
        };
        videoRef.current.addEventListener('playing', onVideoPlaying, true);

        const onVideoPaused = () => {        
            setVideoIsPlaying(false);
        };
        videoRef.current.addEventListener('pause', onVideoPaused, true);

        const onVideoWaiting = () => {     
            setVideoIsLoading(true);
        };
        videoRef.current.addEventListener('waiting', onVideoWaiting, true);

        const onTimeUpdated = () =>{
            if (videoRef.current?.currentTime)
                setCurrentTime(videoRef.current?.currentTime);
        };
        videoRef.current.addEventListener('timeupdate', onTimeUpdated);

        return () => {
            if (videoRef?.current) {
                videoRef.current.removeEventListener("error", videoErrorHandler);
                videoRef.current.removeEventListener('playing', onVideoReady);
                videoRef.current.removeEventListener('canplay', onVideoReady);
                videoRef.current.removeEventListener('waiting', onVideoWaiting);
                videoRef.current.removeEventListener('pause', onVideoPaused);
                videoRef.current.removeEventListener('timeupdate', onTimeUpdated);
            }
        }
    }, [videoRef]);

    useEffect(()=>{
        const onFullScreenChange = (e) => {
            setIsFullScreen(document.fullscreenElement);
        };

        const onKeyboardPress = (e) =>{

            if(!videoRef?.current) return;

            displayControls();

            if(e.keyCode === 32){
                if(videoRef.current.paused) playVideo();
                else pauseVideo();
            }
            else if(e.keyCode === 37)
                changeVideoTime(videoRef.current?.currentTime - 10);
            else if(e.keyCode === 39)
                changeVideoTime(videoRef.current?.currentTime + 10);
        }
        if (videoPlayerContainerRef.current){
            videoPlayerContainerRef.current.addEventListener('fullscreenchange', onFullScreenChange);
            document.addEventListener('keyup', onKeyboardPress, false);
        }

        return () => {
            if (videoPlayerContainerRef.current){
                document.removeEventListener('keyup', onKeyboardPress);
                videoPlayerContainerRef.current.removeEventListener('fullscreenchange', onFullScreenChange);
            } 
        }
    },[]);

    useEffect(()=>{
        if(mustPauseVideo){
            pauseVideo();
        }
            
    },[mustPauseVideo]);

    useEffect(() => {
        if (videoQualitiesOptions) {
            const videoQuality = videoQualitiesOptions.find(op => op.selected);
            if (videoQuality?.data?.url) changeVideoSource(videoQuality.data.url);
        }
    }, [videoQualitiesOptions]);

    return (
        <div ref={videoPlayerContainerRef} className="video-player-content" >
            <CircularProgressBar color={'white'} size={'80px'} position={"center"} visible={videoSource && videoIsLoading} text={videoDownloadState} />
            <div className="video-player-error-message" style={{ display: errorMessage ? '' : 'none' }}>{errorMessage}</div>
            <video ref={videoRef} className="video-player" src={videoSource}></video>
            <div className="video-player-bottom">
                <VideoSubtitles
                    videoCurrentTime={currentTime}
                    urlSource={subtitlesUrl}
                    size={subtitlesSize}
                    subtitlesAdjustTime={subtitlesAdjustTime} />
                <div className="video-player-controls-container" style={fadeTransition(showVideoControls)}>
                    <TimeController 
                        videoDuration={videoRef.current?.duration}
                        videoCurrentTime={currentTime}
                        onTimeChanged={(newTime) => changeVideoTime(newTime)} />
                    <Controls
                        videoIsPlaying={videoIsPlaying}
                        onPlayClick={() => playVideo()}
                        onPauseClick={() => pauseVideo()}
                        onVolumeChanged={(newVolume)=> changeVideoVolume(newVolume)}
                        onPlayBackwardClick={() => changeVideoTime(currentTime - 10)}
                        onPlayForwardClick={() => changeVideoTime(currentTime + 10)}
                        videoSubtitlesOptions={videoSubtitlesOptions}
                        videoQualitiesOptions={videoQualitiesOptions}
                        onSubtitleSizeChange={(pixelsToAdd) => setSubtitlesSize(subtitlesSize + pixelsToAdd <= 60 && subtitlesSize + pixelsToAdd >= 20 ? subtitlesSize + pixelsToAdd : subtitlesSize)}
                        onSubtitlesChange={(newUrlSource) => setSubtitlesUrl(newUrlSource ? newUrlSource : '')}
                        onAdjustSubtitleTimeChange={(time) => setSubtitlesAdjustTime(time ? time : 0)}
                        onVideoQualityChange={(newUrlSource) => changeVideoSource(newUrlSource)}
                        isFullScreen={isFullScreen}
                        onFullScreenStateChanged={(fullscreen) => onFullScreenStateChanged(fullscreen)} />
                </div>
            </div>
        </div>
    );
}
export default VideoPlayer;
