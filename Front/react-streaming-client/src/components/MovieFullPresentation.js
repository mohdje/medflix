
import "../style/movie-full-presentation.css";
import "../style/button.css";

import ArrowBackIcon from '@material-ui/icons/ArrowBack';
import PlayArrowIcon from '@material-ui/icons/PlayArrow';

import VideoPlayerWindow from './VideoPlayerWindow';
import CircularProgressBar from "./CircularProgressBar";

import MoviesAPI from "../js/moviesAPI.js";
import fadeTransition from "../js/customStyles.js";

import { useEffect, useState } from 'react';


function MovieFullPresentation({ movieId, onCloseClick }) {

    const [movieDetails, setMovieDetails] = useState({});
    const [dataLoaded, setDataLoaded] = useState(false);
    const [showMoviePlayer, setShowMoviePlayer] = useState(false);

    useEffect(() => {
        setDataLoaded(false);
        if (movieId) {
            MoviesAPI.getMovieDetails(movieId,
                (details) => {
                    if (details) {
                        setMovieDetails(details);
                        setDataLoaded(true);
                    }
                })
        }
    }, [movieId]);

    const getVideoQualities = (torrents) => {
        if(!torrents)
            return;
       var qualities = torrents.map(t=> t.quality); //.join(", ")
       var withoutDoublons = qualities.filter((q, index) => qualities.indexOf(q) === index);
       return withoutDoublons.join(", ");
    }

    return (
        <div style={{height: '100%'}}>
            <CircularProgressBar color={'white'} size={'80px'} position={"center"} visible={!dataLoaded} />
            <div style={fadeTransition(dataLoaded)} className="movie-full-presentation-container">
                <VideoPlayerWindow visible={showMoviePlayer} movie={movieDetails} onCloseClick={() => setShowMoviePlayer(false)} />
                <div className="movie-full-presentation-close-btn" onClick={() => onCloseClick()}>
                    <ArrowBackIcon className="close-cross" />
                </div>
                <div className="movie-full-presentation-header" style={{ backgroundImage: 'url(' + movieDetails.backgroundImageUrl + ')' }}>
                    <div className="cover-and-rating">
                        <img className="cover" src={movieDetails.coverImageUrl} />
                        <div className="rating">{movieDetails.rating} </div>
                    </div>
                    <div className="title-and-trailer">
                        <div className="title">{movieDetails.title}</div>
                        <div className="year">{movieDetails.year}</div>
                        <iframe className="trailer" src={movieDetails.youtubeTrailerUrl}></iframe>
                    </div>
                </div>
                <div className="movie-full-presentation-play">
                    <div className="qualities">
                        <span className="title">Qualities </span>{getVideoQualities(movieDetails.torrents)}
                    </div>
                    <div className="standard-button red" onClick={() => setShowMoviePlayer(true)}>
                        <PlayArrowIcon/>
                        <div style={{marginLeft: '5px'}}>Play</div>
                    </div>
                </div>
                <div className="movie-full-presentation-body">
                    <div className="movie-full-presentation-info">
                        <div className="title">Genre</div>
                        <div className="content">{movieDetails.genres}</div>
                    </div>
                    <div className="movie-full-presentation-info">
                        <div className="title">Synopsis</div>
                        <div className="content">{movieDetails.synopsis}</div>
                    </div>
                    <div className="movie-full-presentation-info">
                        <div className="title">Director</div>
                        <div className="content">{movieDetails.director}</div>
                    </div>
                    <div className="movie-full-presentation-info">
                        <div className="title">Cast</div>
                        <div className="content">{movieDetails.cast}</div>
                    </div>
                </div>
            </div>
        </div>

    );
}
export default MovieFullPresentation;

//override details for tests
// details.imdbCode = "tt4154796";
// details.torrents = [
//     {
//         quality: "720p",
//         downloadUrl: 'https://cdn.fluidplayer.com/videos/valerian-1080p.mkv'
//     },
//     {
//         quality: "1080p",
//         downloadUrl: 'https://www.learningcontainer.com/wp-content/uploads/2020/05/sample-mp4-file.mp4'
//     }
// ]