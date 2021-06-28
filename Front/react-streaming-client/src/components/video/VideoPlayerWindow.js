
import "../../style/video-player-window.css";
import "../../style/button.css";
import MoviesAPI from "../../js/moviesAPI";
import VideoPlayer from "./videoPlayerComponents/VideoPlayer";

import { useEffect, useState, useRef } from 'react';
import fadeTransition from "../../js/customStyles.js";
import { useOnClickOutside} from '../../js/customHooks';
import { VideoQualities } from "../../js/fakeData";

function VideoPlayerWindow({ visible, onCloseClick }) {
    const [subtitlesOptions, setSubtitlesOptions] = useState([]);
    const [videoQualitiesOptions, setVideoQualitiesOptions] = useState([]);
    const videoPlayerContainerRef = useRef(null);

    const buildSubtitlesOptions = (subtitles) => {
        var newSubtitlesOptions = [];
        newSubtitlesOptions.push(
            {
                label: 'Off',
                selected: true
            });
        subtitles.forEach(sub => {
            var subtitlesOption = {
                label: sub.language,
                selected: false,
                subOptions: []
            };
            sub.subtitlesIds.forEach((id, index) => {
                subtitlesOption.subOptions.push({
                    label: sub.language + ' ' + (index + 1),
                    selected: false,
                    data: {
                        url: id
                    }
                });
            })
            newSubtitlesOptions.push(subtitlesOption);
        });
        setSubtitlesOptions(newSubtitlesOptions);
    }

    useEffect(() => {
        if (visible) {
            MoviesAPI.getAvailableSubtitles(
                (availableSubtitles) => {
                    buildSubtitlesOptions(availableSubtitles);
                })
                setVideoQualitiesOptions(VideoQualities);
        }
    }, [visible]);

    useOnClickOutside(videoPlayerContainerRef, () => onCloseClick());

    return (
        <div style={fadeTransition(visible)} className="video-player-window-container">
             <div className="demo-version-info">This is a demo version. For this version, a free short video is played (to test the video player) instead of the movie you selected. 
                If you want to watch movies you have to use the .Net application available here <a href="https://github.com/mohdje/medflix">https://github.com/mohdje/medflix</a>.
                This application embeds the full version of this react front end app.
            </div>
            <div ref={videoPlayerContainerRef} className="video-player-container">
                <VideoPlayer 
                    videoQualitiesOptions={videoQualitiesOptions} 
                    videoSubtitlesOptions={subtitlesOptions}
                    mustPauseVideo={!visible} />
            </div>
        </div>
    );
}
export default VideoPlayerWindow;