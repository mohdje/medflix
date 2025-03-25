
import "../style/css/home-page.css";
import "../style/css/animations.css";
import MediasCarousel from "../components/media/list/MediasCarouselList";
import MediasHorizontalList from "../components/media/list/MediasHorizontalList";
import CircularProgressBar from "../components/common/CircularProgressBar";

import { mediasInfoApi } from "../services/api";
import { setCache, getCache } from "../services/cacheService";
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
            setCache(cacheIdRef.current, mediaListref.current);
            const newArray = [...mediaListref.current];
            setMediaLists(newArray);
        }
    };

    const loadContent = async () => {
        cacheIdRef.current = buildCacheId(AppMode.getActiveMode().label);

        let cacheData = getCache(cacheIdRef.current);
        mediaListref.current = [];

        if (cacheData) {
            mediaListref.current = cacheData;
            setMediaLists(mediaListref.current);
            setDataLoaded(true);
        }
        else {
            await Promise.all([
                mediasInfoApi.getMediasOfToday().then((medias) => addMedias(todayTrendingTitle, medias)),
                mediasInfoApi.getPopularMedias().then((medias) => addMedias("Popular " + AppMode.getActiveMode().label.toLocaleLowerCase(), medias, true)),
                mediasInfoApi.getRecommandedMedias().then((medias) => addMedias("Recommanded for you", medias, true)),
                mediasInfoApi.getPopularNetflixMedias().then((medias) => addMedias("Popular on Netflix", medias)),
                mediasInfoApi.getPopularDisneyPlusMedias().then((medias) => addMedias("Popular on Disney Plus", medias)),
                mediasInfoApi.getPopularAmazonPrimeMedias().then((medias) => addMedias("Popular on Amazon Prime", medias)),
                mediasInfoApi.getPopularAppleTvMedias().then((medias) => addMedias("Popular on AppleTv", medias)),
            ]);
            setDataLoaded(true);

            if (mediaListref.current?.length > 0)
                onReady();
            else
                onFail();
        }
    }

    useEffect(() => {
        loadContent();
        AppMode.onAppModeChanged(() => {
            loadContent();
        });
    }, []);

    return (
        <>
            <CircularProgressBar position={"center"} visible={!dataLoaded} size="x-large" />
            {
                dataLoaded ? <div className="home-page-container fade-in">
                    <MediasCarousel medias={mediaLists.find(list => list.title === todayTrendingTitle)?.medias} onClick={(mediaId) => onMediaClick(mediaId)} />
                    <div className="blur-divider"></div>
                    {mediaLists.filter(list => list.title !== todayTrendingTitle).map((mediasList) =>
                    (
                        <MediasHorizontalList
                            key={mediasList.title}
                            title={mediasList.title}
                            medias={mediasList.medias}
                            onMediaClick={(media) => onMediaClick(media)} />
                    ))}
                </div> : null
            }
        </>
    )
}

export default HomePage;