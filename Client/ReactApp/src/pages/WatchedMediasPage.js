import { watchHistoryApi } from "../services/api";
import MediasVerticalList from "../components/media/list/MediasVerticalList";
import { useEffect, useState, useReducer } from 'react';
import AppMode from "../services/appMode.js";

export default function WatchedMediasPage({ onMediaClick }) {
    const [watchedMedias, setWatchedMedias] = useState([]);
    const [loadingVisible, setLoadingVisible] = useState(true);

    const titleReducer = (state, mediasLength) => {
        const mediaType = AppMode.getActiveMode().label;
        if (mediasLength > 0)
            return `${mediaType} watch history`;
        else
            return `No ${mediaType.toLowerCase()} wacthed yet`;
    };
    const [title, titleDispatch] = useReducer(titleReducer, '');

    useEffect(() => {
        const loadWatchList = async () => {
            const watchedMedias = await watchHistoryApi.getWatchedMedias();
            setLoadingVisible(false);
            if (watchedMedias?.length > 0) {
                watchedMedias.forEach(watchedMedia => {
                    watchedMedia.media.progress = (watchedMedia.currentTime / watchedMedia.totalDuration) * 100;
                });
                setWatchedMedias(watchedMedias);
            }
            titleDispatch(watchedMedias?.length);
        };
        loadWatchList();
    }, []);

    return (
        <MediasVerticalList
            title={title}
            loadingProgressVisible={loadingVisible}
            medias={watchedMedias.map(watchedMedias => watchedMedias.media)}
            onMediaClick={(media) => onMediaClick(media)} />
    )
}