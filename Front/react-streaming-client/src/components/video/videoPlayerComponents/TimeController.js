
import VideoPlayerSlider from './Slider';

import { useEffect, useState } from 'react';

function TimeController({ videoDuration, videoCurrentTime, onTimeChanged }) {
    const [videoCurrentTimeLabel, setVideoCurrentTimeLabel] = useState('00:00');
    const [videoTotalTimeLabel, setVideoTotalTimeLabel] = useState('00:00');
    const [currentPourcentageTimeVideo, setcurrentPourcentageTimeVideo] = useState(0);
    const [tooltip, setTooltip] = useState(null);
    const [totalTime, setTotalTime] = useState(0);

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
        if (totalTime && totalTime > 0) {
            var newTime = Math.floor((percentage * totalTime) / 100)
            setVideoCurrentTimeLabel(getTimeLabel(newTime));
            onTimeChanged(newTime);
        }
    }

    const updateTimeVideo = (time) => {
        setVideoCurrentTimeLabel(getTimeLabel(time));
        if(totalTime && totalTime > 0)
            setcurrentPourcentageTimeVideo((time) * 100 / totalTime);
        else
            setcurrentPourcentageTimeVideo(0);
    }

    const setTimeControllerTooltip = (percentage) => {
        if (totalTime && totalTime > 0) {
            setTooltip({
                text: getTimeLabel(Math.floor((percentage * totalTime) / 100)),
                position: percentage
            })
        }
    }

    useEffect(() => {
        if (videoCurrentTime)
            updateTimeVideo(videoCurrentTime);
    }, [videoCurrentTime]);

    useEffect(() => {
        if (videoDuration){
            setTotalTime(videoDuration);
            setVideoTotalTimeLabel(getTimeLabel(videoDuration));
        }
        else
            setTotalTime(0);
            
    }, [videoDuration]);

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

export default TimeController;