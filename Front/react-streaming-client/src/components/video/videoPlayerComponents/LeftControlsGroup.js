import PlayArrowIcon from '@material-ui/icons/PlayArrow';
import PauseIcon from '@material-ui/icons/Pause';
import VolumeUp from '@material-ui/icons/VolumeUp';
import VolumeOffIcon from '@material-ui/icons/VolumeOff';

import Slider from './Slider';

import { useEffect, useState } from 'react';

function LeftControlsGroup({ onPlayClick, onPauseClick, onVolumeChanged }) {
    const [showPlayButton, setShowPlayButton] = useState(true);

    const togglePlayButton = (play) => {
        setShowPlayButton(!play);
        if (play) onPlayClick();
        else onPauseClick();
    }

    return (
        <div className="controls-group">
            <PlayArrowIcon className="icon" style={{ display: showPlayButton ? '' : 'none' }} onClick={() => togglePlayButton(true)} />
            <PauseIcon className="icon" style={{ display: showPlayButton ? 'none' : '' }} onClick={() => togglePlayButton(false)} />
            <VolumeController onVolumeChanged={(newVolume) => onVolumeChanged(newVolume)} />
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
