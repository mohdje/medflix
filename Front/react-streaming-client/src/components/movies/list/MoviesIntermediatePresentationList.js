import "../../../style/movie-intermediate-list.css";

import MovieIntermediatePresentation from "../presentation/MovieIntermediatePresentation";
import CircularProgressBar from "../../common/CircularProgressBar";
import { useEffect, useState } from 'react';

function MoviesIntermediatePresentationList({ title, movies, centerToMovie, loadingProgressVisible, onClick }) {

    useEffect(() => {
        if (centerToMovie) {
            var index = movies.findIndex(m => m.id === centerToMovie.id);
            var elem = document.getElementById("movieintermediatepresentation" + index);
            if (elem) {
                elem.scrollIntoView({
                    block: "nearest",
                    inline: "nearest"
                });
            }
        }
    }, [movies, centerToMovie]);

    return (
        <div className="movie-intermediate-list-container">
            <CircularProgressBar color={'white'} size={'80px'} position={"center"} visible={loadingProgressVisible} />
           
            <div className="movie-intermediate-list-title-page">{title}</div>
            <div className="movie-intermediate-list-content">
                {movies.map((movie, index) =>
                (<div id={"movieintermediatepresentation" + index} key={index}>
                    <MovieIntermediatePresentation
                        movie={movie}
                        onClick={() => onClick(movie)}/>
                </div>))}
            </div>
        </div>
    )
}

export default MoviesIntermediatePresentationList;