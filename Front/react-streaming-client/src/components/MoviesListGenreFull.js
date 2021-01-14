import "../style/movie-lite-presentation.css";
import "../style/movies-list.css";
import "../style/button.css";

import AddIcon from '@material-ui/icons/Add';
import CircularProgressBar from "./CircularProgressBar";

import MovieLitePresentation from "./MovieLitePresentation";
import MoviesAPI from "../js/moviesAPI.js";

import { useEffect, useState } from 'react';

const cache = {
    update(genre, movies, pageIndex, elemId) {
        this.genre = genre;
        this.movies = movies;
        this.pageIndex = pageIndex;
        this.movieElementId = elemId;
    },
    get(genre) {
        return this.genre === genre ?
            {
                movies: this.movies,
                pageIndex: this.pageIndex,
                movieElementId: this.movieElementId
            }
            : null;
    },
    clear() {
        this.genre = '';
        this.movies = null;
    },
    genre: '',
    movies: null,
    pageIndex: 0,
    movieElementId: ''
}

function MoviesListGenreFull({ genre, loadFromCache, onMovieClick }) {

    const [movies, setMovies] = useState([]);
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
        MoviesAPI.getMoviesByGenre(genre, pageIndex,
            (moviesOfGenre) => {
                setSearchInProgress(false);
                if (moviesOfGenre && moviesOfGenre.length > 0) {
                    if (pageIndex === 1) setMovies(moviesOfGenre);
                    else setMovies(movies.concat(moviesOfGenre));
                }
            },
            () => setSearchInProgress(false));
    }


    useEffect(() => {
        if (loadFromCache) {
            const cacheMovies = cache.get(genre);
            setPageIndex(cacheMovies.pageIndex);
        }
        else {           
            setPageIndex(1); 
            if(pageIndex === 1) performSearch(); 
        }
    }, [genre]);

    useEffect(() => {
        if (pageIndex > 0) {       
            const cacheMovies = cache.get(genre);
            if (cacheMovies) setMovies(cacheMovies.movies);
            else performSearch();
        }
    }, [pageIndex]);

    useEffect(() => {
        if (movies && movies.length > 0) {
            if (loadFromCache) {
                const cacheMovies = cache.get(genre);
                if (cacheMovies) {
                    scrollToMovie(cacheMovies.movieElementId);
                    cache.clear();
                }
            }
            else if(pageIndex === 1)  scrollToMovie("movielite0");
        }
    }, [movies]);

    const scrollToMovie = (elemId) => {
        var elem = document.getElementById(elemId);
        if (elem) {
            elem.scrollIntoView({
                block: "nearest",
                inline: "nearest"
            });
        }
    }

    const handleMovieClick = (movieId, elementId) => {
        cache.update(genre, movies, pageIndex, elementId);
        onMovieClick(movieId);
    }

    return (
        <div className="movies-list-by-genre-container">
            <div className="movies-list-by-genre-header">
                <div className="movies-list-genre">{genre}</div>
                <CircularProgressBar size={'30px'} color={'white'} visible={searchInProgress} />
            </div>
            <div className="movies-list-container full">
                <div className="movies-list wrap-content">
                    {movies.map((movie, index) =>
                    (<div id={"movielite" + index} key={index}>
                        <MovieLitePresentation movie={movie} onMovieClick={(movieId) => handleMovieClick(movieId, "movielite" + index)} />
                    </div>))}
                    <div className="movies-list-more">
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

export default MoviesListGenreFull;