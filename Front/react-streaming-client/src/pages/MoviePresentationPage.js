
import "../style/movie-presentation-page.css";

import ArrowBackIcon from '@material-ui/icons/ArrowBack';

import ModalMovieTrailer from '../components/modal/ModalMovieTrailer';

import VideoPlayerWindow from '../components/video/VideoPlayerWindow';
import CircularProgressBar from "../components/common/CircularProgressBar";
import PlayButton from "../components/common/buttons/PlayButton";
import TrailerButton from "../components/common/buttons/TrailerButton";
import BookmarkButton from "../components/common/buttons/BookmarkButton";

import MoviesAPI from "../js/moviesAPI.js";
import fadeTransition from "../js/customStyles.js";

import { useEffect, useState } from 'react';


function MovieFullPresentation({ movieId, onCloseClick }) {

    const [dataLoaded, setDataLoaded] = useState(false);
    const [movieDetails, setMovieDetails] = useState({});

    const [showMoviePlayer, setShowMoviePlayer] = useState(false);
    const [showMovieTrailer, setShowMovieTrailer] = useState(false);
    const [addBookmarkButtonVisible, setAddBookmarkButtonVisible] = useState(true);

    useEffect(() => {
        setDataLoaded(false);
        if (movieId) {
            MoviesAPI.getMovieDetails(movieId,
                (details) => {
                    if (details) {
                        setMovieDetails(details);
                        setDataLoaded(true);
                    }
                });
                MoviesAPI.isMovieBookmarked(movieId, 'YtsApiMx', (isMovieBookmarked) => {
                    isMovieBookmarked = isMovieBookmarked === 'true';
                    setAddBookmarkButtonVisible(!isMovieBookmarked);
                });
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
        return withoutDoublons.length > 3 ? withoutDoublons.slice(0, 3).join(", ") + "..." : withoutDoublons.join(", ");
    }

    const bookmarkMovie = () => {
        MoviesAPI.addBookmarkedMovie(movieDetails, () => {
            setAddBookmarkButtonVisible(false);
        });
    }

    const getMovieInfo = () => {
        let info = movieDetails?.year;
        if(movieDetails?.rating)
            info += " - " + movieDetails.rating;

        if(movieDetails?.duration)
            info +=" - " + movieDetails.duration.replace(/\s/g, "");

        return info;
    }

    return (
        <div style={{ height: '100%' }}>
            <CircularProgressBar color={'white'} size={'80px'} position={"center"} visible={!dataLoaded} />
            <ModalMovieTrailer visible={showMovieTrailer} youtubeTrailerUrl={movieDetails.youtubeTrailerUrl} onCloseClick={() => setShowMovieTrailer(false)}/>
            <VideoPlayerWindow visible={showMoviePlayer} onCloseClick={() => setShowMoviePlayer(false)} />
 
            <div style={fadeTransition(dataLoaded)} className="movie-presentation-page-container">
                <div className="back-btn" onClick={() => onCloseClick()}>
                    <ArrowBackIcon className="back-arrow" />
                </div>
                <div className="illustration" style={{ backgroundImage: 'url(' + movieDetails.backgroundImageUrl + ')' }}>
                    <img className="cover" src={movieDetails.coverImageUrl} />
                </div>
                <div className="info">
                    <div className="title">{movieDetails.title}</div>
                    <div className="details">{getMovieInfo()}</div>
                    <MovieInfo infoTitle={"Director"} infoContent={movieDetails.director} />
                    <MovieInfo infoTitle={"Cast"} infoContent={movieDetails.cast} />
                    <MovieInfo infoTitle={"Genre"} infoContent={movieDetails.genres} />
                    <MovieInfo infoTitle={"Synopsis"} infoContent={movieDetails.synopsis} />
                    <MovieInfo infoTitle={"Subtitles"} infoContent={"French, English"} />
                    <MovieInfo infoTitle={"Qualities"} infoContent={getVideoQualities(movieDetails.torrents)} />                  
                    <div className="actions">
                        <TrailerButton visible={movieDetails?.youtubeTrailerUrl} onClick={() => setShowMovieTrailer(true)} />
                        <PlayButton onClick={() => setShowMoviePlayer(true)} />
                        <BookmarkButton onClick={() => bookmarkMovie()} visible={addBookmarkButtonVisible} />
                    </div>
                </div>
            </div>
        </div>
    );
}
export default MovieFullPresentation;

function MovieInfo({ infoTitle, infoContent }) {
    if(infoContent){
        return (
            <div className="extra">
                <div className="title">{infoTitle}</div>
                <div className="content">{infoContent}</div>
            </div>
        )
    }
    else 
        return null; 
}
