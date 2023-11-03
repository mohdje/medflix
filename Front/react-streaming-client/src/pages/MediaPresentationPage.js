
import "../style/media-presentation-page.css";

import ArrowBackIcon from '@material-ui/icons/ArrowBack';

import ModalMediaTrailer from '../components/modal/ModalMediaTrailer';
import ModalEpisodeSelector from '../components/modal/ModalEpisodeSelector';

import { VideoPlayerWindow } from '../components/video/VideoPlayerWindow';
import CircularProgressBar from "../components/common/CircularProgressBar";
import ProgressionBar from "../components/common/ProgressionBar";

import BaseButton from "../components/common/buttons/BaseButton";
import TrailerButton from "../components/common/buttons/TrailerButton";
import PlayWithVLCButton from "../components/common/buttons/PlayWithVLCButton";
import { AddBookmarkButton, RemoveBookmarkButton } from "../components/common/buttons/BookmarkButton";
import PlayButton from "../components/common/buttons/PlayButton";

import SecondaryInfo from "../components/common/text/SecondaryInfo";
import Paragraph from "../components/common/text/Paragraph";
import Title from "../components/common/text/Title";
import TitleAndContent from "../components/common/TitleAndContent";
import Rating from "../components/common/Rating";
import DropDown from "../components/common/DropDown";
import MediasListLiteWithTitle from "../components/media/list/MediasListLiteWithTitle";

import AppServices from "../services/AppServices";
import { ToTimeFormat } from "../helpers/timeFormatHelper";
import fadeTransition from "../helpers/customStyles.js";

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

    const [similarMedias, setSimilarMedias] = useState([]);
    const navHistory = useRef([]);

    const selectedEpisode = useRef({
        seasonNumber: 1,
        episodeNumber: 1
    })


    useEffect(() => {
        if (mediaId)
            loadPage(mediaId);
    }, [mediaId]);

    useEffect(() => {
        if (mediaDetails.imdbId) getAvailableSubtitles(mediaDetails.imdbId, selectedEpisode.current.seasonNumber, selectedEpisode.current.episodeNumber);
        if (mediaDetails.id && mediaDetails.title && mediaDetails.year) {
            clearVersionSources();
            searchVfSources(mediaDetails.id, mediaDetails.title, mediaDetails.year, selectedEpisode.current.seasonNumber, selectedEpisode.current.episodeNumber);
            searchVoSources(mediaDetails.title, mediaDetails.year, mediaDetails.imdbId, selectedEpisode.current.seasonNumber, selectedEpisode.current.episodeNumber);
        }

        const topPage = document.getElementsByClassName('back-btn')[0];
        topPage.scrollIntoView();
    }, [mediaDetails]);

    useEffect(() => {
        if (!selectedVersionSourceLink && selectedVersionSources.length > 0)
            changeSelectedSource(0);

    }, [selectedVersionSources]);

    const loadPage = (mediaId) => {
        setDataLoaded(false);
        setMediaDetails({});
        setSimilarMedias([]);
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
                    else {
                        setDataLoaded(true);
                        const topPage = document.getElementsByClassName('no-media-details-message')[0];
                        topPage.scrollIntoView();
                    }
                });
            getMediaProgression(mediaId, 1, 1);
            AppServices.searchMediaService.getSimilarMedias(mediaId, (recommandedMedias) => {
                if (recommandedMedias && recommandedMedias.length > 0)
                    setSimilarMedias(recommandedMedias);
            });
        }
    }

    const setVersionSources = (versionLabel, sources, setFirst) => {
        if (sources && sources.length > 0) {
            const newVersionSources = {
                versionLabel: versionLabel,
                sources: sources
            };
            if (setFirst) {
                setSelectedVersionSourceLink('');
                versionsSources.current.unshift(newVersionSources)
            }
            else versionsSources.current.push(newVersionSources);
            setSelectedVersionSources(versionsSources.current[0].sources);
        }
    }

    const clearVersionSources = () => {
        versionsSources.current = [];
        setSelectedVersionSources([]);
        setSelectedVersionSourceLink('');
    }

    const changeSelectedSource = (index) => {
        const newTab = [];
        for (let i = 0; i < selectedVersionSources.length; i++) {
            newTab.push(selectedVersionSources[i]);
            newTab[i].selected = index == i;
        }
        setSelectedVersionSourceLink(
            selectedVersionSources.length > 0 ? AppServices.torrentApiService.buildStreamUrl(selectedVersionSources[index].downloadUrl, '', selectedVersionSources[index].seasonNumber, selectedVersionSources[index].episodeNumber)
                : '');
        setSelectedVersionSources(newTab);
    }

    const searchVfSources = (mediaId, mediaTitle, mediaYear, seasonNumber, episodeNumber) => {
        setVfSourcesSearching(true);
        AppServices.torrentApiService.searchVFSources(mediaId, mediaTitle, mediaYear, seasonNumber, episodeNumber,
            (sources) => {
                setVfSourcesSearching(false);
                if (sources && sources.length > 0) {
                    setVersionSources("VF", sources.map(source => ({ ...source, seasonNumber: seasonNumber, episodeNumber: episodeNumber })));
                }

            }, () => {
                setVfSourcesSearching(false);
            }
        );
    }

    const searchVoSources = (title, year, imdbId, seasonNumber, episodeNumber) => {
        setVoSourcesSearching(true);
        AppServices.torrentApiService.searchVOSources(title, year, imdbId, seasonNumber, episodeNumber,
            (sources) => {
                setVoSourcesSearching(false);
                if (sources && sources.length > 0) {
                    setVersionSources("VO", sources.map(source => ({ ...source, seasonNumber: seasonNumber, episodeNumber: episodeNumber })), true);
                }
            }, () => {
                setVoSourcesSearching(false);
            }
        );
    }

    const getMediaProgression = (mediaId, seasonNumber, episodeNumber) => {
        setMediaProgression({});
        AppServices.watchedMediaApiService.getWatchedMedia(
            mediaId,
            seasonNumber,
            episodeNumber,
            (watchedMedia) => {
                if (watchedMedia?.currentTime && watchedMedia?.totalDuration) {
                    setMediaProgression({
                        progression: watchedMedia.currentTime / watchedMedia.totalDuration,
                        remainingTime: watchedMedia.totalDuration - watchedMedia.currentTime,
                        currentTime: watchedMedia.currentTime
                    });
                }
            }
        )
    }


    const getAvailableSubtitles = (imdbCode, seasonNumber, episodeNumber) => {
        setLoadingSubtitles(true);
        AppServices.subtitlesApiService.getAvailableSubtitles(imdbCode, seasonNumber, episodeNumber,
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

    const onWatchedTimeUpdate = (currentTime, duration) => {
        AppServices.watchedMediaApiService.saveWacthedMedia(mediaDetails, currentTime, duration, selectedEpisode.current.seasonNumber, selectedEpisode.current.episodeNumber);
    }

    const onRecommandedMediaClick = (mediaId) => {
        navHistory.current.push(mediaDetails.id);
        loadPage(mediaId);
    }

    const onBackClick = () => {
        if (navHistory.current.length > 0) {
            let lastMediaId = navHistory.current.pop();
            loadPage(lastMediaId);
        }
        else
            onCloseClick();
    }

    const onEpisodeSelected = (seasonNumber, episodeNumber) => {
        clearVersionSources();
        selectedEpisode.current = {
            seasonNumber: seasonNumber,
            episodeNumber: episodeNumber
        };
        searchVoSources(mediaDetails.title, mediaDetails.year, mediaDetails.imdbId, seasonNumber, episodeNumber);
        searchVfSources(mediaDetails.id, mediaDetails.title, mediaDetails.year, seasonNumber, episodeNumber);
        getAvailableSubtitles(mediaDetails.imdbId, seasonNumber, episodeNumber);
        getMediaProgression(mediaDetails.id, seasonNumber, episodeNumber);
    }

    return (
        <div style={{ height: '100%' }}>
            <VideoPlayerWindow
                visible={showMediaPlayer}
                sources={selectedVersionSources}
                subtitles={mediaSubtitlesSources}
                onCloseClick={() => setShowMediaPlayer(false)}
                onWatchedTimeUpdate={(currentTime, duration) => onWatchedTimeUpdate(currentTime, duration)}
                goToTime={mediaProgression?.currentTime}
                mediaDetails={mediaDetails} />
            <CircularProgressBar color={'white'} size={'80px'} position={"center"} visible={!dataLoaded} />
            <ModalMediaTrailer visible={showMediaTrailer} youtubeTrailerUrl={mediaDetails.youtubeTrailerUrl} onCloseClick={() => setShowMediaTrailer(false)} />
            <div style={{ display: dataLoaded && !mediaDetails.title ? 'flex' : 'none' }} className="no-media-details-message">
                <Paragraph text={"No info found for this media"} />
                <BaseButton color="red" content={"Back"} onClick={() => onBackClick()} />
            </div>
            <div style={fadeTransition(dataLoaded && mediaDetails.title)} className="media-presentation-page-container">
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
                                <SecondaryInfo center text={mediaDetails.year + " | " + (mediaDetails.duration ? ToTimeFormat(mediaDetails.duration) + " | " : '') + mediaDetails.genres?.map(genre => genre.name).join(', ')} />
                                <MediaProgression mediaProgression={mediaProgression?.progression} remainingTime={mediaProgression?.remainingTime} />
                                <div className="horizontal">
                                    <TrailerButton visible={mediaDetails?.youtubeTrailerUrl} onClick={() => setShowMediaTrailer(true)} />
                                    <AddBookmarkButton onClick={() => bookmarkMedia()} visible={addBookmarkButtonVisible} />
                                    <RemoveBookmarkButton onClick={() => unbookmarkMedia()} visible={!addBookmarkButtonVisible} />
                                </div>
                                <div className="play-options">
                                    <EpisodeSelector
                                        serieId={mediaDetails.id}
                                        seasonsCount={mediaDetails.seasonsCount}
                                        onEpisodeSelected={(seasonNumber, episodeNumber) => onEpisodeSelected(seasonNumber, episodeNumber)} />
                                    <AvailableSubtitles loading={loadingSubtitles} availableSubtitlesSources={mediaSubtitlesSources} />
                                    <AvailableVersions
                                        loading={voSourcesSearching || vfSourcesSearching}
                                        availableVersionsSources={versionsSources.current}
                                        onVersionSelected={(versionSources) => { setSelectedVersionSourceLink(null); setSelectedVersionSources(versionSources.sources) }} />
                                    <div style={fadeTransition(!(voSourcesSearching || vfSourcesSearching) && Boolean(selectedVersionSources) && selectedVersionSources.length > 0)} >
                                        <div className="horizontal">
                                            <QualitySelector versionSources={selectedVersionSources} onQualityChanged={(i) => changeSelectedSource(i)} />
                                            <PlayButton onClick={() => setShowMediaPlayer(true)} />
                                            <PlayWithVLCButton
                                                videoUrl={selectedVersionSourceLink}
                                                onClick={() => { if (mediaDetails) AppServices.watchedMediaApiService.saveWacthedMedia(mediaDetails, 0, 0, selectedEpisode.current.seasonNumber, selectedEpisode.current.episodeNumber) }} />
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
                                medias={similarMedias}
                                alignLeft
                                listTitle="You may also like"
                                visible={similarMedias && similarMedias.length > 0}
                                onMediaClick={(mediaId) => { onRecommandedMediaClick(mediaId) }} />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}
export default MediaFullPresentation;


function MediaProgression({ mediaProgression, remainingTime }) {
    if (mediaProgression > 0) {
        return (
            <div>
                <ProgressionBar width="100%" value={mediaProgression * 100} />
                <SecondaryInfo text={ToTimeFormat(remainingTime / 60) + ' remaining'} />
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

    useEffect(() => {
        if (loading)
            setSelectedIndex(0);
    }, [loading]);

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

    versionSources.sort((source1, source2) => {
        const [quality1, quality2] = [source1.quality.toLowerCase().trim(), source2.quality.toLowerCase().trim()]
        return quality1 === quality2 ? 0 : (quality1 < quality2 ? -1 : 1);
    });

    const qualities = [];

    versionSources.forEach(source => {
        const nbSameQualities = qualities.filter(q => q.startsWith(source.quality)).length;
        qualities.push(source.quality + (nbSameQualities > 0 ? " (" + nbSameQualities + ")" : ""));
    });

    const content = <DropDown
        values={qualities}
        width="130px"
        onValueChanged={(index) => onQualityChanged(index)} />

    return <TitleAndContent title="Qualities" content={content} justify="left" />

}

function EpisodeSelector({ serieId, seasonsCount, onEpisodeSelected }) {
    const [modalVisible, setModalVisible] = useState(false);
    const [selectedSeasonNumber, setSelectedSeasonNumber] = useState(1)
    const [selectedEpisodeNumber, setSelectedEpisodeNumber] = useState(1)

    useEffect(() => {
        setSelectedSeasonNumber(1);
        setSelectedEpisodeNumber(1);
    }, [seasonsCount]);

    if (!seasonsCount)
        return null;

    const onButtonClick = () => {
        setModalVisible(true);
    }

    const containerStyle = {
        margin: '12px auto',
    }

    const handleEpisodeSelected = (seasonNumber, episodeNumber) => {
        setSelectedSeasonNumber(seasonNumber);
        setSelectedEpisodeNumber(episodeNumber);
        onEpisodeSelected(seasonNumber, episodeNumber);
        setModalVisible(false);
    }

    return <div style={containerStyle}>
        <ModalEpisodeSelector
            visible={modalVisible}
            serieId={serieId}
            numberOfSeasons={seasonsCount}
            onEpisodeSelected={(seasonNumber, episodeNumber) => handleEpisodeSelected(seasonNumber, episodeNumber)}
            onCloseClick={() => setModalVisible(false)} />
        <BaseButton
            color="red"
            content={"Season " + selectedSeasonNumber + " - Episode " + selectedEpisodeNumber}
            onClick={() => onButtonClick()} />
    </div>
}


