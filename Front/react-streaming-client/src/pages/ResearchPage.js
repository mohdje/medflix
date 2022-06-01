import TextField from '@material-ui/core/TextField';
import ClearIcon from '@material-ui/icons/Clear';
import "../style/search-view.css";

import MoviesListLite from "../components/movies/list/MoviesListLite";
import CircularProgressBar from "../components/common/CircularProgressBar";
import MoviesAPI from "../js/moviesAPI.js";
import fadeTransition from "../js/customStyles.js";

import { useEffect, useState, useRef, useReducer } from 'react';

const cacheResearch = {
    searchValue: '',
    result: [],
    save(searchValue, movies){
        this.searchValue = searchValue;
        this.result = movies;
    },
    clean(){
        this.searchValue = '';
        this.result = [];
    }
}; 

function ResearchPage({ loadFromCache, onMovieClick }) {
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
        if(loadFromCache && searchValue === cacheResearch.searchValue)
            return;

        searchValueRef.current = searchValue;
        if (searchValue && searchValue.length > 2) {
            setTimeout(() => {
                if (searchValue === searchValueRef.current) {
                    setSearchInProgress(true);
                    searchResultLabelDispatch(-1);                
                    setMovies([]);
                    cacheResearch.clean();
                    MoviesAPI.searchMovies(searchValue,
                        (movies) => {
                            setSearchInProgress(false);
                            if(searchValueRef.current && searchValue === searchValueRef.current){
                                searchResultLabelDispatch(movies.length);
                                if (movies && movies.length > 0){
                                    setMovies(movies);
                                    cacheResearch.save(searchValue, movies);
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
            setMovies([]);
        }
    }, [searchValue]);

    useEffect(() => {
        if(loadFromCache){
            setMovies(cacheResearch.result);
            searchResultLabelDispatch(cacheResearch.result.length);
            setSearchValue(cacheResearch.searchValue);
        }
    }, []);

    return (
        <div className="search-view-container">
            <div className="search-field-container">
                <div className="search-field">
                    <CircularProgressBar color={'white'} size={'40px'} visible={searchInProgress} />
                    <TextField value={searchValue} onChange={(e) => { setSearchValue(e.target.value) }} placeholder="Enter a movie name" />
                    <ClearIcon style={fadeTransition(searchValue)} className='delete-text-cross' onClick={() => { setSearchValue('') }} />
                </div>
            </div>
            <div className="movies-search-result">
                <MoviesListLite movies={movies} onMovieClick={(movieId) => onMovieClick(movieId)} />
                <div className="label-result">{searchResultLabel}</div>
            </div>
        </div>
    );
}

export default ResearchPage;