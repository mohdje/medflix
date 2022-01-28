
import "../../style/video-player-window.css";
import "../../style/button.css";
import MoviesAPI from "../../js/moviesAPI";
import VideoPlayer from "./VideoPlayer";

import { useEffect, useState, useRef } from 'react';
import fadeTransition from "../../js/customStyles.js";
import { useOnClickOutside} from '../../js/customHooks';

function VideoPlayerWindow({ sources, subtitles, visible, onCloseClick }) {
    const [subtitlesOptions, setSubtitlesOptions] = useState([]);
    const [videoQualitiesOptions, setVideoQualitiesOptions] = useState([]);
    const videoPlayerContainerRef = useRef(null);

    const buildVideoQualitiesOptions = (sources) => {
        var options = [];
       if(!sources) return;
       sources.forEach(source => {
            var qualities = options.filter(o => o.label.startsWith(source.quality));

            var option = {
                label: qualities && qualities.length > 0 ? source.quality + ' (' + (qualities.length + 1) + ')': source.quality,
                selected: false,
                data: {
                    url: MoviesAPI.apiStreamUrl + source.downloadUrl
                }
            }
            options.push(option);
        });
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
                        url: MoviesAPI.getSubtitlesApiUrl(sourceUrl)
                    }
                });
            })
            newSubtitlesOptions.push(subtitlesOption);
        });
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

    useOnClickOutside(videoPlayerContainerRef, () => onCloseClick());

    return (
        <div style={fadeTransition(visible)} className="video-player-window-container">
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