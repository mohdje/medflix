import "../style/search-view.css";

import MediasListLite from "../components/movies/list/MediasListLite";
import CircularProgressBar from "../components/common/CircularProgressBar";
import TextInput from '../components/common/TextInput';
import AppServices from "../services/AppServices";

import { useEffect, useState, useRef, useReducer } from 'react';

const cacheResearch = {
    searchValue: '',
    result: [],
    save(searchValue, medias){
        this.searchValue = searchValue;
        this.result = medias;
    },
    clean(){
        this.searchValue = '';
        this.result = [];
    }
}; 

function ResearchPage({ loadFromCache, onMediaClick }) {
    const [searchValue, setSearchValue] = useState('');
    const [medias, setMedias] = useState([]);
    const [searchInProgress, setSearchInProgress] = useState(false);

    const searchValueRef = useRef(searchValue);

    const searchResultLabelReducer = (state, mediasLength) => {
        if (mediasLength === 0) return "Nothing found";
        else if (mediasLength === 1) return "one result";
        else if (mediasLength > 1) return mediasLength + " results";
        else return "";
    };
    const [searchResultLabel, searchResultLabelDispatch] = useReducer(searchResultLabelReducer, '');

    useEffect(() => {
        if(loadFromCache && searchValue === cacheResearch.searchValue)
            return;

        searchValueRef.current = searchValue;
        if (searchValue && searchValue.length > 2) {
            setTimeout(() => {
                if (searchValue === searchValueRef.current) {
                    setSearchInProgress(true);
                    searchResultLabelDispatch(-1);                
                    setMedias([]);
                    cacheResearch.clean();
                    AppServices.searchMediaService.searchMedias(searchValue,
                        (medias) => {
                            setSearchInProgress(false);
                            if(searchValueRef.current && searchValue === searchValueRef.current){
                                searchResultLabelDispatch(medias.length);
                                if (medias && medias.length > 0){
                                    setMedias(medias);
                                    cacheResearch.save(searchValue, medias);
                                } 
                            }
                        },
                        () => {
                            setSearchInProgress(false);
                            searchResultLabelDispatch(0);
                        });
                }
            }, 1000);
        }
        else {
            searchResultLabelDispatch(-1);
            setMedias([]);
        }
    }, [searchValue]);

    useEffect(() => {
        if(loadFromCache){
            setMedias(cacheResearch.result);
            searchResultLabelDispatch(cacheResearch.result.length);
            setSearchValue(cacheResearch.searchValue);
        }
    }, []);

    return (
        <div className="search-view-container">    
            <TextInput placeHolder="Type here to start search..." onTextChanged={(text) => { setSearchValue(text) }} />
            <CircularProgressBar color={'white'} size={'40px'} visible={searchInProgress} />
            <div className="medias-search-result">
                <MediasListLite medias={medias} onMediaClick={(mediaId) => onMediaClick(mediaId)} />
                <div className="label-result">{searchResultLabel}</div>
            </div>
        </div>
    );
}

export default ResearchPage;