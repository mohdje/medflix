
import "../style/media-presentation-page.css";

import ArrowBackIcon from '@material-ui/icons/ArrowBack';

import ModalMediaTrailer from '../components/modal/ModalMediaTrailer';
import ModalEpisodeSelector from '../components/modal/ModalEpisodeSelector';

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
import MediasListLiteWithTitle from "../components/media/list/MediasListLiteWithTitle";

import AppServices from "../services/AppServices";
import fadeTransition from "../services/customStyles.js";

import { useEffect, useState, useRef } from 'react';


function MediaFullPresentation({ mediaId, onCloseClick }) {

    const [dataLoaded, setDataLoaded] = useState(false);
    const [mediaDetails, setMediaDetails] = useState({});
    const [mediaProgression, setMediaProgression] = useState({});

    const versionsSources = useRef([]);
    const [voSourcesSearching, setVoSourcesSearching] = useState(false);
    const [vfSourcesSearching, setVfSourcesSearching] = useState(false);

    const [selectedVersionSources, setSelectedVersionSources] = useState([]);
    const [selectedVersionSourceLink, setSelectedVersionSourceLink] = useState('');

    const [mediaSubtitlesSources, setMediaSubtitlesSources] = useState([]);
    const [loadingSubtitles, setLoadingSubtitles] = useState(false);

    const [showMediaPlayer, setShowMediaPlayer] = useState(false);
    const [showMediaTrailer, setShowMediaTrailer] = useState(false);
    const [addBookmarkButtonVisible, setAddBookmarkButtonVisible] = useState(true);

    const [recommandedMedias, setRecommandedMedias] = useState([]);
    const navHistory = useRef([]);


    useEffect(() => {
        if(mediaId)
            loadPage(mediaId);
    }, [mediaId]);

    useEffect(() => {
        if (mediaDetails.imdbId) getAvailableSubtitles(mediaDetails.imdbId);
        if (mediaDetails.id && mediaDetails.title && mediaDetails.year) {
            versionsSources.current = [];
            searchVfSources(mediaDetails.id, mediaDetails.title, mediaDetails.year);
            searchVoSources(mediaDetails.title, mediaDetails.year);
        }

        const topPage = document.getElementsByClassName('back-btn')[0];
        topPage.scrollIntoView();
    }, [mediaDetails]);


    useEffect(() => {
        if (showMediaPlayer && mediaDetails) AppServices.watchedMediaApiService.saveWacthedMedia(mediaDetails);
    }, [showMediaPlayer]);

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

    const loadPage = (mediaId) => {
        setDataLoaded(false);
        setMediaDetails({});
        setMediaProgression({});
        if (mediaId) {
            AppServices.searchMediaService.getMediaDetails(mediaId,
                (details) => {
                    if (details) {
                        setMediaDetails(details);
                        setDataLoaded(true);
                        AppServices.bookmarkApiService.isMediaBookmarked(mediaId, (isMediaBookmarked) => {
                            isMediaBookmarked = isMediaBookmarked === 'true';
                            setAddBookmarkButtonVisible(!isMediaBookmarked);
                        });
                    }
                });
                AppServices.watchedMediaApiService.getWatchedMedia(mediaId, (watchedMedia) => {
                if(watchedMedia?.progression)
                    setMediaProgression(watchedMedia.progression);
            });
            AppServices.searchMediaService.getRecommandedMedias(mediaId, (recommandedMedias) => {
                if(recommandedMedias && recommandedMedias.length > 0)
                    setRecommandedMedias(recommandedMedias);
            });
        }
    }

    const changeSelectedSource = (index) => {
        const newTab = [];
        for (let i = 0; i < selectedVersionSources.length; i++) {
            newTab.push(selectedVersionSources[i]);
            newTab[i].selected = index == i;
        }
        setSelectedVersionSourceLink(selectedVersionSources.length > 0 ? AppServices.torrentApiService.buildStreamUrl(selectedVersionSources[index].downloadUrl) : '');
        setSelectedVersionSources(newTab);
    }

    const searchVfSources = (mediaId, mediaTitle, mediaYear) => {
        setVfSourcesSearching(true);
        AppServices.torrentApiService.searchVFSources(mediaId, mediaTitle, mediaYear,
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

    const searchVoSources = (mediaTitle, mediaYear) => {
        setVoSourcesSearching(true);
        AppServices.torrentApiService.searchVOSources(mediaTitle, mediaYear,
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
        AppServices.subtitlesApiService.getAvailableSubtitles(imdbCode,
            (availableSubtitles) => {
                setMediaSubtitlesSources(availableSubtitles);
                setTimeout(() => setLoadingSubtitles(false), 1000);
            })
    }

    const bookmarkMedia = () => {
        AppServices.bookmarkApiService.addBookmarkedMedia(mediaDetails, () => {
            setAddBookmarkButtonVisible(false);
        });
    }

    const unbookmarkMedia = () => {
        AppServices.bookmarkApiService.deleteBookmarkedMedia(mediaDetails, () => {
            setAddBookmarkButtonVisible(true);
        });
    }

    const onWatchedTimeUpdate = (time) => {
       const totalTime = mediaDetails.duration * 60;
       mediaDetails.progression = time/totalTime;
       AppServices.watchedMediaApiService.saveWacthedMedia(mediaDetails)
    }

    const onRecommandedMediaClick = (mediaId) => {
        navHistory.current.push(mediaDetails.id);
        loadPage(mediaId);
    }

    const onBackClick = () => {      
        if(navHistory.current.length > 0){
            let lastMediaId = navHistory.current.pop();
            loadPage(lastMediaId);
        }
        else 
            onCloseClick();
    }

    

    return (
        <div style={{ height: '100%' }}>          
            <VideoPlayerWindow 
                visible={showMediaPlayer} 
                sources={selectedVersionSources} 
                subtitles={mediaSubtitlesSources} 
                onCloseClick={() => setShowMediaPlayer(false)}
                onWatchedTimeUpdate={(time) => onWatchedTimeUpdate(time)} 
                goToTime={mediaProgression * mediaDetails.duration}/> 
            <CircularProgressBar color={'white'} size={'80px'} position={"center"} visible={!dataLoaded} />
            <ModalMediaTrailer visible={showMediaTrailer} youtubeTrailerUrl={mediaDetails.youtubeTrailerUrl} onCloseClick={() => setShowMediaTrailer(false)}/>

            <div style={fadeTransition(dataLoaded)} className="media-presentation-page-container">
                <div className="back-btn" onClick={() => onBackClick()}>
                    <ArrowBackIcon className="back-arrow" />
                </div>
                <div className="presentation" style={{ backgroundImage: 'url(' + mediaDetails.backgroundImageUrl + ')' }}>
                    <div className="info-container">
                        <div className="info">
                            <div className="title">
                                {
                                    mediaDetails.logoImageUrl ? <img src={mediaDetails.logoImageUrl} /> : <Title text={mediaDetails.title} />
                                }
                            </div>
                            <div className="info-content">
                                <Rating rating={mediaDetails.rating} size="50px" />
                                <SecondaryInfo center text={mediaDetails.year  + " | " + (mediaDetails.duration ? ToTimeFormat(mediaDetails.duration) + " | " : '') +  mediaDetails.genre} />
                                <MediaProgression mediaProgression={mediaProgression} mediaDuration={mediaDetails.duration}/>
                                <div className="horizontal">
                                    <TrailerButton visible={mediaDetails?.youtubeTrailerUrl} onClick={() => setShowMediaTrailer(true)} />
                                    <AddBookmarkButton onClick={() => bookmarkMedia()} visible={addBookmarkButtonVisible} />
                                    <RemoveBookmarkButton onClick={() => unbookmarkMedia()} visible={!addBookmarkButtonVisible} />
                                </div>
                                <div className="play-options">
                                    <EpisodeSelector serieId={mediaDetails.id} seasonsCount={mediaDetails.seasonsCount}/>
                                    <AvailableSubtitles loading={loadingSubtitles} availableSubtitlesSources={mediaSubtitlesSources} />
                                    <AvailableVersions
                                        loading={voSourcesSearching || vfSourcesSearching}
                                        availableVersionsSources={versionsSources.current}
                                        onVersionSelected={(versionSources) =>{setSelectedVersionSourceLink(null);setSelectedVersionSources(versionSources.sources)} } />
                                    <div style={fadeTransition(!(voSourcesSearching || vfSourcesSearching) && Boolean(selectedVersionSources) && selectedVersionSources.length > 0)} >
                                        <QualitySelector versionSources={selectedVersionSources} onQualityChanged={(i) => changeSelectedSource(i)} />
                                        <div className="horizontal">
                                            <PlayButton onClick={() => setShowMediaPlayer(true)} />
                                            <PlayWithVLCButton 
                                                videoUrl={selectedVersionSourceLink}
                                                onClick={() => {if(mediaDetails) AppServices.watchedMediaApiService.saveWacthedMedia(mediaDetails)}} />
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>

                        <div className="extra">
                            <TitleAndContent title="Director" content={mediaDetails.director} justify="left" />
                            <TitleAndContent title="Cast" content={mediaDetails.cast} justify="left" />
                            <Paragraph text={mediaDetails.synopsis}></Paragraph>
                            <MediasListLiteWithTitle 
                                medias={recommandedMedias} 
                                listTitle="You may also like" 
                                visible={recommandedMedias && recommandedMedias.length > 0} 
                                onMediaClick={(mediaId) =>  {onRecommandedMediaClick(mediaId)}}/>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}
export default MediaFullPresentation;


function MediaProgression({mediaProgression, mediaDuration}){
    if(mediaProgression > 0){
        return (
            <div>
                <ProgressionBar width="100%" value={mediaProgression * 100}/>
                <SecondaryInfo  text={ToTimeFormat(mediaDuration - (mediaDuration * mediaProgression)) + ' remaining'} />
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

function EpisodeSelector({serieId, seasonsCount}){
    const [modalVisible, setModalVisible] = useState(false);
    const [selectedSeasonNumber, setSelectedSeasonNumber] = useState(1)
    const [selectedEpisodeNumber, setSelectedEpisodeNumber] = useState(1)

    useEffect(() => {
        setSelectedSeasonNumber(1);
        setSelectedEpisodeNumber(1);
    },[seasonsCount]);

    if(!seasonsCount)
        return null;

    const onButtonClick = () => {
        setModalVisible(true);
    }

    const containerStyle = {
        margin: '12px auto',
    }

    const onEpisodeSelected = (seasonNumber, episodeNumber) => {
        setSelectedSeasonNumber(seasonNumber);
        setSelectedEpisodeNumber(episodeNumber);
        setModalVisible(false);
    }

    return <div style={containerStyle}>
            <ModalEpisodeSelector 
                visible={modalVisible} 
                serieId={serieId} 
                numberOfSeasons={seasonsCount} 
                onEpisodeSelected={(seasonNumber, episodeNumber)=> onEpisodeSelected(seasonNumber, episodeNumber)}
                onCloseClick={()=> setModalVisible(false)}/>
            <BaseButton 
                color="red" 
                content={"Season " + selectedSeasonNumber + " - Episode " + selectedEpisodeNumber}
                onClick={()=> onButtonClick()}/>
        </div>
}


