import { bookmarkApi } from "../services/api";
import AppMode from "../services/appMode.js";
import MediasVerticalList from "../components/media/list/MediasVerticalList";
import { useEffect, useState, useReducer } from 'react';
import { addEvent, eventsNames, removeEvent } from "../helpers/eventHelper.js";

export default function BookmarkedMediasPage({ onMediaClick }) {

    const [mediasBookmarks, setMediasBookmarks] = useState([]);
    const [loadingVisible, setLoadingVisible] = useState(true);

    const titleReducer = (state, mediasLength) => {
        const mediaType = AppMode.getActiveMode().label;
        if (mediasLength > 0)
            return `Bookmarked ${mediaType}`;
        else
            return `No bookmarked ${mediaType}`;
    };
    const [title, titleDispatch] = useReducer(titleReducer, '');

    useEffect(() => {

        const loadList = async () => {
            const medias = await bookmarkApi.getBookmarkedMedias();
            setLoadingVisible(false);
            if (medias?.length > 0)
                setMediasBookmarks(medias);

            titleDispatch(medias?.length);
        }
        loadList();
        addEvent(eventsNames.bookmarkUpdated, loadList);

        return () => {
            removeEvent(eventsNames.bookmarkUpdated, loadList)
        }
    }, []);

    return (
        <MediasVerticalList
            title={title}
            loadingProgressVisible={loadingVisible}
            medias={mediasBookmarks}
            onMediaClick={(media) => onMediaClick(media)} />
    )
}