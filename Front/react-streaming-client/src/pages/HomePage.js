
import "../style/home-page.css";
import MediasOfToday from "../components/media/list/MediasOfTodayList";
import MediasListLiteWithTitle from "../components/media/list/MediasListLiteWithTitle";
import CircularProgressBar from "../components/common/CircularProgressBar";

import fadeTransition from "../helpers/customStyles.js";
import AppServices from "../services/AppServices";
import CacheService from "../services/cacheService";
import AppMode from "../services/appMode";

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

    const addMedias = (listTitle, medias, insertAtBeginning) => {
        if (medias && medias.length > 0) {
            let data = {
                title: listTitle,
                medias: medias
            }
            if (insertAtBeginning) mediaListref.current.unshift(data);
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

        if (cache) {
            mediaListref.current = cache.data;
            setMediaLists(mediaListref.current);
            setDataLoaded(true);
        }
        else {
            AppServices.searchMediaService.getMediasOfToday(
                (medias) => {
                    addMedias(todayTrendingTitle, medias);
                    setDataLoaded(true);
                    onReady();
                }, () => {
                    onFail();
                });
            AppServices.searchMediaService.getPopularMedias((medias) => {
                addMedias("Popular " + AppMode.getActiveMode().label.toLocaleLowerCase(), medias, true);
            });
            AppServices.searchMediaService.getRecommandedMedias((medias) => {
                addMedias("Recommanded for you", medias, true);
            });
            AppServices.searchMediaService.getPopularNetflixMedias((medias) => {
                addMedias("Popular on Netflix", medias);
            });
            AppServices.searchMediaService.getPopularDisneyPlusMedias((medias) => {
                addMedias("Popular on Disney Plus", medias);
            });
            AppServices.searchMediaService.getPopularAmazonPrimeMedias((medias) => {
                addMedias("Popular on Amazon Prime", medias);
            });
            AppServices.searchMediaService.getPopularAppleTvMedias((medias) => {
                addMedias("Popular on AppleTv", medias);
            });
        }
    }

    useEffect(() => {
        loadContent();
        AppMode.onAppModeChanged(() => {
            loadContent();
        });
    }, []);

    return (
        <div style={{ height: '100%' }}>
            <CircularProgressBar position={"center"} visible={!dataLoaded} color={"white"} size={"60px"} />
            <div className="home-page-container" style={fadeTransition(dataLoaded)}>
                <MediasOfToday medias={mediaLists.find(list => list.title === todayTrendingTitle)?.medias} onClick={(mediaId) => onMediaClick(mediaId)} />
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