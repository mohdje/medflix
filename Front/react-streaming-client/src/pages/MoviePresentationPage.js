
import "../style/movie-presentation-page.css";

import ArrowBackIcon from '@material-ui/icons/ArrowBack';

import ModalMovieTrailer from '../components/modal/ModalMovieTrailer';

import VideoPlayerWindow from '../components/video/VideoPlayerWindow';
import CircularProgressBar from "../components/common/CircularProgressBar";
import PlayButton from "../components/common/Buttons/PlayButton";
import TrailerButton from "../components/common/Buttons/TrailerButton";
import PlayWithVLCButton from "../components/common/Buttons/PlayWithVLCButton";
import BookmarkButton from "../components/common/Buttons/BookmarkButton";
import ModalPlayWithVLCInstructions from "../components/modal/ModalPlayWithVLCInstructions";

import MoviesAPI from "../js/moviesAPI.js";
import fadeTransition from "../js/customStyles.js";

import { useEffect, useState } from 'react';


function MovieFullPresentation({ movieId, onCloseClick }) {

    const [dataLoaded, setDataLoaded] = useState(false);
    const [movieDetails, setMovieDetails] = useState({});

    const [selectedVersion, setSelectedVersion] = useState("VO");
    const [vfState, setVfState] = useState("not available");
    const [VOMovieSources, setVOMovieSources] = useState([]);
    const [VFMovieSources, setVFMovieSources] = useState([]);

    const [selectedMovieSources, setSelectedMovieSources] = useState([]);
    const [movieSubtitles, setMovieSubtitles] = useState([]);

    const [showMoviePlayer, setShowMoviePlayer] = useState(false);
    const [showMovieTrailer, setShowMovieTrailer] = useState(false);
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

    const getMovieInfo = () => {
        let info = movieDetails?.year;
        if(movieDetails?.rating)
            info += " - " + movieDetails.rating.replace(',','.');

        if(movieDetails?.duration)
            info +=" - " + movieDetails.duration.replace(/\s/g, "");

        return info;
    }

    return (
        <div style={{ height: '100%' }}>
            <CircularProgressBar color={'white'} size={'80px'} position={"center"} visible={!dataLoaded} />
            <ModalPlayWithVLCInstructions visible={showVLCPlayerInstructions} sources={selectedMovieSources} onCloseClick={() => setShowVLCPlayerInstructions(false)} />
            <ModalMovieTrailer visible={showMovieTrailer} youtubeTrailerUrl={movieDetails.youtubeTrailerUrl} onCloseClick={() => setShowMovieTrailer(false)}/>
            <VideoPlayerWindow visible={showMoviePlayer} sources={selectedMovieSources} subtitles={movieSubtitles} onCloseClick={() => setShowMoviePlayer(false)} />
 
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
                    <MovieInfo infoTitle={"Qualities"} infoContent={getVideoQualities(movieDetails.torrents)} />
                    <MovieInfo infoTitle={"Versions"} infoContent={versionSelector()} />
                    <div className="actions">
                        <TrailerButton visible={movieDetails?.youtubeTrailerUrl} onClick={() => setShowMovieTrailer(true)} />
                        <PlayButton onClick={() => setShowMoviePlayer(true)} />
                        <PlayWithVLCButton onClick={() => setShowVLCPlayerInstructions(true)} />
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
