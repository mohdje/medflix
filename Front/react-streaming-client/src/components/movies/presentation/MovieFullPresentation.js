
import "../../../style/movie-full-presentation.css";

import ArrowBackIcon from '@material-ui/icons/ArrowBack';

import VideoPlayerWindow from '../../video/VideoPlayerWindow';
import CircularProgressBar from "../../common/CircularProgressBar";
import PlayButton from "../../common/PlayButton";


import MoviesAPI from "../../../js/moviesAPI.js";
import fadeTransition from "../../../js/customStyles.js";

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

    useEffect(() => {
        if (showMoviePlayer && movieDetails) MoviesAPI.saveLastSeenMovie(movieDetails);
    }, [showMoviePlayer]);

    const getVideoQualities = (torrents) => {
        if (!torrents)
            return;
        var qualities = torrents.map(t => t.quality);
        var withoutDoublons = qualities.filter((q, index) => qualities.indexOf(q) === index);
        return withoutDoublons.join(", ");
    }

    return (
        <div style={{ height: '100%' }}>
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
                <div className="movie-full-presentation-video-info">
                    <VideoInfo infoTitle={"Qualities"} infoContent={getVideoQualities(movieDetails.torrents)} />
                    <VideoInfo infoTitle={"Duration"} infoContent={movieDetails.duration} />
                    <PlayButton onClick={() => setShowMoviePlayer(true)} />
                </div>
                <div className="movie-full-presentation-body">
                    <MovieInfo infoTitle={"Genre"} infoContent={movieDetails.genres} />
                    <MovieInfo infoTitle={"Synopsis"} infoContent={movieDetails.synopsis} />
                    <MovieInfo infoTitle={"Director"} infoContent={movieDetails.director} />
                    <MovieInfo infoTitle={"Cast"} infoContent={movieDetails.cast} />
                </div>
            </div>
        </div>
    );
}
export default MovieFullPresentation;

function VideoInfo({ infoTitle, infoContent }) {
    return (
        <div className="video-info">
            <span className="title">{infoTitle} </span>{infoContent}
        </div>
    )
}

function MovieInfo({ infoTitle, infoContent }) {
    return (
        <div className="movie-full-presentation-info">
            <div className="title">{infoTitle}</div>
            <div className="content">{infoContent}</div>
        </div>
    )
}
