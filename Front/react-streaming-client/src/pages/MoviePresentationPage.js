
import "../style/movie-presentation-page.css";

import ArrowBackIcon from '@material-ui/icons/ArrowBack';

import ModalMovieTrailer from '../components/modal/ModalMovieTrailer';

import VideoPlayerWindow from '../components/video/VideoPlayerWindow';
import CircularProgressBar from "../components/common/CircularProgressBar";

import TrailerButton from "../components/common/buttons/TrailerButton";
import PlayWithVLCButton from "../components/common/buttons/PlayWithVLCButton";
import BookmarkButton from "../components/common/buttons/BookmarkButton";
import PlayButton from "../components/common/buttons/PlayButton";
import ModalPlayWithVLCInstructions from "../components/modal/ModalPlayWithVLCInstructions";
import ModalChangeSources from "../components/modal/ModalChangeSources";


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
    const [loadingSubtitles, setLoadingSubtitles] = useState(false);

    const [showMoviePlayer, setShowMoviePlayer] = useState(false);
    const [showMovieTrailer, setShowMovieTrailer] = useState(false);
    const [showVLCPlayerInstructions, setShowVLCPlayerInstructions] = useState(false);
    const [addBookmarkButtonVisible, setAddBookmarkButtonVisible] = useState(true);
    const [showChangeSourcesModal, setShowChangeSourcesModal] = useState({visible: false, setting: ''});


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
        if (movieDetails.title) getVfSources()
        if (movieDetails.imdbCode) getAvailableSubtitles();
        
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

    const getVfSources = () => {
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

    const getAvailableSubtitles = () => {
        setLoadingSubtitles(true);
        MoviesAPI.getAvailableSubtitles(movieDetails.imdbCode,
            (availableSubtitles) => {
                setMovieSubtitles(availableSubtitles);
                setTimeout(()=> setLoadingSubtitles(false), 1000);
        })   
    }

    const bookmarkMovie = () => {
        MoviesAPI.addBookmarkedMovie(movieDetails, () => {
            setAddBookmarkButtonVisible(false);
        });
    }
    
    const getMovieInfo = () => {
        let info = movieDetails?.year;
        if(movieDetails?.rating)
            info += " - " + movieDetails.rating.replace(',','.');

        if(movieDetails?.duration)
            info +=" - " + movieDetails.duration.replace(/\s/g, "");

        return info;
    }

    const getSubtitlesLanguages = () => {     
        return movieSubtitles?.length > 0 ? movieSubtitles.map(s => s.language).join(", ") : 'No subtitles';
    }

    const changeSource = () => {
        if(showChangeSourcesModal.setting === 'subs') getAvailableSubtitles();
        else if(showChangeSourcesModal.setting === 'vf') getVfSources();
    }

    return (
        <div style={{ height: '100%' }}>
            <CircularProgressBar color={'white'} size={'80px'} position={"center"} visible={!dataLoaded} />
            <ModalPlayWithVLCInstructions visible={showVLCPlayerInstructions} sources={selectedMovieSources} onCloseClick={() => setShowVLCPlayerInstructions(false)} />
            <ModalChangeSources visible={showChangeSourcesModal.visible} setting={showChangeSourcesModal.setting} onCloseClick={(changes)=> { if(changes) changeSource(); setShowChangeSourcesModal({visible: false})}}/>
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
                    <MovieInfo 
                        infoTitle={"Subtitles"} 
                        infoContent={<SubtitlesSelector 
                                        text={getSubtitlesLanguages()} 
                                        loading={loadingSubtitles} 
                                        onChangeSourceClick={()=>setShowChangeSourcesModal({visible: true, setting: 'subs'})}/>} 
                        />
                    <MovieInfo 
                        infoTitle={"Versions"} 
                        infoContent={<VersionSelector  
                                        vfState={vfState} 
                                        onVersionSelected={(version) => setSelectedVersion(version)} 
                                        onChangeSourceClick={()=> setShowChangeSourcesModal({visible: true, setting: 'vf'})} />}
                    />
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

function SubtitlesSelector({text, loading, onChangeSourceClick}){
    if(loading)    
        return <CircularProgressBar color={'white'} size={'15px'} visible={true} />;
    else
        return (
            <div style={{display: 'flex'}}>
                <div>{text}</div>
                <div className={"text-button"} onClick={()=> onChangeSourceClick()}>Change source</div>
            </div>
        )
}

function VersionSelector({ vfState, onVersionSelected, onChangeSourceClick }) {

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
                <div className={"text-button"} onClick={()=> onChangeSourceClick()}>Change source</div>
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
                <div className={"text-button"} onClick={()=> onChangeSourceClick()}>Change source</div>
            </div>
        )
    }

}
