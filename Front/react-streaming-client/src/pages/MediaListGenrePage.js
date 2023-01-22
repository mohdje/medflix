import "../style/media-lite-presentation.css";
import "../style/medias-list.css";
import "../style/button.css";

import AddIcon from '@material-ui/icons/Add';
import CircularProgressBar from "../components/common/CircularProgressBar";

import MediaLitePresentation from "../components/media/presentation/MediaLitePresentation";
import AppServices from "../services/AppServices";

import { useEffect, useState } from 'react';

const cache = {
    update(genreId, medias, pageIndex, elemId) {
        this.genreId = genreId;
        this.medias = medias;
        this.pageIndex = pageIndex;
        this.mediaElementId = elemId;
    },
    get(genreId) {
        return this.genreId === genreId ?
            {
                medias: this.medias,
                pageIndex: this.pageIndex,
                mediaElementId: this.mediaElementId
            }
            : null;
    },
    clear() {
        this.genreId = '';
        this.medias = null;
    },
    genreId: '',
    medias: null,
    pageIndex: 0,
    mediaElementId: ''
}

function MediasListGenrePage({ genre, loadFromCache, onMediaClick }) {

    const [medias, setMedias] = useState([]);
    const [pageIndex, setPageIndex] = useState(0);
    const [searchInProgress, setSearchInProgress] = useState(false);

    const addIconStyle = {
        width: '60px',
        height: '60px',
        color: 'white',
        margin: 'auto'
    };

    const performSearch = () => {
        setSearchInProgress(true);
        AppServices.searchMediaService.getMediasByGenre(genre.id, pageIndex,
            (mediasOfGenre) => {
                setSearchInProgress(false);
                if (mediasOfGenre && mediasOfGenre.length > 0) {
                    if (pageIndex === 1) setMedias(mediasOfGenre);
                    else setMedias(medias.concat(mediasOfGenre));
                }
            },
            () => setSearchInProgress(false));
    }


    useEffect(() => {
        const cacheMedias = cache.get(genre.id);
        if (loadFromCache && cacheMedias) 
            setPageIndex(cacheMedias.pageIndex);
        else {           
            setPageIndex(1); 
            if(pageIndex === 1) performSearch(); 
        }
    }, [genre.id]);

    useEffect(() => {
        if (pageIndex > 0) {       
            const cacheMedias = cache.get(genre.id);
            if (cacheMedias) setMedias(cacheMedias.medias);
            else performSearch();
        }
    }, [pageIndex]);

    useEffect(() => {
        if (medias && medias.length > 0) {
            if (loadFromCache) {
                const cacheMedias = cache.get(genre.id);
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
        cache.update(genre.id, medias, pageIndex, elementId);
        onMediaClick(mediaId);
    }

    return (
        <div className="medias-list-by-genre-container">
            <div className="medias-list-by-genre-header">
                <div className="medias-list-genre">{genre.name}</div>
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
                            style={{ visibility: !searchInProgress ? 'visible' : 'hidden' }}
                            onClick={() => { if (!searchInProgress) setPageIndex(pageIndex + 1) }}>
                            <AddIcon style={addIconStyle} />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default MediasListGenrePage;