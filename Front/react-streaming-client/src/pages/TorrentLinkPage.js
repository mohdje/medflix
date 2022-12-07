
import "../style/torrent-link-page.css";

import TextInput from '../components/common/TextInput';
import OpenButton from '../components/common/buttons/OpenButton';
import CircularProgressBar from '../components/common/CircularProgressBar';
import PlayButton from '../components/common/buttons/PlayButton';
import PlayWithVLCButton from '../components/common/buttons/PlayWithVLCButton';
import VideoPlayerWindow from '../components/video/VideoPlayerWindow';
import MoviesAPI from "../js/moviesAPI";

import { useEffect, useState } from 'react';

function TorrentLinkPage() {

    const [torrentLink, setTorrentLink] = useState('');
    const [openingTorrentLink, setOpeningTorrentLink] = useState(false);
    const [torrentFiles, setTorrentFiles] = useState([]);
    const [selectedFile, setSelectedFile] = useState('');

    const [showMoviePlayer, setShowMoviePlayer] = useState(false);

    const [movieSources, setMovieSources] = useState([]);


    useEffect(()=>{
        setMovieSources([{
            quality: 'Movie',
            downloadUrl: torrentLink,
            fileName: selectedFile
        }]);
    }, [torrentLink, selectedFile]);

    const changeTorrentLink = (torrentLink) => {
        setOpeningTorrentLink(false);
        setSelectedFile('');
        setTorrentLink(torrentLink);
        setTorrentFiles([]);
    };

    const openTorrent = () => {
        setTorrentFiles([]);
        setOpeningTorrentLink(true);
        MoviesAPI.getTorrentFiles(torrentLink, 
            (response) => {
                setOpeningTorrentLink(false);
                setTorrentFiles(response);
            },
            () => {
                setOpeningTorrentLink(false);
                setTorrentFiles([]);
            });
    };

    const onPlayFileClick = (file) => {
        setSelectedFile(file);
        setShowMoviePlayer(true);
    }


    return (
        <div className="torrent-link-page-container">
            <div className="title">Enter a torrent url or magnet and click on Open to see the list of files</div>
           <TextInput placeHolder="Torrent link or magnet..." large onTextChanged={(text) => changeTorrentLink(text)} />

            <div style={{margin: "10px 0"}}>
                <OpenButton visible={!openingTorrentLink} onClick={() => openTorrent()}/>
                { Boolean(openingTorrentLink) ? <CircularProgressBar visible size="30px" color="white"/> : null }
                
            </div>
           <FileList torrentLink={torrentLink} files={torrentFiles} onPlayFileClick={(file)=> onPlayFileClick(file)}/>
          
           <VideoPlayerWindow visible={showMoviePlayer} sources={movieSources} onCloseClick={() => setShowMoviePlayer(false)} />
        </div>
    )
}

export default TorrentLinkPage;

function FileList({torrentLink, files, onPlayFileClick}){

    const [selectedFileIndex, setSelectedFileIndex] = useState(null);

    useEffect(()=>{
        setSelectedFileIndex(null);
    },[files]);

    
    if(!files || files.length === 0)
        return null;

    const fileListStyle = {
        background: 'linear-gradient(342deg, rgba(42,42,42,1) 0%, rgba(0,0,0,1) 65%)',
        borderRadius: '15px',
        padding:'5px 0',
        minWidth: '50%'
    };

    const fileStyle = {
        padding: '10px 7px',
        fontSize: '18px',
        fontWeight: '500',
        borderRadius: '7px',
        cursor: 'pointer'
    };

    const selectedFileStyle = {
        ...fileStyle,
        background: '#5c2525'
    };

    const playButtons = (file, index) =>{
        if(index === selectedFileIndex){
            return ( 
                <div className="play-buttons-container">
                    <PlayButton onClick={()=> onPlayFileClick(file)}/>
                    <PlayWithVLCButton videoUrl={MoviesAPI.apiStreamUrl(torrentLink, file)}/>
                </div>)
        }
        else 
            return null;   
    }

    return(
        <div style={fileListStyle}>
            {files.map((file, index) => 
                <div>
                    <div 
                        key={index} 
                        style={index === selectedFileIndex ? selectedFileStyle : fileStyle}
                        onMouseEnter={(e)=> {if(index !== selectedFileIndex) e.target.style.background='#362b2b'}}
                        onMouseLeave={(e)=> {if(index !== selectedFileIndex) e.target.style.background=''}}
                        onClick={()=> setSelectedFileIndex(index)}>
                        {file}
                        {playButtons(file, index)}
                    </div>
                   
                </div>
                )}
        </div>
    )
}