
import "../style/home-page.css";
import MediasOfToday from "../components/movies/list/MediasOfTodayList";
import MediasListLiteWithTitle from "../components/movies/list/MediasListLiteWithTitle";
import CircularProgressBar from "../components/common/CircularProgressBar";

import fadeTransition from "../js/customStyles.js";
import MoviesAPI from "../js/moviesAPI.js";
import CacheService from "../js/cacheService";
import AppMode from "../js/appMode";

import { useState, useEffect, useRef } from 'react';

function HomePage({ onMediaClick, onReady, onFail }) {
    const [dataLoaded, setDataLoaded] = useState(false);

    const mediaListref = useRef([]);
    const cacheIdRef = useRef("");

    const [mediaLists, setMediaLists] = useState([]);

    const buildCacheId = (appMode) => {
        return 'homepage' + appMode;
    }

    const todayTrendingTitle = "todayTrending"

    const addMedias = (listTitle, medias, insertAtBeginning)=> {
        if(medias && medias.length > 0){ 
            let data = {
                title: listTitle,
                medias: medias
            }
            if(insertAtBeginning)mediaListref.current.unshift(data);
            else mediaListref.current.push(data);
            CacheService.setCache(cacheIdRef.current, mediaListref.current);
            const newArray = [...mediaListref.current];
            setMediaLists(newArray);
        }
    };

    const loadContent = () => {
        cacheIdRef.current = buildCacheId(AppMode.getActiveMode().label);

        let cache = CacheService.getCache(cacheIdRef.current);
        mediaListref.current = [];

        if(cache){
            mediaListref.current = cache.data;
            setMediaLists(mediaListref.current);
            setDataLoaded(true);
        }
        else{
            MoviesAPI.getMoviesOfToday(
            (medias) => {
                addMedias(todayTrendingTitle, medias);
                setDataLoaded(true);
                onReady();
            });
            MoviesAPI.getPopularMovies((medias)=>{
                addMedias("Popular " + AppMode.getActiveMode().label.toLocaleLowerCase(), medias, true);
            }, ()=>{
                onFail();
            });
            MoviesAPI.getRecommandedMovies(null, (medias)=>{
                addMedias("Recommanded for you", medias, true);
            });
            MoviesAPI.getPopularNetflixMovies((medias)=>{
                addMedias("Popular on Netflix", medias);
            });
            MoviesAPI.getPopularDisneyPlusMovies((medias)=>{
                addMedias("Popular on Disney Plus", medias);
            });
            MoviesAPI.getPopularAmazonPrimeMovies((medias)=>{
                addMedias("Popular on Amazon Prime", medias);
            });
        }
    }

    useEffect(()=>{
        loadContent();
        AppMode.onAppModeChanged(()=>{
            loadContent();
        });
    },[]);

    return (
        <div style={{height: '100%'}}>
            <CircularProgressBar position={"center"} visible={!dataLoaded} color={"white"} size={"60px"}/>
            <div className="home-page-container" style={fadeTransition(dataLoaded)}>
                <MediasOfToday medias={mediaLists.find(list => list.title === todayTrendingTitle)?.medias} onClick={(mediaId) => onMediaClick(mediaId)}/>
                <div className="blur-divider"></div>    
                 {mediaLists.filter(list => list.title !== todayTrendingTitle).map((mediasList) =>
                (
                    <MediasListLiteWithTitle
                        key={mediasList.title}
                        visible={true}
                        listTitle={mediasList.title}
                        medias={mediasList.medias}
                        onMediaClick={(mediaId) => onMediaClick(mediaId)} />
                ))}    
            </div>
        </div>
    )
}

export default HomePage;