
import "../style/torrent-link-page.css";

import TextInput from '../components/common/TextInput';
import PlayButton from '../components/common/buttons/PlayButton';
import PlayWithVLCButton from '../components/common/buttons/PlayWithVLCButton';
import VideoPlayerWindow from '../components/video/VideoPlayerWindow';
import MoviesAPI from "../js/moviesAPI";

import { useEffect, useState } from 'react';

function TorrentLinkPage() {

    const [torrentLink, setTorrentLink] = useState('');
    const [showMoviePlayer, setShowMoviePlayer] = useState(false);

    const [movieSources, setMovieSources] = useState([]);
    const [showVlcNotInstalledMsg, setShowVlcNotInstalledMsg]= useState(false);

    useEffect(()=>{
        setMovieSources([{
            quality: 'Movie',
            downloadUrl: torrentLink
        }]);
    }, [torrentLink]);

    const playWithVLC = ()=>{
        
        MoviesAPI.playWithVlc( 
            MoviesAPI.apiStreamUrl(torrentLink), 
            null, 
            ()=>{
                setShowVlcNotInstalledMsg(true);
            }
        );
    }
    return (
        <div className="torrent-link-page-container">
            <div className="title">Enter a torrent url or magnet and press Play to watch</div>
           <TextInput placeHolder="Torrent link or magnet..." large onTextChanged={(text) => setTorrentLink(text)} />

           <div className="play-buttons-container">
                <PlayButton onClick={()=> setShowMoviePlayer(true)}/>
                <PlayWithVLCButton onClick={()=> playWithVLC()}/>
           </div>
           <div className="message" style={{display:  showVlcNotInstalledMsg ? '' : 'none'}}>It seems you don't have VLC installed on your computer</div>
           <VideoPlayerWindow visible={showMoviePlayer} sources={movieSources} onCloseClick={() => setShowMoviePlayer(false)} />
        </div>
    )
}

export default TorrentLinkPage;