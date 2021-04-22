
import MoviesListGenreLite from "../list/MoviesListGenreLite";
import { useRef, useEffect, useState } from 'react';

function LastMoviesLists({ genres, onMovieClick }) {
    const lastMoviesListRef = useRef(null);
    const [elementsVisible, setElementsVisible] = useState([]);
    const [elementsOpacity, setElementsOpacity] = useState([]);


    const isElementVisible = (elem, containerBoundings, bottomMargin, topMargin) => {
        const elemBoundings = elem.getBoundingClientRect();
        return Math.floor(elemBoundings.bottom) >= Math.floor(containerBoundings.top + topMargin)
            && Math.floor(elemBoundings.top) <= Math.floor(containerBoundings.bottom + bottomMargin);
    }

    const updateElementsVisibility = () => {
        var newElementsVisibility = [];
        var newElementsOpacity = [];
        var listBoundings = lastMoviesListRef.current.getBoundingClientRect();
        for (let i = 0; i < lastMoviesListRef.current.children.length; i++) {
            newElementsVisibility.push(isElementVisible(lastMoviesListRef.current.children[i], listBoundings, 0, 80));
            newElementsOpacity.push(!isElementVisible(lastMoviesListRef.current.children[i], listBoundings, 0, 200));
        }
        setElementsVisible(newElementsVisibility);
        setElementsOpacity(newElementsOpacity);
    }

    useEffect(() => {
        var newElementsVisibility = [];
        var newElementsOpacity = [];
        for (let i = 0; i < lastMoviesListRef.current.children.length; i++) {
            newElementsVisibility.push(true);
            newElementsOpacity.push(false);
        }
        setElementsVisible(newElementsVisibility);
        setElementsOpacity(newElementsOpacity);
    }, [genres]);

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
                    opacity={elementsOpacity[index]}
                    genre={genre}
                    onMovieClick={(movieId) => onMovieClick(movieId)} />
            ))}
        </div>
    );
}

export default LastMoviesLists;