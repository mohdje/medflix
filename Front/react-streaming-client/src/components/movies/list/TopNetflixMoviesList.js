import "../../../style/movies-list.css";
import MoviesAPI from "../../../js/moviesAPI.js";
import MoviesListLite from "./MoviesListLite";
import { useEffect, useState } from 'react';
import fadeTransition from "../../../js/customStyles.js";
import CacheService from "../../../js/cacheService";


function TopNetflixMovies({ onMovieClick }) {
    const [movies, setMovies] = useState([]);
    const cacheId = "topnetflixcache";

    useEffect(() => {
        let cache = CacheService.getCache(cacheId);
        if(cache){
            setMovies(cache.data);
        }
        else{
            MoviesAPI.getTopNetflixMovies(
                (movies) => {
                    if (movies && movies.length > 0){
                        setMovies(movies);
                        CacheService.setCache(cacheId , movies);
                    } 
                });
        }
    }, []);

    return (
        <div className="movies-list-genre-container">
            <div className="movies-list-header">
                <div className="movies-list-genre">Top Netflix movies</div>
            </div>
            <LoadingMovies visible={!movies || movies.length === 0} />
            <div style={fadeTransition(movies && movies.length > 0, 1)} >
                <MoviesListLite movies={movies} onMovieClick={(movieId)=>onMovieClick(movieId)}/>
            </div>
        </div>
    );
}
export default TopNetflixMovies;

function LoadingMovies({visible}) {
    const [loadingAnimationStyle, setLoadingAnimationStyle] = useState();

    const maxOpacity = 0.6;
    const minOpacity = 0.2;

    const containerStyle = {
        display: 'flex',
        alignItems: 'center',
        marginLeft: '10px'
    }

    useEffect(() => {
        performLoadingAnimation(maxOpacity);
    }, []);

    const performLoadingAnimation = (opacity) => {
        const loadingMovieStyle = {
            width: '150px',
            height: '225px',
            borderRadius: '5px',
            backgroundColor: '#757575',
            margin: '10px',
            opacity: opacity,
            transition: 'opacity 0.5s ease'
        }

        setLoadingAnimationStyle(loadingMovieStyle);
            setTimeout(() => {
                performLoadingAnimation(opacity === maxOpacity ? minOpacity : maxOpacity);
            }, 500);
        
    }

    if(visible){
        return (<div style={containerStyle}>
            <div style={loadingAnimationStyle}></div>
            <div style={loadingAnimationStyle}></div>
            <div style={loadingAnimationStyle}></div>
            <div style={loadingAnimationStyle}></div>
            <div style={loadingAnimationStyle}></div>
            <div style={loadingAnimationStyle}></div>
        </div>);
    }
    else return null;
}