import "../../../style/media-lite-presentation.css";
import "../../../style/medias-list.css";
import "../../../style/button.css";

import AddIcon from '@material-ui/icons/Add';
import CircularProgressBar from "../../common/CircularProgressBar";

import MediaLitePresentation from "../presentation/MediaLitePresentation";

import CacheService from "../../../services/cacheService";

import { useEffect, useState } from 'react';

function MediasCategorieList({ categorie, searchOperation, loadFromCache, onMediaClick }) {

    const [medias, setMedias] = useState([]);
    const [pageIndex, setPageIndex] = useState(0);
    const [searchInProgress, setSearchInProgress] = useState(false);
    const [plusButtonVisible, setPlusButtonVisible] = useState(false);
    
    const addIconStyle = {
        width: '60px',
        height: '60px',
        color: 'white',
        margin: 'auto'
    };

    const performSearch = () => {
        setSearchInProgress(true);
        setPlusButtonVisible(false); 
        searchOperation(categorie.id, pageIndex,
            (mediasOfCategories) => {
                setSearchInProgress(false);
                if (mediasOfCategories && mediasOfCategories.length > 0) {
                    setPlusButtonVisible(true);
                    if (pageIndex === 1) setMedias(mediasOfCategories);
                    else setMedias(medias.concat(mediasOfCategories));
                }
            },
            () => setSearchInProgress(false));
    }

    const getCacheId = (categorieId) => "categorie" + categorieId;

    useEffect(() => {
        if (loadFromCache) {
            const cacheMedias = CacheService.getCache(getCacheId(categorie.id));
            setPageIndex(cacheMedias.data.pageIndex);
        }
        else {           
            CacheService.setCache(getCacheId(categorie.id), null);
            setMedias([]);
            setPageIndex(1); 
            if(pageIndex === 1) performSearch(); 
        }
    }, [categorie.id]);

    useEffect(() => {
        if (pageIndex > 0) {  
            const cacheMedias = CacheService.getCache(getCacheId(categorie.id));
            if (cacheMedias && cacheMedias.data) setMedias(cacheMedias.data.medias);
            else performSearch();
        }
    }, [pageIndex]);

    useEffect(() => {
        if (medias && medias.length > 0) {
            if (loadFromCache) {
                const cacheMedias = CacheService.getCache(getCacheId(categorie.id));
                if (cacheMedias && cacheMedias.data) {
                    setPlusButtonVisible(true);
                    scrollToMedia(cacheMedias.data.mediaElementId);
                    CacheService.setCache(getCacheId(categorie.id), null);
                }
            }
            else if(pageIndex === 1)  scrollToMedia("medialite0");
        }
    }, [medias]);

    const scrollToMedia = (elemId) => {
        var elem = document.getElementById(elemId);
        if (elem) {
            elem.scrollIntoView({
                block: "nearest",
                inline: "nearest"
            });
        }
    }
 
    const handleMediaClick = (mediaId, elementId) => {
        const data = {
            medias: medias,
            pageIndex: pageIndex,
            mediaElementId: elementId
        }
        CacheService.setCache(getCacheId(categorie.id), data);
        onMediaClick(mediaId);
    }

    return (
        <div className="medias-list-by-categorie-container">
            <div className="medias-list-by-categorie-header">
                <div className="medias-list-categorie">{categorie.name}</div>
                <CircularProgressBar size={'30px'} color={'white'} visible={searchInProgress} />
            </div>
            <div className="medias-list-container full">
                <div className="medias-list wrap-content">
                    {medias.map((media, index) =>
                    (<div id={"medialite" + index} key={index}>
                        <MediaLitePresentation media={media} hoverEffect={true} onMediaClick={(mediaId) => handleMediaClick(mediaId, "medialite" + index)} />
                    </div>))}
                    <div className="medias-list-more">
                        <div className="round-btn grey"
                            style={{ visibility: plusButtonVisible ? 'visible' : 'hidden' }}
                            onClick={() => { if (plusButtonVisible) {setPageIndex(pageIndex + 1);} }}>
                            <AddIcon style={addIconStyle} />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default MediasCategorieList;