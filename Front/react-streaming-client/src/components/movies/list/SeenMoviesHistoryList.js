import "../../../style/seen-movies-history-list.css";

import SeenMoviePresentation from "../presentation/SeenMoviePresentation";
import VideoPlayerWindow from '../../video/VideoPlayerWindow';
import CircularProgressBar from "../../common/CircularProgressBar";
import ModalLoadingMessage from "../../modal/ModalLoadingMessage";
import MoviesAPI from "../../../js/moviesAPI.js";
import { useEffect, useState } from 'react';

function SeenMoviesHistoryList({ onMoreClick }) {
    const [selectedMovie, setSelectedMovie] = useState(null);
    const [seenMovies, setSeenMovies] = useState([]);
    const [moviesServiceLoadingVisible, setMoviesServiceLoadingVisible] = useState(false);

    useEffect(() => {
        MoviesAPI.getLastSeenMovies((lastSeenMovies) => {
            setSeenMovies(lastSeenMovies);
        });
    }, []);

    const selectMoviesService = (seenMovie, callback) => {
        setMoviesServiceLoadingVisible(true);
        setTimeout(()=>{
            MoviesAPI.changeMovieService(seenMovie.serviceName, () => {
            
                setMoviesServiceLoadingVisible(false);
                callback();
            })
        }, 2000);
       
    }

    return (
        <div className="seen-movies-history-list-container">
            <CircularProgressBar color={'white'} size={'80px'} position={"center"} visible={!(seenMovies && seenMovies.length > 0)} />
            <ModalLoadingMessage visible={moviesServiceLoadingVisible} loadingMessage={'Movies service selection'} />
            <VideoPlayerWindow visible={selectedMovie} movie={selectedMovie} onCloseClick={() => setSelectedMovie(null)} />
            <div className="seen-movies-history-list-title-page">Last movies you played</div>
            <div className="seen-movies-history-list-content">
                {seenMovies.map((seenMovie, index) =>
                (<SeenMoviePresentation
                    key={index}
                    seenMovie={seenMovie}
                    onMoreClick={() => selectMoviesService(seenMovie, () => onMoreClick(seenMovie.movie.id))}
                    onPlayClick={() => selectMoviesService(seenMovie, () => setSelectedMovie(seenMovie.movie))} />))}
            </div>
        </div>
    )
}

export default SeenMoviesHistoryList;