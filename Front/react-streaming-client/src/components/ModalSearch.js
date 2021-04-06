import TextField from '@material-ui/core/TextField';
import ClearIcon from '@material-ui/icons/Clear';
import "../style/search-view.css";

import ModalWindow from "./ModalWindow";
import MoviesListLite from "./MoviesListLite";
import CircularProgressBar from "./CircularProgressBar";
import MoviesAPI from "../js/moviesAPI.js";
import fadeTransition from "../js/customStyles.js";

import { useEffect, useState, useRef, useReducer } from 'react';

function ModalSearch({ visible, onCloseClick, onMovieClick }) {
    const [searchValue, setSearchValue] = useState('');
    const [movies, setMovies] = useState([]);
    const [searchInProgress, setSearchInProgress] = useState(false);

    const searchValueRef = useRef(searchValue);

    const searchResultLabelReducer = (state, moviesLength) => {
        if (moviesLength === 0) return "No result found";
        else if (moviesLength === 1) return "Only one movie found";
        else if (moviesLength > 1) return moviesLength + " movies found";
        else return "";
    };
    const [searchResultLabel, searchResultLabelDispatch] = useReducer(searchResultLabelReducer, '');

    useEffect(() => {
        if (!visible) setSearchValue('');
    }, [visible]);

    useEffect(() => {
        if (searchValue && searchValue.length > 2) {
            searchValueRef.current = searchValue;
            setTimeout(() => {
                if (searchValue === searchValueRef.current) {
                    setSearchInProgress(true);
                    searchResultLabelDispatch(-1);
                    setMovies([]);
                    MoviesAPI.searchMovies(searchValue,
                        (movies) => {
                            setSearchInProgress(false);
                            console.log("movies.length", movies.length);
                            searchResultLabelDispatch(movies.length);
                            if (movies && movies.length > 0) setMovies(movies);
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
            setMovies([]);
        }
    }, [searchValue]);

    const searchView = () => {
        return (
            <div className="search-view-container">
                <div className="search-field-container">
                    <div className="search-field">
                        <CircularProgressBar color={'white'} size={'40px'} visible={searchInProgress} />
                        <TextField value={searchValue} onChange={(e) => { setSearchValue(e.target.value) }} disabled={searchInProgress} placeholder="Enter a movie name" />
                        <ClearIcon style={fadeTransition(searchValue && !searchInProgress)} className='delete-text-cross' onClick={() => { setSearchValue('') }} />
                    </div>
                </div>
                <div className="movies-search-result">
                    <MoviesListLite movies={movies} onMovieClick={(movieId) => onMovieClick(movieId)} />
                    <div className="label-result">{searchResultLabel}</div>
                </div>
            </div>
        );
    }

    return (
        <ModalWindow visible={visible} content={searchView()} onCloseClick={() => onCloseClick()} />
    );
}

export default ModalSearch;