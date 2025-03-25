import "../style/css/search-view.css";

import MediasHorizontalList from "../components/media/list/MediasHorizontalList";
import MediasVerticalList from "../components/media/list/MediasVerticalList";

import CircularProgressBar from "../components/common/CircularProgressBar";
import TextInput from '../components/common/TextInput';
import { mediasInfoApi } from "../services/api";
import { useEffect, useState, useRef, useReducer } from 'react';

export default function ResearchPage({ onMediaClick }) {
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

    const searchMedias = async (text) => {
        searchResultLabelDispatch(-1);
        setMedias([]);
        setSearchInProgress(true);
        const medias = await mediasInfoApi.searchMedias(text);
        setSearchInProgress(false);

        if (searchValueRef.current && text === searchValueRef.current) {
            searchResultLabelDispatch(medias.length);
            if (medias && medias.length > 0)
                setMedias(medias);
            else
                searchResultLabelDispatch(0);
        }
    }
    useEffect(() => {
        searchValueRef.current = searchValue;
        if (searchValue && searchValue.length > 2) {
            setTimeout(() => {
                if (searchValue === searchValueRef.current) {
                    searchMedias(searchValue);
                }
            }, 1000);
        }
        else {
            searchResultLabelDispatch(-1);
            setMedias([]);
        }
    }, [searchValue]);

    return (
        <div className="search-view-container">
            <div className="search-input-container">
                <TextInput placeHolder="Type here to start search..." onTextChanged={(text) => { setSearchValue(text) }} />
            </div>
            <CircularProgressBar size={searchInProgress ? 'medium' : 'none'} visible={searchInProgress} />
            <div className="desktop-search-result">
                <MediasHorizontalList title={searchResultLabel} centerTitle medias={medias} onMediaClick={(media) => onMediaClick(media)} />
            </div>
            <div className="mobile-search-result">
                <MediasVerticalList title={searchResultLabel} medias={medias} onMediaClick={(media) => onMediaClick(media)} />
            </div>
        </div>
    );
}