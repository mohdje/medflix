import "../../style/css/list-categories-view.css";
import ModalWindow from "./ModalWindow";
import CircularProgressBar from "../common/CircularProgressBar";
import { mediasInfoApi } from "../../services/api";
import AppMode from "../../services/appMode";

import { useEffect, useState } from 'react';

function ModalCategories({ visible, onCloseClick, onGenreClick, onPlatformClick }) {
    const [listGenres, setListGenres] = useState([]);
    const [listPlatforms, setListPlatforms] = useState([]);

    const loadCategories = async () => {
        setListGenres([]);
        const genres = await mediasInfoApi.getMediaGenres();
        if (genres && genres.length > 0)
            setListGenres(genres);

        const platforms = await mediasInfoApi.getMediaPlatforms();
        if (platforms && platforms.length > 0)
            setListPlatforms(platforms);
    };

    useEffect(() => {
        loadCategories();
        AppMode.onAppModeChanged(() => {
            loadCategories();
        });
    }, []);

    const loading = !listGenres || listGenres.length === 0 || !listPlatforms || listPlatforms.length === 0;

    const listGenresView = () => {
        return (
            <div className="list-categories-view-container">
                <CircularProgressBar size="large" position="center" visible={loading} />
                <h1>Platforms</h1>
                <div className="list-categories">
                    {listPlatforms.map(genre =>
                        <h2 key={genre.id}
                            onClick={() => onPlatformClick(genre)}>
                            {genre.name}
                        </h2>)}
                </div>
                <h1>Genres</h1>
                <div className="list-categories">
                    {listGenres.map(genre =>
                        <h2 key={genre.id}
                            onClick={() => onGenreClick(genre)}>
                            {genre.name}
                        </h2>)}
                </div>
            </div >
        )
    }

    return (
        <ModalWindow visible={visible} content={listGenresView()} onCloseClick={() => onCloseClick()} />
    )
}

export default ModalCategories;