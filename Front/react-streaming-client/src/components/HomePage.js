
import "../style/home-page.css";
import SuggestedMovies from "./movies/list/SuggestedMoviesList";
import LastMoviesLists from "./movies/views/LastMoviesView";
import CircularProgressBar from "./common/CircularProgressBar";

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
                <LastMoviesLists genres={genres} onMovieClick={(movieId) => onMovieClick(movieId)} />
            </div>
        </div>
    )
}

export default HomePage;