
import "../style/movie-full-presentation.css";

import ArrowBackIcon from '@material-ui/icons/ArrowBack';

import VideoPlayerWindow from '../components/video/VideoPlayerWindow';
import CircularProgressBar from "../components/common/CircularProgressBar";
import PlayButton from "../components/common/PlayButton";
import PlayWithVLCButton from "../components/common/PlayWithVLCButton";
import BookmarkButton from "../components/common/BookmarkButton";
import ModalPlayWithVLCInstructions from "../components/modal/ModalPlayWithVLCInstructions";

import MoviesAPI from "../js/moviesAPI.js";
import fadeTransition from "../js/customStyles.js";

import { useEffect, useState } from 'react';


function MovieFullPresentation({ movieId, onCloseClick }) {

    const [movieDetails, setMovieDetails] = useState({});

    const [selectedVersion, setSelectedVersion] = useState("VO");
    const [vfState, setVfState] = useState("not available");
    const [availableMovieSources, setAvailableMovieSources] = useState([]);
    const [VOMovieSources, setVOMovieSources] = useState([]);
    const [VFMovieSources, setVFMovieSources] = useState([]);


    const [selectedMovieSources, setSelectedMovieSources] = useState([]);
    const [movieSubtitles, setMovieSubtitles] = useState([]);

    const [dataLoaded, setDataLoaded] = useState(false);
    const [showMoviePlayer, setShowMoviePlayer] = useState(false);
    const [showVLCPlayerInstructions, setShowVLCPlayerInstructions] = useState(false);
    const [addBookmarkButtonVisible, setAddBookmarkButtonVisible] = useState(true);


    useEffect(() => {
        setDataLoaded(false);
        if (movieId) {
            MoviesAPI.getMovieDetails(movieId,
                (details) => {
                    if (details) {
                        setMovieDetails(details);
                        setVOMovieSources(details.torrents);
                        setDataLoaded(true);
                    }
                });
            MoviesAPI.getActiveVOMovieService((service) => {           
                MoviesAPI.isMovieBookmarked(movieId, service.id, (isMovieBookmarked) => {
                    isMovieBookmarked = isMovieBookmarked === 'true';
                    setAddBookmarkButtonVisible(!isMovieBookmarked);
                });
            });
        }
    }, [movieId]);

    useEffect(() => {

        if (movieDetails.title) {
            setVfState('loading');
            MoviesAPI.searchVFSources(movieDetails.title, movieDetails.year,
                (sources) => {
                    if (sources && sources.length > 0) {
                        setVFMovieSources(sources);
                        setVfState('available');
                    }
                    else
                        setVfState('not available');
                }, () => {
                    setVfState('not available');
                }
            );
        }

        if (movieDetails.imdbCode) {
            MoviesAPI.getAvailableSubtitles(movieDetails.imdbCode,
                (availableSubtitles) => {
                    setMovieSubtitles(availableSubtitles);
                })
        }
    }, [movieDetails]);

    useEffect(() => {
        if(selectedVersion === "VO" && VOMovieSources && VOMovieSources.length > 0)
            setSelectedMovieSources(VOMovieSources);
        else if(selectedVersion === "VF" && VFMovieSources && VFMovieSources.length > 0)
            setSelectedMovieSources(VFMovieSources);
    }, [selectedVersion, VOMovieSources, VFMovieSources]);

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

    const versionSelector = () => {
        return (
            <VersionSelector vfState={vfState} onVersionSelected={(version) => setSelectedVersion(version)} />
        );
    }

    return (
        <div style={{ height: '100%' }}>
            <CircularProgressBar color={'white'} size={'80px'} position={"center"} visible={!dataLoaded} />
            <ModalPlayWithVLCInstructions visible={showVLCPlayerInstructions} sources={selectedMovieSources} onCloseClick={() => setShowVLCPlayerInstructions(false)} />
            <div style={fadeTransition(dataLoaded)} className="movie-full-presentation-container">
                <VideoPlayerWindow visible={showMoviePlayer} sources={selectedMovieSources} subtitles={movieSubtitles} onCloseClick={() => setShowMoviePlayer(false)} />
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
                    <VideoInfo infoTitle={"Versions"} infoContent={versionSelector()} />
                    <VideoInfo infoTitle={"Qualities"} infoContent={getVideoQualities(movieDetails.torrents)} />
                    <VideoInfo infoTitle={"Duration"} infoContent={movieDetails.duration} />
                    <PlayButton onClick={() => setShowMoviePlayer(true)} />
                    <PlayWithVLCButton onClick={() => setShowVLCPlayerInstructions(true)} />
                    <BookmarkButton onClick={() => bookmarkMovie()} visible={addBookmarkButtonVisible} />
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

function VersionSelector({ vfState, onVersionSelected }) {

    const [selectedVersion, setSelectedVersion] = useState("VO");

    const toggleSelection = (selectedVersion) => {
        setSelectedVersion(selectedVersion);
        onVersionSelected(selectedVersion);
    }

    if (vfState === 'available') {
        return (
            <div className="movie-version-selector">
                <div className={"standard-button " + (selectedVersion === "VO" ? 'red' : 'grey')} onClick={() => toggleSelection("VO")}>VO</div>
                <div className={"standard-button " + (selectedVersion === "VF" ? 'red' : 'grey')} onClick={() => toggleSelection("VF")}>VF</div>
            </div>
        );
    }
    else if (vfState === 'loading') {

        return (
            <div className="movie-version-selector">
                <div className="version">VO</div>
                <div className="version loading">
                    <div>VF</div>
                    <CircularProgressBar color={'white'} size={'15px'} visible={true} />
                </div>
            </div>
        );

    }
    else {
        return (
            <div className="movie-version-selector">
                <div className="version">VO</div>
            </div>
        )
    }

}
