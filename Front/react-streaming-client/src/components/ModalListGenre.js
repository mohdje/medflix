import "../style/list-genres-view.css";
import MoviesAPI from "../js/moviesAPI.js";
import ModalWindow from "./ModalWindow";
import CircularProgressBar from "./CircularProgressBar";

import { useEffect, useState } from 'react';

function ModalListGenre({ visible, onCloseClick, onGenreClick }) {

    const [genres, setGenres] = useState([]);
    const [loading, setLoading] = useState(true);

    const listGenresView = () => {
        return (
            <div className="list-genres-view-container">
                <CircularProgressBar color={'white'} size={'60px'} position={'center'} visible={loading} />
                <div className="list-genres">
                    {genres.map(genre =>
                        <div key={genre}
                            className="genre"
                            onClick={() => onGenreClick(genre)}>
                            {genre}
                        </div>)}
                </div>
            </div>

        )
    }

    useEffect(() => {
        setLoading(true);
        MoviesAPI.getMoviesGenres(
            (genres) => {
                if (genres && genres.length > 0) setGenres(genres);
                setLoading(false);
            }
        );
    }, []);

    return (
        <ModalWindow visible={visible} content={listGenresView()} onCloseClick={() => onCloseClick()} />
    )
}

export default ModalListGenre;