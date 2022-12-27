
import "../style/movie-presentation-page.css";

import ArrowBackIcon from '@material-ui/icons/ArrowBack';

import ModalMovieTrailer from '../components/modal/ModalMovieTrailer';

import { VideoPlayerWindow, ToTimeFormat } from '../components/video/VideoPlayerWindow';
import CircularProgressBar from "../components/common/CircularProgressBar";
import ProgressionBar from "../components/common/ProgressionBar";

import BaseButton from "../components/common/buttons/BaseButton";
import TrailerButton from "../components/common/buttons/TrailerButton";
import PlayWithVLCButton from "../components/common/buttons/PlayWithVLCButton";
import {AddBookmarkButton, RemoveBookmarkButton} from "../components/common/buttons/BookmarkButton";
import PlayButton from "../components/common/buttons/PlayButton";

import SecondaryInfo from "../components/common/text/SecondaryInfo";
import Paragraph from "../components/common/text/Paragraph";
import Title from "../components/common/text/Title";
import TitleAndContent from "../components/common/TitleAndContent";
import Rating from "../components/common/Rating";
import DropDown from "../components/common/DropDown";
import MoviesListLiteWithTitle from "../components/movies/list/MoviesListLiteWithTitle";

import MoviesAPI from "../js/moviesAPI.js";
import fadeTransition from "../js/customStyles.js";

import { useEffect, useState, useRef } from 'react';


function MovieFullPresentation({ movieId, onCloseClick }) {

    const [dataLoaded, setDataLoaded] = useState(false);
    const [movieDetails, setMovieDetails] = useState({});
    const [movieProgression, setMovieProgression] = useState({});

    const versionsSources = useRef([]);
    const [voSourcesSearching, setVoSourcesSearching] = useState(false);
    const [vfSourcesSearching, setVfSourcesSearching] = useState(false);

    const [selectedVersionSources, setSelectedVersionSources] = useState([]);
    const [selectedVersionSourceLink, setSelectedVersionSourceLink] = useState('');

    const [movieSubtitlesSources, setMovieSubtitlesSources] = useState([]);
    const [loadingSubtitles, setLoadingSubtitles] = useState(false);

    const [showMoviePlayer, setShowMoviePlayer] = useState(false);
    const [showMovieTrailer, setShowMovieTrailer] = useState(false);
    const [addBookmarkButtonVisible, setAddBookmarkButtonVisible] = useState(true);

    const [recommandedMovies, setRecommandedMovies] = useState([]);
    const navHistory = useRef([]);


    useEffect(() => {
        if(movieId)
            loadPage(movieId);
    }, [movieId]);

    useEffect(() => {
        if (movieDetails.imdbId) getAvailableSubtitles(movieDetails.imdbId);
        if (movieDetails.id && movieDetails.title && movieDetails.year) {
            versionsSources.current = [];
            searchVfSources(movieDetails.id, movieDetails.title, movieDetails.year);
            searchVoSources(movieDetails.title, movieDetails.year);
        }

        const topPage = document.getElementsByClassName('back-btn')[0];
        topPage.scrollIntoView();
    }, [movieDetails]);


    useEffect(() => {
        if (showMoviePlayer && movieDetails) MoviesAPI.saveWacthedMovie(movieDetails);
    }, [showMoviePlayer]);

    useEffect(() => {
        if (versionsSources.current?.length > 0) {
            setSelectedVersionSources(versionsSources.current[0].sources);
        }
        else{
            setSelectedVersionSources([]);
            setSelectedVersionSourceLink('');
        }
    }, [versionsSources.current.length]);

    useEffect(()=>{
        if(!selectedVersionSourceLink && selectedVersionSources.length > 0)
            changeSelectedSource(0);
        
    },[selectedVersionSources]);

    const loadPage = (movieId) => {
        setDataLoaded(false);
        setMovieDetails({});
        if (movieId) {
            MoviesAPI.getMovieDetails(movieId,
                (details) => {
                    if (details) {
                        setMovieDetails(details);
                        setDataLoaded(true);
                        MoviesAPI.isMovieBookmarked(movieId, (isMovieBookmarked) => {
                            isMovieBookmarked = isMovieBookmarked === 'true';
                            setAddBookmarkButtonVisible(!isMovieBookmarked);
                        });
                    }
                });
            MoviesAPI.getWatchedMovie(movieId, (watchedMovie) => {
                if(watchedMovie?.progression)
                    setMovieProgression(watchedMovie.progression);
            });
            MoviesAPI.getRecommandedMovies(movieId, (recommandedMovies) => {
                if(recommandedMovies && recommandedMovies.length > 0)
                    setRecommandedMovies(recommandedMovies);
            });
        }
    }

    const changeSelectedSource = (index) => {
        const newTab = [];
        for (let i = 0; i < selectedVersionSources.length; i++) {
            newTab.push(selectedVersionSources[i]);
            newTab[i].selected = index == i;
        }
        setSelectedVersionSourceLink(selectedVersionSources.length > 0 ? MoviesAPI.apiStreamUrl(selectedVersionSources[index].downloadUrl) : '');
        setSelectedVersionSources(newTab);
    }

    const searchVfSources = (movieId, movieTitle, movieYear) => {
        setVfSourcesSearching(true);
        MoviesAPI.searchVFSources(movieId, movieTitle, movieYear,
            (sources) => {
                setVfSourcesSearching(false);
                if (sources && sources.length > 0) {
                    versionsSources.current.push({
                        versionLabel: "VF",
                        sources: sources
                    });
                }

            }, () => {
                setVfSourcesSearching(false);
            }
        );
    }

    const searchVoSources = (movieTitle, movieYear) => {
        setVoSourcesSearching(true);
        MoviesAPI.searchVOSources(movieTitle, movieYear,
            (sources) => {
                setVoSourcesSearching(false);
                if (sources && sources.length > 0) {
                    versionsSources.current.unshift({
                        versionLabel: "VO",
                        sources: sources
                    });
                }
            }, () => {
                setVoSourcesSearching(false);
            }
        );
    }

    const getAvailableSubtitles = (imdbCode) => {
        setLoadingSubtitles(true);
        MoviesAPI.getAvailableSubtitles(imdbCode,
            (availableSubtitles) => {
                setMovieSubtitlesSources(availableSubtitles);
                setTimeout(() => setLoadingSubtitles(false), 1000);
            })
    }

    const bookmarkMovie = () => {
        MoviesAPI.addBookmarkedMovie(movieDetails, () => {
            setAddBookmarkButtonVisible(false);
        });
    }

    const unbookmarkMovie = () => {
        MoviesAPI.deleteBookmarkedMovie(movieDetails, () => {
            setAddBookmarkButtonVisible(true);
        });
    }

    const onWatchedTimeUpdate = (time) => {
       const totalTime = movieDetails.duration * 60;
       movieDetails.progression = time/totalTime;
       MoviesAPI.saveWacthedMovie(movieDetails)
    }

    const onRecommandedMovieClick = (movieId) => {
        navHistory.current.push(movieDetails.id);
        loadPage(movieId);
    }

    const onBackClick = () => {      
        if(navHistory.current.length > 0){
            let lastMovieId = navHistory.current.pop();
            loadPage(lastMovieId);
        }
        else 
            onCloseClick();
    }

    

    return (
        <div style={{ height: '100%' }}>          
            <VideoPlayerWindow 
                visible={showMoviePlayer} 
                sources={selectedVersionSources} 
                subtitles={movieSubtitlesSources} 
                onCloseClick={() => setShowMoviePlayer(false)}
                onWatchedTimeUpdate={(time) => onWatchedTimeUpdate(time)} 
                goToTime={movieProgression * movieDetails.duration}/> 
            <CircularProgressBar color={'white'} size={'80px'} position={"center"} visible={!dataLoaded} />
            <ModalMovieTrailer visible={showMovieTrailer} youtubeTrailerUrl={movieDetails.youtubeTrailerUrl} onCloseClick={() => setShowMovieTrailer(false)}/>

            <div style={fadeTransition(dataLoaded)} className="movie-presentation-page-container">
                <div className="back-btn" onClick={() => onBackClick()}>
                    <ArrowBackIcon className="back-arrow" />
                </div>
                <div className="presentation" style={{ backgroundImage: 'url(' + movieDetails.backgroundImageUrl + ')' }}>
                    <div className="info-container">
                        <div className="info">
                            <div className="title">
                                {
                                    movieDetails.logoImageUrl ? <img src={movieDetails.logoImageUrl} /> : <Title text={movieDetails.title} />
                                }
                            </div>
                            <div className="info-content">
                                <Rating rating={movieDetails.rating} size="50px" />
                                <SecondaryInfo center text={movieDetails.year  + " | " + ToTimeFormat(movieDetails.duration) + " | " +  movieDetails.genre} />
                                <MovieProgression movieProgression={movieProgression} movieDuration={movieDetails.duration}/>
                                <div className="horizontal">
                                    <TrailerButton visible={movieDetails?.youtubeTrailerUrl} onClick={() => setShowMovieTrailer(true)} />
                                    <AddBookmarkButton onClick={() => bookmarkMovie()} visible={addBookmarkButtonVisible} />
                                    <RemoveBookmarkButton onClick={() => unbookmarkMovie()} visible={!addBookmarkButtonVisible} />
                                </div>
                                <div className="play-options">
                                    <AvailableSubtitles loading={loadingSubtitles} availableSubtitlesSources={movieSubtitlesSources} />
                                    <AvailableVersions
                                        loading={voSourcesSearching || vfSourcesSearching}
                                        availableVersionsSources={versionsSources.current}
                                        onVersionSelected={(versionSources) =>{setSelectedVersionSourceLink(null);setSelectedVersionSources(versionSources.sources)} } />
                                    <div style={fadeTransition(Boolean(selectedVersionSources) && selectedVersionSources.length > 0)} >
                                        <QualitySelector versionSources={selectedVersionSources} onQualityChanged={(i) => changeSelectedSource(i)} />
                                        <div className="horizontal">
                                            <PlayButton onClick={() => setShowMoviePlayer(true)} />
                                            <PlayWithVLCButton 
                                                videoUrl={selectedVersionSourceLink}
                                                onClick={() => {if(movieDetails) MoviesAPI.saveWacthedMovie(movieDetails)}} />
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>

                        <div className="extra">
                            <TitleAndContent title="Director" content={movieDetails.director} justify="left" />
                            <TitleAndContent title="Cast" content={movieDetails.cast} justify="left" />
                            <Paragraph text={movieDetails.synopsis}></Paragraph>
                            <MoviesListLiteWithTitle 
                                movies={recommandedMovies} 
                                listTitle="You may also like" 
                                visible={recommandedMovies && recommandedMovies.length > 0} 
                                onMovieClick={(movieId) =>  {onRecommandedMovieClick(movieId)}}/>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}
export default MovieFullPresentation;


function MovieProgression({movieProgression, movieDuration}){
    if(movieProgression > 0){
        return (
            <div>
                <ProgressionBar width="100%" value={movieProgression * 100}/>
                <SecondaryInfo  text={ToTimeFormat(movieDuration - (movieDuration * movieProgression)) + ' remaining'} />
            </div>
        )
    }
    else 
        return null;
}

function AvailableSubtitles({ availableSubtitlesSources, loading }) {
    const availableSubtitles = availableSubtitlesSources?.length > 0 ? availableSubtitlesSources.map(s => s.language).join(", ") : 'No subtitles available';
    const content = loading ? <CircularProgressBar color={'white'} size={'15px'} visible={true} /> : availableSubtitles;
    return (
        <TitleAndContent title="Subtitles" content={content} />
    )
}

function AvailableVersions({ availableVersionsSources, loading, onVersionSelected }) {
    const [selectedIndex, setSelectedIndex] = useState(0);
    let content;

    const onVersionClick = (index) => {
        setSelectedIndex(index);
        onVersionSelected(availableVersionsSources[index]);
    };

    if (loading)
        content = <CircularProgressBar color={'white'} size={'15px'} visible={true} />;
    else {
        if (availableVersionsSources?.length > 0) {
            content = availableVersionsSources.map((v, i) => <BaseButton
                key={i}
                color={selectedIndex == i ? "red" : "grey"}
                content={v.versionLabel}
                onClick={() => onVersionClick(i)} />);
        }
        else
            content = 'No version available';
    }

    return (
        <TitleAndContent title="Versions" content={content} />
    )
}

function QualitySelector({ versionSources, onQualityChanged }) {
    if (!versionSources || versionSources.length === 0)
        return null;

    const qualities = [];
    versionSources.forEach(source => {

        const nbSameQualities = qualities.filter(q => q.startsWith(source.quality)).length;
        qualities.push(source.quality + (nbSameQualities > 0 ? " (" + nbSameQualities + ")" : ""));
    });

    const content = <DropDown
        values={qualities}
        width="120px"
        onValueChanged={(index) => onQualityChanged(index)} />

    return <TitleAndContent title="Qualities" content={content} justify="left"/>

}


