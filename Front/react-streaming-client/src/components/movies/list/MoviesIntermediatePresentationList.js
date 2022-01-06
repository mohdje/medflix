import "../../../style/movie-intermediate-list.css";

import MovieIntermediatePresentation from "../presentation/MovieIntermediatePresentation";
import VideoPlayerWindow from '../../video/VideoPlayerWindow';
import CircularProgressBar from "../../common/CircularProgressBar";
import ModalLoadingMessage from "../../modal/ModalLoadingMessage";
import MoviesAPI from "../../../js/moviesAPI.js";
import { useEffect, useState } from 'react';

function MoviesIntermediatePresentationList({ title, moviesBookmarks, centerToMovieBookmark, showDeleteButton, loadingProgressVisible, onMoreClick, onDeleteClick }) {
    const [selectedMovie, setSelectedMovie] = useState(null);
    const [moviesServiceLoadingVisible, setMoviesServiceLoadingVisible] = useState(false);

    const selectMoviesService = (movieBookmark, callback) => {
        setMoviesServiceLoadingVisible(true);
        setTimeout(() => {
            MoviesAPI.selectVOMovieService(movieBookmark.serviceId, () => {
                setMoviesServiceLoadingVisible(false);
                callback();
            })
        }, 2000);
    }

    useEffect(() => {
        if (centerToMovieBookmark) {
            var index = moviesBookmarks.findIndex(m => m.movie.id === centerToMovieBookmark.movie.id && m.serviceName === centerToMovieBookmark.serviceName);
            var elem = document.getElementById("movieintermediatepresentation" + index);
            if (elem) {
                elem.scrollIntoView({
                    block: "nearest",
                    inline: "nearest"
                });
            }
        }
    }, [moviesBookmarks, centerToMovieBookmark]);

    useEffect(() => {
        console.log(selectedMovie);
        if (selectedMovie) MoviesAPI.saveLastSeenMovie(selectedMovie);
    }, [selectedMovie]);

    return (
        <div className="movie-intermediate-list-container">
            <CircularProgressBar color={'white'} size={'80px'} position={"center"} visible={loadingProgressVisible} />
            <ModalLoadingMessage visible={moviesServiceLoadingVisible} loadingMessage={'Movies service selection'} />
            <VideoPlayerWindow visible={selectedMovie} movie={selectedMovie} onCloseClick={() => setSelectedMovie(null)} />
            <div className="movie-intermediate-list-title-page">{title}</div>
            <div className="movie-intermediate-list-content">
                {moviesBookmarks.map((movieBookmark, index) =>
                (<div id={"movieintermediatepresentation" + index} key={index}>
                    <MovieIntermediatePresentation
                        movieBookmark={movieBookmark}
                        deleteButtonAvailable={showDeleteButton}
                        onMoreClick={() => selectMoviesService(movieBookmark, () => onMoreClick(movieBookmark))}
                        onPlayClick={() => selectMoviesService(movieBookmark, () => setSelectedMovie(movieBookmark.movie))}
                        onDeleteClick={() => onDeleteClick(movieBookmark)} />
                </div>))}
            </div>
        </div>
    )
}

export default MoviesIntermediatePresentationList;