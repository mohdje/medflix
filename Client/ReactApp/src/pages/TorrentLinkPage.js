
import "../style/css/torrent-link-page.css";

import TextInput from '../components/common/TextInput';
import Button from "../components/common/Button";
import CircularProgressBar from '../components/common/CircularProgressBar';
import { torrentApi } from "../services/api";

import { useEffect, useState, useRef } from 'react';

function TorrentLinkPage() {

    const [torrentLink, setTorrentLink] = useState('');
    const torrentLinkRef = useRef('');
    const [openingTorrentLink, setOpeningTorrentLink] = useState(false);
    const [openingHistory, setOpeningHistory] = useState(false);
    const [torrentHistory, setTorrentHistory] = useState([]);
    const [torrentFiles, setTorrentFiles] = useState([]);
    const [selectedFile, setSelectedFile] = useState('');
    const [loadingMessage, setLoadingMessage] = useState('');

    const [showMoviePlayer, setShowMoviePlayer] = useState(false);

    const [movieSources, setMovieSources] = useState([]);


    useEffect(() => {
        setMovieSources([{
            quality: 'Video',
            downloadUrl: torrentLink,
            fileName: selectedFile
        }]);

        if (!torrentLink)
            getTorrentHistory();

        torrentLinkRef.current = torrentLink;

    }, [torrentLink, selectedFile]);

    const getTorrentHistory = async () => {
        setOpeningHistory(true);
        setLoadingMessage('Loading history');
        const history = await torrentApi.getTorrentHistory();
        setOpeningHistory(false);
        setTorrentHistory(history);
    };

    const changeTorrentLink = (torrentLink) => {
        setOpeningTorrentLink(false);
        setSelectedFile('');
        setTorrentLink(torrentLink);
        setTorrentFiles([]);
    };

    const openTorrent = async () => {
        setTorrentFiles([]);
        setOpeningTorrentLink(true);
        setLoadingMessage('Opening torrent');

        const response = await torrentApi.getTorrentFiles(torrentLink);
        if (decodeURIComponent(torrentLinkRef.current) === decodeURIComponent(response.link)) {
            setOpeningTorrentLink(false);
            setTorrentFiles(response.files);
        }
    };

    const onPlayFileClick = (file) => {
        setSelectedFile(file);
        setShowMoviePlayer(true);
    }

    const torrentFilesAreLoadingOrLoaded = () => {
        return openingTorrentLink || (torrentFiles && torrentFiles.length > 0);
    }


    return (
        <div className="torrent-link-page-container">
            <div className="title">Enter a torrent url or magnet and click on Open to see the list of files</div>
            <TextInput placeHolder="Torrent link or magnet..." large onTextChanged={(text) => changeTorrentLink(text)} value={torrentLink} />

            <div style={{ margin: "10px 0" }}>
                <Button text="Open" color="red" onClick={() => openTorrent()} visible={!torrentFilesAreLoadingOrLoaded()} />
                {Boolean(openingTorrentLink) || Boolean(openingHistory) ? <CircularProgressBar visible size="medium" text={loadingMessage} /> : null}
            </div>
            <TorrentHistory visible={!torrentFilesAreLoadingOrLoaded()} files={torrentHistory} onClick={(torrentLink) => { changeTorrentLink(torrentLink) }} />
            <TorrentFilesList visible={torrentFilesAreLoadingOrLoaded()} torrentLink={torrentLink} files={torrentFiles} onPlayFileClick={(file) => onPlayFileClick(file)} />

            {/* <VideoPlayerWindow visible={showMoviePlayer} sources={movieSources} onCloseClick={() => setShowMoviePlayer(false)} onWatchedTimeUpdate={() => { }} /> */}
        </div>
    )
}

export default TorrentLinkPage;

function TorrentFilesList({ visible, torrentLink, files, onPlayFileClick }) {
    if (!visible)
        return null;

    return <FilesList
        torrentLink={torrentLink}
        files={files}
        onPlayFileClick={(file) => onPlayFileClick(file)}
        contentType="fileFromTorrent"
        listTitle="Torrent files" />
}

function TorrentHistory({ visible, files, onClick }) {
    if (!visible)
        return null;

    return <FilesList
        files={files}
        onFileClick={(torrentLink) => onClick(torrentLink)}
        contentType="torrentFile"
        listTitle="Last opened torrent files" />
}

function FilesList({ torrentLink, files, onFileClick, onPlayFileClick, contentType, listTitle }) {

    const [selectedFileIndex, setSelectedFileIndex] = useState(null);

    useEffect(() => {
        setSelectedFileIndex(null);
    }, [files]);


    if (!files || files.length === 0)
        return null;

    const fileListContainerStyle = {
        width: '50%'
    };

    const fileListStyle = {
        background: 'linear-gradient(342deg, rgba(42,42,42,1) 0%, rgba(0,0,0,1) 65%)',
        borderRadius: '15px',
        padding: '5px 0',
        maxHeight: '300px',
        overflowY: 'auto'
    };

    const textStyle = {
        padding: '10px 7px',
        fontSize: '18px',
        fontWeight: '500',
        borderRadius: '7px',
        cursor: 'pointer'
    };

    const selectedTextStyle = {
        ...textStyle,
        background: 'linear-gradient(90deg, rgba(187,0,0,1) 0%, rgba(193,76,76,1) 35%, rgba(0,0,0,1) 100%)'
    };

    const titleStyle = {
        ...textStyle,
        fontSize: '20px',
        color: '#858585'
    };


    const getComponent = (file, index) => {
        if (contentType === 'fileFromTorrent') {
            return <FileFromTorrent
                torrentLink={torrentLink}
                file={file}
                isSelected={index === selectedFileIndex}
                onClick={() => setSelectedFileIndex(index)}
                onPlayFileClick={() => onPlayFileClick(file)} />
        }
        else if (contentType === 'torrentFile') {
            return <TorrentLink file={file} onClick={(torrentLink) => onFileClick(torrentLink)} />
        }
    };

    const onMouseHover = (index, enter) => {
        if (index !== selectedFileIndex) {
            const element = document.getElementById('idfile_' + index);
            element.style.background = enter ? '#362b2b' : '';
        }
    }

    return (
        <div style={fileListContainerStyle}>
            <div style={titleStyle}>{listTitle}</div>
            <div style={fileListStyle}>
                {files.map((file, index) =>
                    <div
                        id={'idfile_' + index}
                        key={index}
                        onMouseEnter={() => { onMouseHover(index, true) }}
                        onMouseLeave={() => { onMouseHover(index, false) }} style={index === selectedFileIndex ? selectedTextStyle : textStyle}>
                        {getComponent(file, index)}
                    </div>
                )}
            </div>
        </div>

    )
}

function FileFromTorrent({ torrentLink, file, isSelected, onClick, onPlayFileClick }) {
    let playButtons = null;
    if (isSelected) {
        playButtons = (
            <div className="play-buttons-container">
                <Button text="Play" color="red" onClick={() => onPlayFileClick()} />
            </div>)
    }

    return (
        <div
            onClick={() => onClick()}>
            {file}
            {playButtons}
        </div>
    );
}

function TorrentLink({ file, onClick }) {
    const dateStyle = {
        textAlign: 'left',
        color: 'grey'
    };

    const formateDate = (fullDateTime) => {
        const today = new Date();

        const yesterday = new Date();
        yesterday.setDate(yesterday.getDate() - 1);

        const date = new Date(fullDateTime);

        let day = '';
        if (yesterday.toDateString() === date.toDateString())
            day = 'Yesterday';
        else if (today.toDateString() === date.toDateString())
            day = 'Today';
        else {
            let options = { year: "numeric", month: "long", day: "numeric" };
            day = date.toLocaleString('en-US', options);
        }

        const time = date.toLocaleTimeString(navigator.language, {
            hour: '2-digit',
            minute: '2-digit',
            hour12: false
        });
        return day + ', ' + time;
    }

    return (
        <div onClick={() => onClick(file.link)}>
            <div style={dateStyle}>{formateDate(file.lastOpenedDateTime)}</div>
            <div style={{ textAlign: 'left' }}>{file.link}</div>
        </div>
    );
}