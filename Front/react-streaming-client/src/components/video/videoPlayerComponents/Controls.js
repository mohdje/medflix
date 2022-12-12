import LeftControlsGroup from "./LeftControlsGroup";
import RightControlsGroup from "./RightControlsGroup";

function Controls({
    videoPaused,
    onPlayClick,
    onPauseClick,
    onVolumeChanged,
    onPlayBackwardClick,
    onPlayForwardClick,
    videoSubtitlesOptions, 
    videoQualitiesOptions,
    onSubtitlesChange,
    onSubtitleSizeChange,
    onAdjustSubtitleTimeChange,
    onVideoQualityChange,
    isFullScreen,
    onFullScreenStateChanged }) {
        
    return (
        <div className="video-player-controls">
            <LeftControlsGroup 
                videoPaused = {videoPaused}
                onPlayClick={() => onPlayClick()} 
                onPauseClick={() => onPauseClick()} 
                onVolumeChanged={(newVolume) => onVolumeChanged(newVolume)}
                onPlayBackwardClick={()=> onPlayBackwardClick()}
                onPlayForwardClick={()=> onPlayForwardClick()}/>
            <RightControlsGroup
                subtitlesOptions={videoSubtitlesOptions}
                qualityOptions={videoQualitiesOptions}
                onSubtitleSizeChange={(pixelsToAdd) => onSubtitleSizeChange(pixelsToAdd)}
                onSubtitlesChange={(newUrlSource) => onSubtitlesChange(newUrlSource)}
                onAdjustSubtitleTimeChange={(time) => onAdjustSubtitleTimeChange(time)}
                onVideoQualityChange={(newUrlSource) => onVideoQualityChange(newUrlSource)} 
                isFullScreen={isFullScreen}
                onFullScreenStateChanged={(fullscreen) => onFullScreenStateChanged(fullscreen)}/>
        </div>
    );
}

export default Controls;