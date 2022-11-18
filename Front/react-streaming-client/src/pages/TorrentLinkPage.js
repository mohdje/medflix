
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


    useEffect(()=>{
        setMovieSources([{
            quality: 'Movie',
            downloadUrl: torrentLink
        }]);
    }, [torrentLink]);


    return (
        <div className="torrent-link-page-container">
            <div className="title">Enter a torrent url or magnet and press Play to watch</div>
           <TextInput placeHolder="Torrent link or magnet..." large onTextChanged={(text) => setTorrentLink(text)} />

           <div className="play-buttons-container">
                <PlayButton onClick={()=> setShowMoviePlayer(true)}/>
                <PlayWithVLCButton videoUrl={MoviesAPI.apiStreamUrl(torrentLink)}/>
           </div>
           <VideoPlayerWindow visible={showMoviePlayer} sources={movieSources} onCloseClick={() => setShowMoviePlayer(false)} />
        </div>
    )
}

export default TorrentLinkPage;