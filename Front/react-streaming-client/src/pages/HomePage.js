
import "../style/home-page.css";
import SuggestedMovies from "../components/movies/list/SuggestedMoviesList";
import MoviesListGenreLite from "../components/movies/list/MoviesListGenreLite";
import CircularProgressBar from "../components/common/CircularProgressBar";

import fadeTransition from "../js/customStyles.js";

import { useState } from 'react';

function HomePage({ genres, onMovieClick }) {
    const [dataLoaded, setDataLoaded] = useState(false);

    return (
        <div style={{height: '100%'}}>
            <CircularProgressBar color={'white'} size={'80px'} position={"center"} visible={!dataLoaded} />
            <div className="home-page-container" style={fadeTransition(dataLoaded)}>
                <SuggestedMovies onMoreClick={(movieId) => onMovieClick(movieId)} onDataLoaded={()=>setDataLoaded(true)}/>
                <div className="blur-divider"></div>               
                {genres.map((genre) =>
                (
                    <MoviesListGenreLite
                        key={genre}
                        visible={true}
                        genre={genre}
                        onMovieClick={(movieId) => onMovieClick(movieId)} />
                ))}               
            </div>
        </div>
    )
}

export default HomePage;