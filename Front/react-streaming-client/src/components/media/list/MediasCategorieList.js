import "../../../style/media-lite-presentation.css";
import "../../../style/medias-list.css";
import "../../../style/button.css";

import AddIcon from '@material-ui/icons/Add';
import CircularProgressBar from "../../common/CircularProgressBar";

import MediaLitePresentation from "../presentation/MediaLitePresentation";

import { useEffect, useState } from 'react';

const cache = {
    update(categorieId, medias, pageIndex, elemId) {
        this.categorieId = categorieId;
        this.medias = medias;
        this.pageIndex = pageIndex;
        this.mediaElementId = elemId;
    },
    get(categorieId) {
        return this.categorieId === categorieId ?
            {
                medias: this.medias,
                pageIndex: this.pageIndex,
                mediaElementId: this.mediaElementId
            }
            : null;
    },
    clear() {
        this.categorieId = '';
        this.medias = null;
    },
    categorieId: '',
    medias: null,
    pageIndex: 0,
    mediaElementId: ''
}

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


    useEffect(() => {
        const cacheMedias = cache.get(categorie.id);
        if (loadFromCache && cacheMedias) 
            setPageIndex(cacheMedias.pageIndex);
        else {           
            setMedias([]);
            setPageIndex(1); 
            if(pageIndex === 1) performSearch(); 
        }
    }, [categorie.id]);

    useEffect(() => {
        if (pageIndex > 0) {       
            const cacheMedias = cache.get(categorie.id);
            if (cacheMedias) setMedias(cacheMedias.medias);
            else performSearch();
        }
    }, [pageIndex]);

    useEffect(() => {
        if (medias && medias.length > 0) {
            if (loadFromCache) {
                const cacheMedias = cache.get(categorie.id);
                if (cacheMedias) {
                    scrollToMedia(cacheMedias.mediaElementId);
                    cache.clear();
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
        cache.update(categorie.id, medias, pageIndex, elementId);
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