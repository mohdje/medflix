import PlayArrowIcon from '@material-ui/icons/PlayArrow';
import PauseIcon from '@material-ui/icons/Pause';
import VolumeUp from '@material-ui/icons/VolumeUp';
import VolumeOffIcon from '@material-ui/icons/VolumeOff';
import Forward10Icon from '@material-ui/icons/Forward10';
import Replay10Icon from '@material-ui/icons/Replay10';

import Slider from './Slider';

import { useEffect, useState, useRef } from 'react';

function LeftControlsGroup({ videoIsPlaying, onPlayClick, onPauseClick, onVolumeChanged, onPlayBackwardClick, onPlayForwardClick }) {
    const [showPlayButton, setShowPlayButton] = useState(true);

    useEffect(() => {
        setShowPlayButton(!videoIsPlaying);
    }, [videoIsPlaying]);

    return (
        <div className="controls-group">
            <PlayArrowIcon className="icon" style={{ display: showPlayButton ? '' : 'none' }} onClick={() => onPlayClick()} />
            <PauseIcon className="icon" style={{ display: showPlayButton ? 'none' : '' }} onClick={() => onPauseClick()} />
            <VolumeController onVolumeChanged={(newVolume) => onVolumeChanged(newVolume)} />
            <Replay10Icon className="icon" onClick={() => onPlayBackwardClick()}/>
            <Forward10Icon className="icon" onClick={() => onPlayForwardClick()}/>
        </div>)
}

export default LeftControlsGroup;


function VolumeController({ onVolumeChanged }) {
    const defaultVolume = 70;
    const currentVolumeRef = useRef(0);
    const [previousVolume, setPreviousVolume] = useState(defaultVolume);

    const changeVolume = (percentage) => {
        currentVolumeRef.current = percentage;
        onVolumeChanged(percentage / 100);
    };

    const muteVideo = () => {
        setPreviousVolume(currentVolumeRef.current);
        changeVolume(0);
    }

    useEffect(()=>{
        changeVolume(defaultVolume);
      
        const onKeyboardPress = (e) =>{
            if(e.keyCode === 38 && currentVolumeRef.current < 100)
                changeVolume(currentVolumeRef.current + 10);
            else if(e.keyCode === 40 && currentVolumeRef.current > 0)
                changeVolume(currentVolumeRef.current - 10);
        }

        document.addEventListener('keyup', onKeyboardPress, false);

        return () => {
            document.removeEventListener('keyup', onKeyboardPress);
        }
    },[]);

    return (<div className="video-volume-controller">
        <VolumeUp className="icon" onClick={() => { muteVideo() }} style={{ display: currentVolumeRef.current > 0 ? '' : 'none' }} />
        <VolumeOffIcon className="icon" onClick={() => changeVolume(previousVolume)} style={{ display: currentVolumeRef.current > 0 ? 'none' : '' }} />
        <Slider
            value={currentVolumeRef.current}
            onCursorMoved={(percentage) => changeVolume(percentage)}
            onCursorEndMove={(percentage) => changeVolume(percentage)}
            width={'120px'}
            height={'4px'} />
    </div>)
}
