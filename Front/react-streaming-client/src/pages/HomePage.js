
import "../style/home-page.css";
import MoviesOfToday from "../components/movies/list/MoviesOfTodayList";
import MoviesListLiteWithTitle from "../components/movies/list/MoviesListLiteWithTitle";

import fadeTransition from "../js/customStyles.js";
import MoviesAPI from "../js/moviesAPI.js";
import CacheService from "../js/cacheService";

import { useState, useEffect, useRef } from 'react';

function HomePage({ onMovieClick, onReady, onFail }) {
    const [dataLoaded, setDataLoaded] = useState(false);

    const moviesLists = useRef([]);

    const cacheId = "popularmovieslists";

    const addMovies = (listTitle, movies, insertAtBeginning)=> {
        if(movies && movies.length > 0){ 
            let data = {
                title: listTitle,
                movies: movies
            }
            if(insertAtBeginning)moviesLists.current.unshift(data);
            else moviesLists.current.push(data);
            CacheService.setCache(cacheId, moviesLists.current);
        }
    };

    useEffect(()=>{
        let cache = CacheService.getCache(cacheId);

        if(cache){
            moviesLists.current = cache.data;
        }
        else{
            MoviesAPI.getPopularMovies((movies)=>{
                addMovies("Popular movies", movies, true);
                onReady();
            }, ()=>{
                onFail();
            });
            MoviesAPI.getRecommandedMovies((movies)=>{
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
    },[]);

    return (
        <div style={{height: '100%'}}>
            <div className="home-page-container" style={fadeTransition(dataLoaded)}>
                <MoviesOfToday onClick={(movieId) => onMovieClick(movieId)} onDataLoaded={()=>setDataLoaded(true)}/>
                <div className="blur-divider"></div>    
                 {moviesLists.current.map((moviesList) =>
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