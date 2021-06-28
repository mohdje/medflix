import { useEffect, useState } from 'react';
import { FakeSubtitles } from "../../../js/fakeData";

function VideoSubtitles({ videoCurrentTime, urlSource, size, subtitlesAdjustTime }) {
    const [currentSubtitles, setCurrentSubtitles] = useState('');
    const [subtitles, setSubtitles] = useState([]);
    const [textItalic, setTextItalic] = useState(false);

    const applySubtitles = (currentTime) => {
        if (!subtitles || subtitles.length === 0)
            return;
        
        var subtitle = subtitles.find(s =>
            (s.startTime + subtitlesAdjustTime) <= currentTime && currentTime <= (s.endTime + subtitlesAdjustTime));

        if (subtitle?.text) {
            var text = subtitle.text;
            setTextItalic(false);
            if (text.includes('<i>') || text.includes('</i>')) {
                setTextItalic(true);
                text = text.replace('<i>', '').replace('</i>', '');
            }
            setCurrentSubtitles(text);
        }
        else setCurrentSubtitles('');
    }

    useEffect(() => {
        if (urlSource) {
            var subs = FakeSubtitles.getSubtitles(urlSource);
            setSubtitles(subs);
        }

        return () => {
            setSubtitles([]);
            setCurrentSubtitles('');
        }
    }, [urlSource]);

    useEffect(() => {
        if(videoCurrentTime)
            applySubtitles(videoCurrentTime);
    }, [videoCurrentTime, subtitles, subtitlesAdjustTime]);

    return (<div className="video-player-subtitles"
        style={{ fontSize: size + 'px', fontStyle: textItalic ? 'italic' : '' }}>
        {currentSubtitles}
    </div>);
}

export default VideoSubtitles;