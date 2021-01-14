
import MoviesListGenreLite from "./MoviesListGenreLite";
import { useRef, useEffect, useState } from 'react';

function LastMoviesLists({ onShowGenreFullList, onMovieClick }) {
    const genres = ['Thriller', 'Crime', 'Animation', 'Sci-Fi'];//'Comedy', 'Horror', 'Drama'
    const lastMoviesListRef = useRef(null);
    const [elementsVisible, setElementsVisible] = useState([]);

    const isElementVisible = (elem, containerBoundings) => {
        const elemBoundings = elem.getBoundingClientRect();

        return Math.floor(elemBoundings.top) <= Math.floor(containerBoundings.bottom - 100)
            && Math.floor(elemBoundings.bottom) >= Math.floor(containerBoundings.top + 150);
    }

    const updateElementsVisibility = () => {
        var newElementsVisibility = [];
        var listBoundings = lastMoviesListRef.current.getBoundingClientRect();
        for (let i = 0; i < lastMoviesListRef.current.children.length; i++) {
            newElementsVisibility.push(isElementVisible(lastMoviesListRef.current.children[i], listBoundings));
        }
        setElementsVisible(newElementsVisibility);
    }

    useEffect(() => {
        var newElementsVisibility = [];
        for (let i = 0; i < lastMoviesListRef.current.children.length; i++) {
            newElementsVisibility.push(true);
        }
        setElementsVisible(newElementsVisibility);
    }, []);

    useEffect(() => {
        if (lastMoviesListRef?.current)
            lastMoviesListRef.current.addEventListener("scroll", updateElementsVisibility, false);
        return () => {
            if (lastMoviesListRef?.current)
                lastMoviesListRef.current.removeEventListener("scroll", updateElementsVisibility);
        }
    }, [lastMoviesListRef]);

    return (
        <div ref={lastMoviesListRef} style={{ height: '100%', overflowY: 'scroll' }}>
            {genres.map((genre, index) =>
            (
                <MoviesListGenreLite
                    key={genre}
                    visible={elementsVisible[index]}
                    genre={genre}
                    onMoreClick={() => onShowGenreFullList(genre)}
                    onMovieClick={(movieId) => onMovieClick(movieId)} />
            ))}
        </div>
    );
}

export default LastMoviesLists;