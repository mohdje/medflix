

import PlayArrowIcon from '@material-ui/icons/PlayArrow';
import PauseIcon from '@material-ui/icons/Pause';
import MoviePicturesPresentation from "../presentation/MoviePicturesPresentation";
import MoviesAPI from "../../../js/moviesAPI.js";
import { useEffect, useState } from 'react';
import CacheService from "../../../js/cacheService";
import "../../../style/movie-pictures-presentation.css";


function MoviesOfToday({ onClick, onDataLoaded }) {
    const [movies, setMovies] = useState([]);
    const [movieIndexVisible, setMovieIndexVisible] = useState(0);
    const [carouselPlay, setCarouselPlay] = useState(true);
    const cacheId = "moviesoftodaycache";

    useEffect(() => {

        const cache = CacheService.getCache(cacheId);
        if (cache) {
            setMovies(cache.data);
            onDataLoaded();
        }
        else {
            MoviesAPI.getMoviesOfToday(
                (movies) => {
                    if (movies && movies.length > 0) {
                        CacheService.setCache(cacheId, movies);
                        setMovies(movies);
                        onDataLoaded();
                    }
                })
        }

    }, []);

    useEffect(() => {
        var changeMovie;
        if (carouselPlay) {
            changeMovie = setTimeout(() => {
                if (movieIndexVisible === movies.length - 1)
                    setMovieIndexVisible(0)
                else setMovieIndexVisible(movieIndexVisible + 1);
            }, 5000);
        }

        return () => {
            clearTimeout(changeMovie);
        }
    }, [movieIndexVisible, carouselPlay]);


    return (
        <div className="movies-pictures-container">
            {movies.map((movie, index) =>
                <MoviePicturesPresentation
                    key={index}
                    movie={movie}
                    visible={index === movieIndexVisible}
                    onClick={() => onClick(movie.id)} />)}
            <div className="movies-pictures-nav-controls">
                <MoviePicturesCarousel
                    nbMovies={movies?.length}
                    selectedIndex={movieIndexVisible}
                    onNavElementClick={(index) => setMovieIndexVisible(index)} />
                <div className="movies-pictures-play-pause" onClick={() => setCarouselPlay(!carouselPlay)}>
                    <PlayArrowIcon className="icon" style={{ display: carouselPlay ? 'none' : '' }} />
                    <PauseIcon className="icon" style={{ display: !carouselPlay ? 'none' : '' }} />
                </div>
            </div>
        </div>
    );
}

export default MoviesOfToday;

function MoviePicturesCarousel({ nbMovies, selectedIndex, onNavElementClick }) {

    const navElements = [];

    for (let i = 0; i <= nbMovies - 1; i++)
        navElements.push(i);

    return (
        <div className="movies-pictures-nav">
            {navElements.map((i) =>
                <div key={i}
                    onClick={() => onNavElementClick(i)}
                    className={"nav-elem" + (selectedIndex === i ? ' selected' : '')}></div>)}
        </div>
    );
}