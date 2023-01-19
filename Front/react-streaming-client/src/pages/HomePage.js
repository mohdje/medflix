
import "../style/home-page.css";
import MoviesOfToday from "../components/movies/list/MoviesOfTodayList";
import MoviesListLiteWithTitle from "../components/movies/list/MoviesListLiteWithTitle";

import fadeTransition from "../js/customStyles.js";
import MoviesAPI from "../js/moviesAPI.js";
import CacheService from "../js/cacheService";
import AppMode from "../js/appMode";

import { useState, useEffect, useRef } from 'react';

function HomePage({ onMovieClick, onReady, onFail }) {
    const [dataLoaded, setDataLoaded] = useState(false);

    const moviesListsRef = useRef([]);
    const cacheIdRef = useRef("");

    const [moviesLists, setMoviesLists] = useState([]);

    const buildCacheId = (appMode) => {
        return 'homepage' + appMode;
    }

    const todayTrendingTitle = "todayTrending"

    const addMovies = (listTitle, movies, insertAtBeginning)=> {
        if(movies && movies.length > 0){ 
            let data = {
                title: listTitle,
                movies: movies
            }
            if(insertAtBeginning)moviesListsRef.current.unshift(data);
            else moviesListsRef.current.push(data);
            CacheService.setCache(cacheIdRef.current, moviesListsRef.current);
            const newArray = [...moviesListsRef.current];
            setMoviesLists(newArray);
        }
    };

    const loadContent = () => {
        cacheIdRef.current = buildCacheId(AppMode.getActiveMode().label);

        let cache = CacheService.getCache(cacheIdRef.current);
        moviesListsRef.current = [];

        if(cache){
            moviesListsRef.current = cache.data;
            setMoviesLists(moviesListsRef.current);
            setDataLoaded(true);
        }
        else{
            MoviesAPI.getMoviesOfToday(
            (movies) => {
                addMovies(todayTrendingTitle, movies);
                setDataLoaded(true);
                onReady();
            });
            MoviesAPI.getPopularMovies((movies)=>{
                addMovies("Popular movies", movies, true);
            }, ()=>{
                onFail();
            });
            MoviesAPI.getRecommandedMovies(null, (movies)=>{
                addMovies("Recommanded for you", movies, true);
            });
            MoviesAPI.getPopularNetflixMovies((movies)=>{
                addMovies("Popular on Netflix", movies);
            });
            MoviesAPI.getPopularDisneyPlusMovies((movies)=>{
                addMovies("Popular on Disney Plus", movies);
            });
            MoviesAPI.getPopularAmazonPrimeMovies((movies)=>{
                addMovies("Popular on Amazon Prime", movies);
            });
        }
    }

    useEffect(()=>{
        loadContent();
        AppMode.onAppModeChanged(()=>{
            loadContent();
        });
    },[]);

    return (
        <div style={{height: '100%'}}>
            <div className="home-page-container" style={fadeTransition(dataLoaded)}>
                <MoviesOfToday movies={moviesLists.find(list => list.title === todayTrendingTitle)?.movies} onClick={(movieId) => onMovieClick(movieId)}/>
                <div className="blur-divider"></div>    
                 {moviesLists.filter(list => list.title !== todayTrendingTitle).map((moviesList) =>
                (
                    <MoviesListLiteWithTitle
                        key={moviesList.title}
                        visible={true}
                        listTitle={moviesList.title}
                        movies={moviesList.movies}
                        onMovieClick={(movieId) => onMovieClick(movieId)} />
                ))}    
            </div>
        </div>
    )
}

export default HomePage;