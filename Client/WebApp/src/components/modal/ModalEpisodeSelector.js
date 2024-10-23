import ModalWindow from "./ModalWindow";
import TitleAndContent from "../common/TitleAndContent";
import CircularProgressBar from "../common/CircularProgressBar";
import ProgressionBar from "../common/ProgressionBar";
import SecondaryInfo from "../common/text/SecondaryInfo";
import Paragraph from "../common/text/Paragraph";
import DropDown from "../common/DropDown";
import AppServices from "../../services/AppServices";
import fadeTransition from "../../helpers/customStyles.js";
import { ToTimeFormat } from "../../helpers/timeFormatHelper";

import { useEffect, useState, useRef } from "react";

import "../../style/modal-episode-selector.css";

function ModalEpisodeSelector({ visible, serieId, numberOfSeasons, onEpisodeSelected, onCloseClick }) {
    const [selectedSeasonNumber, setSelectedSeasonNumber] = useState(1);
    const [episodes, setEpisodes] = useState([]);
    const [isLoading, setIsLoading] = useState(false);
    const [noEpisodeMessageVisible, setNoEpisodeMessageVisible] = useState(false);
    const episodeListRef = useRef(null);

    const seasonNumberList = [];
    for (let i = 1; i <= numberOfSeasons; i++) {
        seasonNumberList.push("Season " + i);
    }

    const content = () => {
        return (
            <div className="modal-episode-selector-container">
                <div className="season-selector-container">
                    <DropDown values={seasonNumberList} width="120px" textAlignement={"center"} onValueChanged={(selectedIndex) => setSelectedSeasonNumber(selectedIndex + 1)} />
                </div>
                <div ref={episodeListRef} className="episode-list-container">
                    <div className="loading-container">
                        <CircularProgressBar color={"white"} size={"50px"} visible={isLoading} />
                        <Paragraph text={"No episodes found"} visible={noEpisodeMessageVisible} />
                    </div>
                    <div style={fadeTransition(episodes && episodes.length > 0)}>
                        {episodes.map(episode =>
                            <Episode
                                key={episode.episodeNumber}
                                episode={episode}
                                onClick={() => onEpisodeSelected(selectedSeasonNumber, episode.episodeNumber)} />)}
                    </div>

                </div>
            </div>
        );
    };

    useEffect(() => {
        if (serieId) {
            setEpisodes([]);
            setIsLoading(true);
            setNoEpisodeMessageVisible(false)
            AppServices.searchMediaService.getEpisodes(serieId, selectedSeasonNumber,
                (episodes) => {
                    if (episodeListRef.current)
                        episodeListRef.current.scrollTop = 0;

                    setIsLoading(false);
                    if (episodes && episodes.length > 0) {
                        loadEpisodesProgress(episodes, selectedSeasonNumber);
                    }
                    else
                        setNoEpisodeMessageVisible(true)
                },
                () => {
                    setIsLoading(false);
                    setNoEpisodeMessageVisible(true)
                })
        }
    }, [serieId, selectedSeasonNumber]);

    const loadEpisodesProgress = (episodesOfSeason, seasonNumber) => {
        AppServices.watchedMediaApiService.getWatchedEpisodes(serieId, seasonNumber,
            (watchedEpisodes) => {
                const newTab = [...episodesOfSeason]
                if (watchedEpisodes && watchedEpisodes.length > 0) {
                    watchedEpisodes.forEach(watchedEpisode => {
                        const episode = newTab.find(ep => ep.episodeNumber === watchedEpisode.episodeNumber);
                        if (episode)
                            episode.progression = watchedEpisode.currentTime / watchedEpisode.totalDuration;
                    });
                }
                setEpisodes(newTab);
            },
            () => {
                setEpisodes(episodesOfSeason);
            });
    }

    return (
        <ModalWindow visible={visible} content={content()} onCloseClick={() => onCloseClick()} />
    );
}

export default ModalEpisodeSelector;

function Episode({ episode, onClick }) {
    return <div className="episode-container" onClick={() => onClick()}>
        <div style={{ margin: 'auto 0' }}>
            <img src={episode.imagePath} />
            {episode.progression ? <ProgressionBar value={episode.progression * 100} width="98%" /> : null}
        </div>

        <div>
            <TitleAndContent title={episode.episodeNumber + '.' + episode.name} content={ToTimeFormat(episode.runTime)} justify="left" />
            <SecondaryInfo text={episode.overview.substring(0, 300)} />
        </div>
    </div>
}
