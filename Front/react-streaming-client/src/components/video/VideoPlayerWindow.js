
import "../../style/video-player-window.css";
import "../../style/button.css";
import MoviesAPI from "../../js/moviesAPI";
import VideoPlayer from "./VideoPlayer";

import { useEffect, useState, useRef } from 'react';
import fadeTransition from "../../js/customStyles.js";
import { useOnClickOutside} from '../../js/customHooks';

function VideoPlayerWindow({ movie, visible, onCloseClick }) {
    const [subtitlesOptions, setSubtitlesOptions] = useState([]);
    const [videoQualitiesOptions, setVideoQualitiesOptions] = useState([]);
    const videoPlayerContainerRef = useRef(null);

    const buildVideoQualitiesOptions = (movie) => {
        var options = [];
       if(!movie.torrents) return;
        movie.torrents.forEach(t => {
            var qualities = options.filter(o => o.label.startsWith(t.quality));

            var option = {
                label: qualities && qualities.length > 0 ? t.quality + ' (' + (qualities.length + 1) + ')': t.quality,
                selected: false,
                data: {
                    url: MoviesAPI.apiStreamUrl + t.downloadUrl
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
            sub.subtitlesIds.forEach((id, index) => {
                subtitlesOption.subOptions.push({
                    label: sub.language + ' ' + (index + 1),
                    selected: false,
                    data: {
                        url: MoviesAPI.apiSubtitlesUrl + id
                    }
                });
            })
            newSubtitlesOptions.push(subtitlesOption);
        });
        setSubtitlesOptions(newSubtitlesOptions);
    }

    useEffect(() => {
        if (visible && movie?.imdbCode) {
            MoviesAPI.getAvailableSubtitles(movie.imdbCode,
                (availableSubtitles) => {
                    buildSubtitlesOptions(availableSubtitles);
                })
            buildVideoQualitiesOptions(movie);
        }
    }, [movie, visible]);

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