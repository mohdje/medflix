
import MediasVerticalList from "../components/media/list/MediasVerticalList";
import { mediasInfoApi } from "../services/api";
import AppMode from "../services/appMode";
import { useEffect, useState } from 'react';

export default function MediaListOfCategoriePage({ categorie, type, onMediaClick }) {
    const [medias, setMedias] = useState([]);
    const [pageIndex, setPageIndex] = useState(1);

    const [searchInProgress, setSearchInProgress] = useState(false);
    const [moreButtonVisible, setMoreButtonVisible] = useState(false);

    useEffect(() => {
        setMedias([]);
        setPageIndex(1);
    }, [categorie]);

    useEffect(() => {
        fetchMediasForPage(categorie.id, pageIndex);
    }, [pageIndex])

    const fetchMediasForPage = async (categorieId, pageIndex) => {
        setSearchInProgress(true);
        setMoreButtonVisible(false);

        let mediasOfCategory;
        if (type === "genre")
            mediasOfCategory = await mediasInfoApi.getMediasByGenre(categorieId, pageIndex);
        else if (type === "platform")
            mediasOfCategory = await mediasInfoApi.getMediasByPlatform(categorieId, pageIndex);

        setSearchInProgress(false);
        if (mediasOfCategory && mediasOfCategory.length > 0) {
            setMoreButtonVisible(true);
            setMedias(medias.concat(mediasOfCategory));
        }
    };

    return (
        <MediasVerticalList
            title={`${categorie.name} ${AppMode.getActiveMode().label.toLowerCase()}`}
            onMediaClick={(media) => onMediaClick(media)}
            moreButtonVisible={moreButtonVisible}
            loadingProgressVisible={searchInProgress}
            onMoreButtonClick={() => setPageIndex(pageIndex + 1)}
            medias={medias} />
    );
}
