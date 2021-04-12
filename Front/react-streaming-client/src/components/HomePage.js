
import "../style/home-page.css";
import SuggestedMovies from "./movies/list/SuggestedMoviesList";
import LastMoviesLists from "./movies/list/LastMoviesLists";
import CircularProgressBar from "./common/CircularProgressBar";

import fadeTransition from "../js/customStyles.js";

import { useState } from 'react';

function HomePage({ genres, onMovieClick }) {
    const [dataLoaded, setDataLoaded] = useState(false);

    return (
        <div style={{height: '100%'}}>
            <CircularProgressBar color={'white'} size={'80px'} position={"center"} visible={!dataLoaded} />
            <div className="home-page-container" style={fadeTransition(dataLoaded)}>
                <div className="suggested-movies">
                    <SuggestedMovies onMoreClick={(movieId) => onMovieClick(movieId)} onDataLoaded={()=>setDataLoaded(true)}/>
                </div>
                <div className="blur-divider"></div>
                <div className="last-movies">
                    <LastMoviesLists genres={genres} onMovieClick={(movieId) => onMovieClick(movieId)} />
                </div>
            </div>
        </div>
    )
}

export default HomePage;