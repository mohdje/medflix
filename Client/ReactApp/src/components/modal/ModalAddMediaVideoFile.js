import ModalWindow from "./ModalWindow";
import CircularProgressBar from "../common/CircularProgressBar";
import TextInput from "../common/TextInput";
import MediasHorizontalList from "../media/list/MediasHorizontalList";
import MediaLitePresentation from "../media/presentation/MediaLitePresentation";
import Badge from "../common/Badge";
import Button from "../common/Button";
import AppMode from "../../services/appMode";
import { searchMedias } from "../../services/api/mediasInfoApi";

import { useEffect, useState, useRef } from "react";

function ModalAddMediaVideoFile({ visible, onUploadFileClick, onCloseClick }) {
    const [isLoading, setIsLoading] = useState(false);
    const [searchResults, setSearchResults] = useState({ movies: [], series: [] });
    const [selectedMedia, setSelectedMedia] = useState(null);
    const [selectedMediaIsTvShow, setSelectedMediaIsTvShow] = useState(null);
    const [seasonNumber, setSeasonNumber] = useState(null);
    const [episodeNumber, setEpisodeNumber] = useState(null);
    const languages = [
        {
            value: 0,
            label: "Original"
        },
        {
            value: 1,
            label: "French"
        }
    ]
    const [selectedLanguage, setLanguage] = useState(languages[0]);
    const videoQualities = ["480p", "720p", "1080p", "4K", "Unknown"];
    const [selectedVideoQuality, setVideoQuality] = useState(videoQualities[0]);
    const [selectedFile, setSelectedFile] = useState(null);
    const fileInputRef = useRef(null);
    const searchTimeoutRef = useRef(null);

    useEffect(() => () => clearTimeout(searchTimeoutRef.current), []);

    useEffect(() => {
        setSelectedFile(null);
        if (fileInputRef.current) {
            fileInputRef.current.value = '';
        }
    }, [visible]);

    const containerStyle = {
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        width: '100%',
        height: '100%',
        overflowY: 'auto',
        padding: '20px',
    }

    const sectionStyle = {
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center',
        marginTop: '50px',
        width: '50%',
        gap: '20px',
        textAlign: 'center'
    }

    const onTitleChanged = (text) => {
        setSelectedMedia(null);
        clearTimeout(searchTimeoutRef.current);

        if (text?.length < 3) {
            setSearchResults({ movies: [], series: [] });
            setIsLoading(false);
            return;
        }

        searchTimeoutRef.current = setTimeout(async () => {
            setIsLoading(true);
            setSearchResults({ movies: [], series: [] });
            const [movies, series] = await Promise.all(AppMode.modes.map((mode) => searchMedias(text, mode)));
            setSearchResults({ movies, series });
            setIsLoading(false);
        }, 1500);
    }

    const onMediaSelected = (media, isTvShow) => {
        setSearchResults({ movies: [], series: [] });
        setSelectedMedia(media);
        setSelectedMediaIsTvShow(isTvShow);
    }

    const showNextSteps = selectedMedia && (!selectedMediaIsTvShow || (selectedMediaIsTvShow && seasonNumber && episodeNumber));

    const modalContent = (<div style={containerStyle}>
        <h1>Add Media Video File</h1>
        <div style={sectionStyle}>
            <h3>1. Search and select the media you want to add a video for</h3>
            {!selectedMedia && <TextInput placeHolder="Enter movie or tv show name..." onTextChanged={(text) => onTitleChanged(text)} />}
            {isLoading && <CircularProgressBar visible={isLoading} size="large" />}
            {selectedMedia && <MediaLitePresentation media={selectedMedia} onMediaClick={() => setSelectedMedia(null)} />}
        </div>
        {searchResults.movies.length > 0 && <MediasHorizontalList title="Movies" centerTitle medias={searchResults.movies} onMediaClick={(media) => onMediaSelected(media, false)} />}
        {searchResults.series.length > 0 && <MediasHorizontalList title="Series" centerTitle medias={searchResults.series} onMediaClick={(media) => onMediaSelected(media, true)} />}
        <div style={{ display: (selectedMedia && selectedMediaIsTvShow) ? 'flex' : 'none', gap: '50px', justifyContent: 'center' }}>
            <div style={{ ...sectionStyle, width: '30%', maxWidth: '100px' }}>
                <h3>Season</h3>
                <TextInput placeHolder="Season number" onTextChanged={(text) => setSeasonNumber(text)} integerOnly minValue={1} />
            </div>
            <div style={{ ...sectionStyle, width: '30%', maxWidth: '100px' }}>
                <h3>Episode</h3>
                <TextInput placeHolder="Episode number" onTextChanged={(text) => setEpisodeNumber(text)} integerOnly minValue={1} />
            </div>
        </div>
        {showNextSteps &&
            <div style={sectionStyle}>
                <h3>2. Select the language of the video</h3>
                <div>
                    {languages.map((language) => (
                        <Badge key={language.value} text={language.label} onClick={() => setLanguage(language)} active={selectedLanguage?.value === language.value} />
                    ))}
                </div>
            </div>}
        {showNextSteps &&
            <div style={sectionStyle}>
                <h3>3. Select the quality of the video</h3>
                <div>
                    {videoQualities.map((quality) => (
                        <Badge key={quality} text={quality} onClick={() => setVideoQuality(quality)} active={selectedVideoQuality === quality} />
                    ))}
                </div>
            </div>}
        {showNextSteps && <div style={sectionStyle}>
            <h3>4. Select the video file</h3>
            <Button text="Choose file" onClick={() => fileInputRef.current.click()} />
            <input
                ref={fileInputRef}
                style={{ display: 'none' }}
                type="file"
                id="fileInput"
                accept=".mp4,.mkv,.avi"
                onChange={(event) => setSelectedFile(event.target.files[0] ?? null)}
            />
            <h4>{selectedFile?.name}</h4>
        </div>}
        {showNextSteps && selectedFile && (
            <div style={{ marginTop: '30px' }}>
                <Button text="Upload File" color="red" large onClick={() => onUploadFileClick(selectedMedia.id, seasonNumber, episodeNumber, selectedLanguage, selectedVideoQuality, selectedFile)} />
            </div>
        )}
    </div >);

    return (
        <ModalWindow visible={visible} content={modalContent} onCloseClick={onCloseClick} />
    )
}

export default ModalAddMediaVideoFile;
