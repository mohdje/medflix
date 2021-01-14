

import PlayArrowIcon from '@material-ui/icons/PlayArrow';
import PauseIcon from '@material-ui/icons/Pause';
import SuggestedMovie from "./SuggestedMovie";
import MoviesAPI from "../js/moviesAPI.js";
import { useEffect, useState } from 'react';

function SuggestedMovies({ onMoreClick, onDataLoaded }) {
    const [movies, setMovies] = useState([]);
    const [movieIndexVisible, setMovieIndexVisible] = useState(0);
    const [suggestedMoviesPlay, setSuggestedMoviesPlay] = useState(true);

    useEffect(() => {
        MoviesAPI.getSuggestedMovies(
            (suggestedMovies) => {
                if (suggestedMovies && suggestedMovies.length > 0) {
                    setMovies(suggestedMovies);
                    onDataLoaded();
                }
            },
            () => console.log('fail'));
    }, []);

    useEffect(() => {
        var changeMovie;
        if (suggestedMoviesPlay) {
            changeMovie = setTimeout(() => {
                if (movieIndexVisible === movies.length - 1)
                    setMovieIndexVisible(0)
                else setMovieIndexVisible(movieIndexVisible + 1);
            }, 5000);
        }

        return () => {
            clearTimeout(changeMovie);
        }
    }, [movieIndexVisible, suggestedMoviesPlay]);


    return (
        <div className="suggested-movies-container">
            <SuggestMovieNavigation 
                nbMovies={movies?.length} 
                selectedIndex={movieIndexVisible}
                onNavElementClick={(index)=>setMovieIndexVisible(index)}/>
            <div className="suggested-movies-play-pause" onClick={() => setSuggestedMoviesPlay(!suggestedMoviesPlay)}>
                <PlayArrowIcon className="icon" style={{ display: suggestedMoviesPlay ? 'none' : '' }} />
                <PauseIcon className="icon" style={{ display: !suggestedMoviesPlay ? 'none' : '' }} />
            </div>
            {movies.map((movie, index) =>
                <SuggestedMovie
                    key={index}
                    movie={movie}
                    visible={index === movieIndexVisible}
                    onMoreClick={() => onMoreClick(movie.id)} />)}
        </div>
    );
}

export default SuggestedMovies;

function SuggestMovieNavigation({ nbMovies, selectedIndex, onNavElementClick }) {

    const navElements = [];

    for (let i = 0; i <= nbMovies -1; i++)  
        navElements.push(i);

    return (
        <div className="suggested-movies-nav">
            {navElements.map((i)=>
             <div   key={i} 
                    onClick={()=> onNavElementClick(i)}
                    className={"nav-elem" + (selectedIndex === i ? ' selected' : '')}></div> )}
        </div>
    );
}