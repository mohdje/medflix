
import "../style/media-presentation-page.css";

import ArrowBackIcon from '@material-ui/icons/ArrowBack';

import ModalMediaTrailer from '../components/modal/ModalMediaTrailer';
import ModalEpisodeSelector from '../components/modal/ModalEpisodeSelector';
import ModalVersionAndQualitySelector from '../components/modal/ModalVersionAndQualitySelector';

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
import MediasListLiteWithTitle from "../components/media/list/MediasListLiteWithTitle";

import AppServices from "../services/AppServices";
import { ToTimeFormat } from "../helpers/timeFormatHelper";
import fadeTransition from "../helpers/customStyles.js";

import { useEffect, useState, useRef, version } from 'react';


function MediaFullPresentation({ mediaId, onCloseClick }) {

    const [dataLoaded, setDataLoaded] = useState(false);
    const [mediaDetails, setMediaDetails] = useState({});
    const [mediaProgression, setMediaProgression] = useState({});

    const torrents = useRef([]);
    const [torrentSearching, setTorrentSearching] = useState(false);
    const [updateSelectedTorrent, setUpdateSelectedTorrent] = useState(true);

    const [selectedVersionSources, setSelectedVersionSources] = useState([]);
    const [videoSourceUrl, setVideoSourceUrl] = useState('');

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
        if (mediaDetails.imdbId) {
            getAvailableSubtitles(mediaDetails.imdbId, selectedEpisode.current.seasonNumber, selectedEpisode.current.episodeNumber);
            if (mediaDetails.id && mediaDetails.title && mediaDetails.year)
                searchTorrents(mediaDetails);
        }

        const topPage = document.getElementsByClassName('back-btn')[0];
        topPage.scrollIntoView();
    }, [mediaDetails]);

    useEffect(() => {
        if (updateSelectedTorrent && mediaProgression && torrents.current?.length > 0) {
            const torrentToSelect = torrents.current.find(torrent => torrent.downloadUrl === mediaProgression.torrentUrl);
            if (torrentToSelect) {
                changeSelectedTorrent(torrentToSelect);
                setUpdateSelectedTorrent(false);
            }
        }
    }, [mediaProgression, torrents.current]);

    const searchTorrents = (mediaDetails) => {
        torrents.current = [];
        setVideoSourceUrl('');
        setTorrentSearching(true);
        AppServices.torrentApiService.searchTorrents(mediaDetails.id, mediaDetails.title, mediaDetails.year, mediaDetails.imdbId, selectedEpisode.current.seasonNumber, selectedEpisode.current.episodeNumber,
            (result) => {
                setTorrentSearching(false);
                if (result && result.length > 0) {
                    torrents.current = result.map(torrent => ({ ...torrent, seasonNumber: selectedEpisode.current.seasonNumber, episodeNumber: selectedEpisode.current.episodeNumber }));
                    changeSelectedTorrent(torrents.current[0]);
                }
            }, () => {
                setTorrentSearching(false);
            }
        );
    }

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

    const changeSelectedTorrent = (selectedTorrent) => {
        const refreshedTorrentsList = [];
        for (let i = 0; i < torrents.current.length; i++) {
            const torrent = torrents.current[i];
            torrent.selected = torrent.downloadUrl == selectedTorrent.downloadUrl;
            refreshedTorrentsList.push(torrent);
        }
        torrents.current = refreshedTorrentsList;

        setVideoSourceUrl(
            selectedTorrent ? AppServices.torrentApiService.buildStreamUrl(selectedTorrent.downloadUrl, '', selectedTorrent.seasonNumber, selectedTorrent.episodeNumber)
                : '');

        setSelectedVersionSources(torrents.current.filter(t => t.languageVersion === selectedTorrent.languageVersion));
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
                        currentTime: watchedMedia.currentTime,
                        torrentUrl: watchedMedia.torrentUrl
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

    const onWatchedTimeUpdate = (currentTime, duration, sourceUrl) => {
        AppServices.watchedMediaApiService.saveWacthedMedia(mediaDetails, currentTime, duration, selectedEpisode.current.seasonNumber, selectedEpisode.current.episodeNumber, sourceUrl);
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
        selectedEpisode.current = {
            seasonNumber: seasonNumber,
            episodeNumber: episodeNumber
        };
        searchTorrents(mediaDetails);
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
                onWatchedTimeUpdate={(currentTime, duration, sourceUrl) => onWatchedTimeUpdate(currentTime, duration, sourceUrl)}
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
                                    <EpisodeSelector
                                        serieId={mediaDetails.id}
                                        seasonsCount={mediaDetails.seasonsCount}
                                        onEpisodeSelected={(seasonNumber, episodeNumber) => onEpisodeSelected(seasonNumber, episodeNumber)} />
                                    <AddBookmarkButton onClick={() => bookmarkMedia()} visible={addBookmarkButtonVisible} />
                                    <RemoveBookmarkButton onClick={() => unbookmarkMedia()} visible={!addBookmarkButtonVisible} />
                                </div>

                            </div>
                        </div>
                        <div className="play-options">
                            <AvailableItems
                                title="Subtitles"
                                isLoading={loadingSubtitles}
                                itemsList={mediaSubtitlesSources && mediaSubtitlesSources.map(s => s.language)}
                                emptyMessage={"No subtitles available"} />
                            <div className="horizontal">
                                <AvailableItems
                                    title="Versions"
                                    isLoading={torrentSearching}
                                    itemsList={torrents.current && [...new Set(torrents.current.map(t => t.languageVersion))]}
                                    emptyMessage={"No version available"} />
                                <div style={fadeTransition(!torrentSearching && torrents.current?.length > 0)} className="horizontal">
                                    <QualitySelector torrents={torrents.current} onQualityChanged={(torrent) => changeSelectedTorrent(torrent)} />
                                    <PlayButton onClick={() => setShowMediaPlayer(true)} />
                                    <PlayWithVLCButton
                                        videoUrl={videoSourceUrl}
                                        onClick={() => { if (mediaDetails) AppServices.watchedMediaApiService.saveWacthedMedia(mediaDetails, 0, 0, selectedEpisode.current.seasonNumber, selectedEpisode.current.episodeNumber) }} />
                                </div>
                            </div>
                        </div>
                        <div className="extra">
                            <div className="media-details">
                                <TitleAndContent title="Director" content={mediaDetails.director} justify="left" />
                                <TitleAndContent title="Cast" content={mediaDetails.cast} justify="left" />
                                <Paragraph text={mediaDetails.synopsis}></Paragraph>
                            </div>

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

function AvailableItems({ title, itemsList, emptyMessage, isLoading }) {
    const availableItems = itemsList?.length > 0 ? itemsList.join(", ") : emptyMessage;
    const content = isLoading ? <CircularProgressBar color={'white'} size={'15px'} visible={true} /> : availableItems;
    return (
        <TitleAndContent title={title} content={content} />
    )
}

function QualitySelector({ torrents, onQualityChanged }) {
    const [modalQualitySelectorVisible, setModalQualitySelectorVisible] = useState(false);
    const [selectedVersionAndQuality, setSelectedVersionAndQuality] = useState(false);

    useEffect(() => {
        if (torrents?.length > 0) {
            const selectedTorrent = torrents.find(t => t.selected);
            if (selectedTorrent) {
                setSelectedVersionAndQuality(selectedTorrent.languageVersion + " | " + selectedTorrent.quality);
            }
        }

    }, [torrents]);


    const onQualityClick = (torrent) => {
        setModalQualitySelectorVisible(false);
        const selectedTorrent = torrents.find(t => t.selected);
        if (selectedTorrent.downloadUrl !== torrent.downloadUrl)
            onQualityChanged(torrent);
    }

    return <div>
        <BaseButton color="grey" content={selectedVersionAndQuality} onClick={() => setModalQualitySelectorVisible(true)} />
        <ModalVersionAndQualitySelector
            visible={modalQualitySelectorVisible}
            torrents={torrents}
            onQualityClick={(torrent) => onQualityClick(torrent)} />
    </div>
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

    const handleEpisodeSelected = (seasonNumber, episodeNumber) => {
        setSelectedSeasonNumber(seasonNumber);
        setSelectedEpisodeNumber(episodeNumber);
        onEpisodeSelected(seasonNumber, episodeNumber);
        setModalVisible(false);
    }

    return <div>
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


