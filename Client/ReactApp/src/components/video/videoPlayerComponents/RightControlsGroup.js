
// import SettingsIcon from '@material-ui/icons/Settings';
// import SubtitlesIcon from '@material-ui/icons/Subtitles';
// import FullscreenIcon from '@material-ui/icons/Fullscreen';
// import FullscreenExitIcon from '@material-ui/icons/FullscreenExit';
// import SortByAlphaIcon from '@material-ui/icons/SortByAlpha';
// import RestoreIcon from '@material-ui/icons/Restore';

import { VideoOptions, VideoOptionsButton } from './Options';

import { useState, useRef, useEffect } from 'react';
import { useOnClickOutside } from '../../../helpers/customHooks';

function RightControlsGroup({
    subtitlesOptions,
    qualityOptions,
    onSubtitleSizeChange,
    onSubtitlesChange,
    onAdjustSubtitleTimeChange,
    onVideoQualityChange,
    isFullScreen,
    onFullScreenStateChanged }) {

    const [videoInFullscreen, setVideoInFullscreen] = useState(false);

    const changeFullScreenState = (fullscreen) => {
        onFullScreenStateChanged(fullscreen);
    };

    useEffect(() => {
        setVideoInFullscreen(isFullScreen);
    }, [isFullScreen]);

    return (
        <div className="controls-group">
            <div className="controls-sub-group">
                {/* <VideoOptions options={subtitlesOptions}
                    icon={<SubtitlesIcon className="icon" />}
                    onOptionChanged={(option) => onSubtitlesChange(option?.data?.url ? option.data.url : '')}
                />
                <AdjustSubtitleTimeOption onAdjustSubtitleTimeChange={(time) => onAdjustSubtitleTimeChange(time)} />
                <VideoOptionsButton icon={<SortByAlphaIcon className="icon big" />} onClick={() => onSubtitleSizeChange(2)} />
                <VideoOptionsButton icon={<SortByAlphaIcon className="icon small" />} onClick={() => onSubtitleSizeChange(-2)} /> */}
            </div>

            {/* <VideoOptions options={qualityOptions}
                icon={<SettingsIcon className="icon" />}
                onOptionChanged={(option) => onVideoQualityChange(option?.data?.url ? option.data.url : '')}
            /> */}

            {/* <VideoOptionsButton icon={<FullscreenIcon className="icon big" />} hide={videoInFullscreen} onClick={() => changeFullScreenState(true)} />
            <VideoOptionsButton icon={<FullscreenExitIcon className="icon big" />} hide={!videoInFullscreen} onClick={() => changeFullScreenState(false)} /> */}
        </div>)
}
export default RightControlsGroup;

function AdjustSubtitleTimeOption({ onAdjustSubtitleTimeChange }) {

    const [showMenu, setShowMenu] = useState(false);
    const menuRef = useRef(null);

    useOnClickOutside(menuRef, () => setShowMenu(false));

    return (
        <div ref={menuRef} className="adjust-subtitle-time-container">
            <div className={"video-options-menu" + (showMenu ? '' : ' hidden')}>
                <div className="label">Adj. Sub.</div>
                <div className="adjust-subtitle-time-input-container">
                    <input type="number" step="0.01" onChange={(e) => onAdjustSubtitleTimeChange(e.target.valueAsNumber)} /><span className="label"> s</span>
                </div>
            </div>
            {/* <VideoOptionsButton icon={<RestoreIcon className="icon" />} onClick={() => setShowMenu(!showMenu)} /> */}
        </div>
    );
}

