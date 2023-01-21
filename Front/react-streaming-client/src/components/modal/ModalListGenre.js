import "../../style/list-genres-view.css";
import ModalWindow from "./ModalWindow";
import CircularProgressBar from "../common/CircularProgressBar";
import MoviesAPI from "../../services/moviesAPI.js";
import AppMode from "../../services/appMode";

import { useEffect, useState } from 'react';

function ModalListGenre({ visible, onCloseClick, onGenreClick }) {
    const [loading, setLoading] = useState(true);
    const [listGenres, setListGenres] = useState([]);

    const loadGenres = () => {
        setListGenres([]);
        MoviesAPI.getMoviesGenres(
            (genres) => {
              if (genres && genres.length > 0) {
                setListGenres(genres);
              }
            }
          );
    }
    useEffect(() => {
        loadGenres();
        AppMode.onAppModeChanged(()=>{
            loadGenres();
        });
    }, []);

    useEffect(() => {
        setLoading(!listGenres || listGenres.length === 0);
    }, [listGenres]);

    const listGenresView = () => {
        return (
            <div className="list-genres-view-container">
                <CircularProgressBar color={'white'} size={'60px'} position={'center'} visible={loading} />
                <div className="list-genres">
                    {listGenres.map(genre =>
                        <div key={genre.id}
                            className="genre"
                            onClick={() => onGenreClick(genre)}>
                            {genre.name}
                        </div>)}
                </div>
            </div>
        )
    }

    return (
        <ModalWindow visible={visible} content={listGenresView()} onCloseClick={() => onCloseClick()} />
    )
}

export default ModalListGenre;