import LeftControlsGroup from "./LeftControlsGroup";
import RightControlsGroup from "./RightControlsGroup";

function Controls({
    onPlayClick,
    onPauseClick,
    onVolumeChanged,
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
                onPlayClick={() => onPlayClick()} 
                onPauseClick={() => onPauseClick()} 
                onVolumeChanged={(newVolume) => onVolumeChanged(newVolume)}/>
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