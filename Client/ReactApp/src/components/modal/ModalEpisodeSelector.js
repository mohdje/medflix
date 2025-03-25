import "../../style/css/modal-episode-selector.css";
import "../../style/css/animations.css";

import ModalWindow from "./ModalWindow";
import CircularProgressBar from "../common/CircularProgressBar";
import ProgressionBar from "../common/ProgressionBar";
import DropDown from "../common/DropDown";
import { mediasInfoApi, watchHistoryApi } from "../../services/api";
import { ToTimeFormat } from "../../helpers/timeFormatHelper";

import { useEffect, useState, useRef } from "react";

export default function ModalEpisodeSelector({ visible, serieId, numberOfSeasons, selectedSeason, onEpisodeSelected, onCloseClick }) {
    const [selectedSeasonNumber, setSelectedSeasonNumber] = useState(selectedSeason);
    const [episodes, setEpisodes] = useState([]);
    const [isLoading, setIsLoading] = useState(false);
    const [noEpisodeMessageVisible, setNoEpisodeMessageVisible] = useState(false);
    const episodeListRef = useRef(null);

    const seasonNumberList = Array.from(
        { length: numberOfSeasons },
        (_, i) => `Season ${i + 1}`
    );

    useEffect(() => {
        const loadEpisodes = async () => {
            if (serieId) {
                setEpisodes([]);
                setIsLoading(true);
                setNoEpisodeMessageVisible(false);
                const episodes = await mediasInfoApi.getEpisodes(serieId, selectedSeasonNumber);

                if (episodeListRef.current)
                    episodeListRef.current.scrollTop = 0;

                setIsLoading(false);
                if (episodes && episodes.length > 0) {
                    loadEpisodesProgress(episodes, selectedSeasonNumber);
                }
                else {
                    setIsLoading(false);
                    setNoEpisodeMessageVisible(true)
                }
            }
        };
        loadEpisodes();

    }, [serieId, selectedSeasonNumber]);

    const loadEpisodesProgress = async (episodesOfSeason, seasonNumber) => {
        const watchedEpisodes = await watchHistoryApi.getWatchedEpisodes(serieId, seasonNumber);
        const episodes = [...episodesOfSeason];

        if (watchedEpisodes?.length > 0) {
            watchedEpisodes.forEach(watchedEpisode => {
                const episode = episodes.find(ep => ep.episodeNumber === watchedEpisode.episodeNumber);
                if (episode)
                    episode.progression = {
                        currentTime: watchedEpisode.currentTime,
                        totalDuration: watchedEpisode.totalDuration,
                        videoSource: watchedEpisode.videoSource
                    }
            });
        }
        setEpisodes(episodes);
    }

    const modalContent = () => {
        return (
            <div className="modal-episode-selector-container">
                <div className="season-selector-container">
                    <DropDown values={seasonNumberList} width="120px" textAlignement={"center"} onValueChanged={(selectedIndex) => setSelectedSeasonNumber(selectedIndex + 1)} />
                </div>
                <div ref={episodeListRef} className="episode-list-container">
                    <div className="loading-container">
                        <CircularProgressBar size="large" visible={isLoading} />
                        {noEpisodeMessageVisible ? <h3>No episodes found</h3> : null}
                    </div>
                    {episodes.map(episode =>
                        <Episode
                            key={episode.episodeNumber}
                            episode={episode}
                            onClick={() => onEpisodeSelected(selectedSeasonNumber, episode.episodeNumber, episode.progression)} />)}
                </div>
            </div>
        );
    };

    return (
        <ModalWindow visible={visible} content={modalContent()} onCloseClick={() => onCloseClick()} />
    );
}

function Episode({ episode, onClick }) {
    const releaseDate = new Date(episode.airDate);
    let episodeInfo = releaseDate > new Date() ? `Release on ${releaseDate.toLocaleDateString()}` : ToTimeFormat(episode.runTime);

    const computeProgress = (progression) => (progression.currentTime / progression.totalDuration) * 100;
    return <div className="episode-container fade-in" onClick={() => onClick()}>
        <div>
            <img src={episode.imagePath} />
            {episode.progression ? <ProgressionBar value={computeProgress(episode.progression)} width="98%" /> : null}
        </div>
        <div>
            <h3 className="title">{`${episode.episodeNumber}.${episode.name}`}<span className="episode-info">{episodeInfo}</span></h3>
            <p className="synopsis">{episode.overview.substring(0, 300)}</p>
        </div>
    </div>
}
