
import "../style/css/media-presentation-page.css";
import "../style/css/animations.css";

import BackIcon from "../assets/back.svg";
import TrailerVideoIcon from "../assets/trailer_video.svg";
import PlusIcon from "../assets/plus.svg";
import MinusIcon from "../assets/minus.svg";
import PlayIcon from "../assets/play.svg";
import RestartIcon from "../assets/restart.svg";

import CircularProgressBar from "../components/common/CircularProgressBar";
import ProgressionBar from "../components/common/ProgressionBar";
import Button from "../components/common/Button.js";
import Toast from "../components/common/Toast.js";

import ModalMediaTrailer from '../components/modal/ModalMediaTrailer';
import ModalEpisodeSelector from '../components/modal/ModalEpisodeSelector';
import MediasHorizontalList from "../components/media/list/MediasHorizontalList";
import NativeMediaPlayer from "../components/video/NativeMediaPlayer.js";

import {
    mediaSourcesApi,
    mediasInfoApi,
    watchHistoryApi,
    bookmarkApi
} from "../services/api";
import { ToTimeFormat } from "../helpers/timeFormatHelper";
import { eventsNames, raiseEvent } from "../helpers/eventHelper.js";

import { useEffect, useState, useRef } from 'react';

export default function MediaFullPresentation({ mediaId, onSimilarMediaClick, onCloseClick }) {

    const [loadingMediaDetails, setLoadingMediaDetails] = useState(true);
    const [mediaDetails, setMediaDetails] = useState({});
    const [mediaProgression, setMediaProgression] = useState({});
    const [shouldRestart, setShouldRestart] = useState(false);

    const [mediaVersionsSources, setMediaVersionsSources] = useState([]);
    const [searchingVersionsSources, setSearchingVersionsSources] = useState([]);

    const [mediaSubtitlesSources, setMediaSubtitlesSources] = useState([]);
    const [loadingSubtitles, setLoadingSubtitles] = useState(false);

    const [showMediaPlayer, setShowMediaPlayer] = useState(false);
    const [showMediaTrailer, setShowMediaTrailer] = useState(false);
    const [showEpisodeSelection, setShowEpisodeSelection] = useState(false);
    const [showToastMessage, setShowToastMessage] = useState(false);
    const [toastMessage, setToastMessage] = useState(null);

    const [mediaIsBookmarked, setMediaIsBookmarked] = useState(true);
    const [changingBookmark, setChangingBookmark] = useState(false);

    const [similarMedias, setSimilarMedias] = useState([]);

    const selectedEpisode = useRef({
        seasonNumber: 1,
        episodeNumber: 1
    })

    useEffect(() => {
        const loadPage = async () => {
            if (!mediaId)
                return;

            const mediaDetailsResult = await mediasInfoApi.getMediaDetails(mediaId);

            if (mediaDetailsResult) {
                setMediaDetails(mediaDetailsResult);
                await Promise.all([
                    getWatchMediaInfo().then(watchedMedia => {
                        searchAvailableSubtitles(mediaDetailsResult, selectedEpisode.current.seasonNumber, selectedEpisode.current.episodeNumber);
                        searchMediaVersionsSources(mediaDetailsResult, selectedEpisode.current.seasonNumber, selectedEpisode.current.episodeNumber);
                    }),
                    mediasInfoApi.getSimilarMedias(mediaId).then((medias) => setSimilarMedias(medias)),
                    bookmarkApi.isMediaBookmarked(mediaId).then(isBookmarked => setMediaIsBookmarked(isBookmarked))
                ]);
            }
            setLoadingMediaDetails(false);
        }
        loadPage();

    }, []);

    const getWatchMediaInfo = async () => {
        const watchedMedia = await watchHistoryApi.getLastWatchedMediaInfo(mediaId);
        if (watchedMedia) {
            selectedEpisode.current = {
                episodeNumber: watchedMedia.episodeNumber,
                seasonNumber: watchedMedia.seasonNumber
            };

            setMediaProgression({
                progression: watchedMedia.currentTime / watchedMedia.totalDuration,
                remainingTime: watchedMedia.totalDuration - watchedMedia.currentTime,
                currentTime: watchedMedia.currentTime,
                videoSource: watchedMedia.videoSource
            })
        }
        return watchedMedia;
    }

    const searchMediaVersionsSources = async (mediaDetails, seasonNumber, episodeNumber) => {
        setMediaVersionsSources([]);
        setSearchingVersionsSources(true);

        const availableVersionsSources = await mediaSourcesApi.getAvailableVersionsSources(mediaDetails.id, mediaDetails.title, mediaDetails.year, mediaDetails.imdbId, seasonNumber, episodeNumber);

        setSearchingVersionsSources(false);
        setMediaVersionsSources(availableVersionsSources);
    }

    const searchAvailableSubtitles = async (mediaDetails, seasonNumber, episodeNumber) => {
        setLoadingSubtitles(true);
        setMediaSubtitlesSources(null);
        const subtitlesSources = await mediaSourcesApi.getAvailableSubtitles(mediaDetails.imdbId, seasonNumber, episodeNumber);

        if (subtitlesSources)
            setMediaSubtitlesSources(subtitlesSources);

        setLoadingSubtitles(false)
    }

    const toggleBookmark = async () => {
        setChangingBookmark(true);
        const result = mediaIsBookmarked ? await bookmarkApi.unbookmarkMedia(mediaDetails) : await bookmarkApi.bookmarkMedia(mediaDetails);
        if (result) {
            displayToastMessage(mediaIsBookmarked ? "Removed from your list with success" : "Added to your list with success");
            setMediaIsBookmarked(!mediaIsBookmarked);
            raiseEvent(eventsNames.bookmarkUpdated);
        }
        else {
            displayToastMessage("Failing to add/remove from your list");
        }

        setChangingBookmark(false);
    }

    const onEpisodeSelected = (seasonNumber, episodeNumber, watchProgress) => {
        selectedEpisode.current = {
            seasonNumber: seasonNumber,
            episodeNumber: episodeNumber
        };
        setShowEpisodeSelection(false);
        searchMediaVersionsSources(mediaDetails, seasonNumber, episodeNumber);
        searchAvailableSubtitles(mediaDetails, seasonNumber, episodeNumber);
        if (watchProgress)
            setMediaProgression({
                progression: watchProgress.currentTime / watchProgress.totalDuration,
                remainingTime: watchProgress.totalDuration - watchProgress.currentTime,
                currentTime: watchProgress.currentTime,
                videoSource: watchProgress.videoSource
            });
        else
            setMediaProgression({});
    }

    const openMediaPlayer = (restart) => {
        setShouldRestart(restart);
        setShowMediaPlayer(true);
    };

    const closeMediaPlayer = () => {
        setShowMediaPlayer(false);
        getWatchMediaInfo();
        raiseEvent(eventsNames.watchProgressUpdated);
    };

    const displayToastMessage = (message) => {
        setToastMessage(message);
        setShowToastMessage(true);
        setTimeout(() => setShowToastMessage(false), 3000);
    }

    const enablePlayButton = mediaVersionsSources?.length > 0;
    const enableTrailerButton = mediaDetails?.youtubeTrailerUrl;
    const selectedEpisodeIndentifier = `Season ${selectedEpisode.current.seasonNumber} Episode ${selectedEpisode.current.episodeNumber}`;
    const noInfoMessageStyle = {
        display: !loadingMediaDetails && !mediaDetails.title ? 'flex' : 'none',
        flexDirection: 'column',
        alignItems: 'center',
        marginTop: '10%',
        gap: '10px'
    }

    return (
        <>
            <NativeMediaPlayer
                visible={showMediaPlayer}
                mediaDetails={mediaDetails}
                currentTime={mediaProgression?.currentTime}
                shouldRestart={shouldRestart}
                videoSource={mediaProgression?.videoSource}
                episodeNumber={selectedEpisode.current.episodeNumber}
                seasonNumber={selectedEpisode.current.seasonNumber}
                mediaSources={mediaVersionsSources}
                subtitlesSources={mediaSubtitlesSources}
                onCloseClick={() => closeMediaPlayer()} />
            <CircularProgressBar size="x-large" position="center" visible={loadingMediaDetails} />
            <ModalMediaTrailer visible={showMediaTrailer} youtubeTrailerUrl={mediaDetails?.youtubeTrailerUrl} onCloseClick={() => setShowMediaTrailer(false)} />
            {mediaDetails?.seasonsCount ? <ModalEpisodeSelector
                visible={showEpisodeSelection}
                serieId={mediaDetails?.id}
                numberOfSeasons={mediaDetails?.seasonsCount}
                defaultSeasonNumber={selectedEpisode.current.seasonNumber}
                onEpisodeSelected={(seasonNumber, episodeNumber, watchProgress) => onEpisodeSelected(seasonNumber, episodeNumber, watchProgress)}
                onCloseClick={() => setShowEpisodeSelection(false)} /> : null}
            <Toast message={toastMessage} visible={showToastMessage} />
            <div style={noInfoMessageStyle}>
                <h3>No info found for this media</h3>
                <Button color="red" text="Back" onClick={() => onCloseClick()} />
            </div>

            {
                !loadingMediaDetails && mediaDetails.title ?
                    <div className="media-presentation-page-container fade-in">
                        <div className="back-btn-container">
                            <Button imgSrc={BackIcon} color="transparent" rounded onClick={() => onCloseClick()} />
                        </div>
                        <div className="presentation" style={{ backgroundImage: 'url(' + mediaDetails.backgroundImageUrl + ')' }}>
                            <div className="info-container">
                                <div className="info-section-top">
                                    <div className="title">
                                        {mediaDetails.logoImageUrl ? <img src={mediaDetails.logoImageUrl} /> : <h1>{mediaDetails.title} </h1>}
                                    </div>
                                    <div className="metadata-list">
                                        <h3 className="metadata-item">{mediaDetails.rating.toFixed(1)}</h3>
                                        <h3 className="metadata-item">{mediaDetails.year}</h3>
                                        {mediaDetails.duration ? <h3 className="metadata-item">{ToTimeFormat(mediaDetails.duration)}</h3> : null}
                                        <h3 className="metadata-item">{mediaDetails.genres?.map(genre => genre.name).join(' • ')}</h3>
                                    </div>
                                    {mediaProgression?.progression ?
                                        <div className="watch-progression">
                                            <ProgressionBar width="100%" value={mediaProgression.progression * 100} />
                                            <h3>{ToTimeFormat(mediaProgression.remainingTime / 60) + ' remaining'}</h3>
                                        </div>
                                        : null}

                                    <div className="actions-container">
                                        {mediaDetails?.seasonsCount ? <Button text={selectedEpisodeIndentifier} color="white" onClick={() => { setShowEpisodeSelection(true) }} /> : null}
                                        <Button imgSrc={PlayIcon} disabled={!enablePlayButton} text={mediaProgression?.progression ? "Resume" : "Play"} color="red" onClick={() => openMediaPlayer(false)} />
                                        {mediaProgression?.progression ? <Button imgSrc={RestartIcon} disabled={!enablePlayButton} rounded color="white" onClick={() => openMediaPlayer(true)} /> : null}
                                        <Button imgSrc={TrailerVideoIcon} disabled={!enableTrailerButton} color="transparent" rounded onClick={() => setShowMediaTrailer(true)} />
                                        <Button imgSrc={mediaIsBookmarked ? MinusIcon : PlusIcon} disabled={changingBookmark} color="transparent" rounded onClick={() => toggleBookmark()} />
                                    </div>
                                </div>
                                <div className="info-section-bottom">
                                    <div className="available-resources">
                                        <AvailableItems
                                            title="Versions"
                                            isLoading={searchingVersionsSources}
                                            itemsList={mediaVersionsSources.map(v => v.language)}
                                            emptyMessage={"No version available"} />
                                        <AvailableItems
                                            title="Subtitles"
                                            isLoading={loadingSubtitles}
                                            itemsList={mediaSubtitlesSources?.map(s => s.language)}
                                            emptyMessage={"No subtitles available"} />
                                    </div>
                                    <div className="media-details">
                                        <TitleAndContent title="Director" content={mediaDetails.director} />
                                        <TitleAndContent title="Cast" content={mediaDetails.cast} />
                                        <p>{mediaDetails.synopsis}</p>
                                    </div>

                                    {similarMedias?.length > 0 ? <MediasHorizontalList
                                        medias={similarMedias}
                                        alignLeft
                                        title="You may also like"
                                        visible={similarMedias && similarMedias.length > 0}
                                        onMediaClick={(media) => { onSimilarMediaClick(media) }} /> : null}
                                </div>
                            </div>
                        </div>
                    </div> : null
            }
        </>
    );
}

function AvailableItems({ title, itemsList, emptyMessage, isLoading }) {
    const availableItems = itemsList?.length > 0 ? itemsList.join(", ") : emptyMessage;
    return (
        <div style={{ width: "250px" }}>
            <TitleAndContent isLoading={isLoading} title={title} content={availableItems} />
        </div>
    )
}

function TitleAndContent({ isLoading, title, content }) {
    if (!content)
        return null;

    return (
        <div className="title-and-content">
            <h3 className="title">{title}</h3>
            {isLoading ? <CircularProgressBar size="small" visible /> : <h3 className="content">{content}</h3>}
        </div>
    )
}
