import ModalWindow from "./ModalWindow";
import TitleAndContent from "../common/TitleAndContent";
import CircularProgressBar from "../common/CircularProgressBar";
import SecondaryInfo from "../common/text/SecondaryInfo";
import Paragraph from "../common/text/Paragraph";
import DropDown from "../common/DropDown";
import AppServices from "../../services/AppServices";

import { useEffect, useState, useRef } from "react";

import "../../style/modal-episode-selector.css";

function ModalEpisodeSelector({visible, serieId, numberOfSeasons, onEpisodeSelected, onCloseClick}){
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
            <div class="modal-episode-selector-container">
                <div class="season-selector-container">
                    <DropDown values={seasonNumberList}  width="120px" textAlignement={"center"} onValueChanged={(selectedIndex) => setSelectedSeasonNumber(selectedIndex + 1)}/>
                </div>
                <div ref={episodeListRef} class="episode-list-container">
                    <div class="loading-container">
                        <CircularProgressBar color={"white"} size={"50px"} visible={isLoading}/>
                        <Paragraph text={"No episodes found"} visible={noEpisodeMessageVisible}/>
                    </div>
                   
                    {episodes.map(episode => 
                        <Episode 
                            key={episode.episodeNumber} 
                            episode={episode}
                            onClick={()=>onEpisodeSelected(selectedSeasonNumber, episode.episodeNumber)}/>)}
                </div>
            </div>
        );
    };

    useEffect(()=>{
        

        if(serieId){
            setEpisodes([]);
            setIsLoading(true);
            setNoEpisodeMessageVisible(false)
            AppServices.searchMediaService.getEpisodes(serieId, selectedSeasonNumber,
                (episodes)=>{
                    if(episodeListRef.current)
                        episodeListRef.current.scrollTop = 0;

                    setIsLoading(false);
                    if(episodes && episodes.length > 0)
                        setEpisodes(episodes);
                    else 
                        setNoEpisodeMessageVisible(true)
                },
                ()=>{
                    setIsLoading(false);
                    setNoEpisodeMessageVisible(true)
                })
        }
       
        
    },[serieId, selectedSeasonNumber]);

    return (
        <ModalWindow visible={visible} content={content()} onCloseClick={() => onCloseClick()} />
    );
}

export default ModalEpisodeSelector; 

function Episode({episode, onClick}){

    const ToTimeFormat = (runTime) => {
        const totalHours = runTime / 60;

        const hours = Math.trunc(totalHours);
        const minutes = Math.trunc((totalHours - hours) * 60);
    
        let timeFormat = '';
        if (hours > 0)
            timeFormat += hours + 'h ';
    
        if (minutes > 0)
            timeFormat += minutes + 'min';
    
        return timeFormat;
    }

    return <div class="episode-container" onClick={()=> onClick()}>
                <img src={episode.imagePath}/>
                <div>
                    <TitleAndContent title={episode.episodeNumber + '.' + episode.name} content={ToTimeFormat(episode.runTime)} justify="left" />
                    <SecondaryInfo text={episode.overview.substring(0, 300)}/>
                </div>
            </div>
}
