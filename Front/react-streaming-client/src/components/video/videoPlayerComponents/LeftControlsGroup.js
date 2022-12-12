import PlayArrowIcon from '@material-ui/icons/PlayArrow';
import PauseIcon from '@material-ui/icons/Pause';
import VolumeUp from '@material-ui/icons/VolumeUp';
import VolumeOffIcon from '@material-ui/icons/VolumeOff';
import Forward10Icon from '@material-ui/icons/Forward10';
import Replay10Icon from '@material-ui/icons/Replay10';

import Slider from './Slider';

import { useEffect, useState } from 'react';

function LeftControlsGroup({ videoPaused, onPlayClick, onPauseClick, onVolumeChanged, onPlayBackwardClick, onPlayForwardClick }) {
    const [showPlayButton, setShowPlayButton] = useState(true);

    const togglePlayButton = (play) => {
        setShowPlayButton(!play);
        if (play) onPlayClick();
        else onPauseClick();
    }

    useEffect(() => {
        if (videoPaused) setShowPlayButton(true);
    }, [videoPaused]);

    return (
        <div className="controls-group">
            <PlayArrowIcon className="icon" style={{ display: showPlayButton ? '' : 'none' }} onClick={() => togglePlayButton(true)} />
            <PauseIcon className="icon" style={{ display: showPlayButton ? 'none' : '' }} onClick={() => togglePlayButton(false)} />
            <VolumeController onVolumeChanged={(newVolume) => onVolumeChanged(newVolume)} />
            <Replay10Icon className="icon" onClick={() => onPlayBackwardClick()}/>
            <Forward10Icon className="icon" onClick={() => onPlayForwardClick()}/>
        </div>)
}

export default LeftControlsGroup;


function VolumeController({ onVolumeChanged }) {

    const defaultVolume = 70;
    const [currentVolume, setCurrentVolume] = useState(defaultVolume);
    const [previousVolume, setPreviousVolume] = useState(defaultVolume);

    const changeVolume = (percentage) => {
        setCurrentVolume(percentage);
        onVolumeChanged(percentage / 100);
    };

    const muteVideo = () => {
        setPreviousVolume(currentVolume);
        changeVolume(0);
    }

    useEffect(() => {
        changeVolume(defaultVolume);
    }, []);

    return (<div className="video-volume-controller">
        <VolumeUp className="icon" onClick={() => { muteVideo() }} style={{ display: currentVolume > 0 ? '' : 'none' }} />
        <VolumeOffIcon className="icon" onClick={() => changeVolume(previousVolume)} style={{ display: currentVolume > 0 ? 'none' : '' }} />
        <Slider
            value={currentVolume}
            onCursorMoved={(percentage) => changeVolume(percentage)}
            onCursorEndMove={(percentage) => changeVolume(percentage)}
            width={'120px'}
            height={'4px'} />
    </div>)
}
