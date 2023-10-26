import "../../style/list-categories-view.css";
import ModalWindow from "./ModalWindow";
import CircularProgressBar from "../common/CircularProgressBar";
import AppServices from "../../services/AppServices";
import AppMode from "../../services/appMode";

import { useEffect, useState } from 'react';

function ModalCategories({ visible, onCloseClick, onGenreClick, onPlatformClick }) {
    const [loading, setLoading] = useState(true);
    const [listGenres, setListGenres] = useState([]);
    const [listPlatforms, setListPlatforms] = useState([]);

    const loadCategories = () => {
        setListGenres([]);
        AppServices.searchMediaService.getMediaGenres(
            (genres) => {
                if (genres && genres.length > 0) {
                    setListGenres(genres);
                }
            }
        );

        AppServices.searchMediaService.getMediaPlatforms(
            (platforms) => {
                if (platforms && platforms.length > 0) {
                    setListPlatforms(platforms);
                }
            }
        );
    }
    useEffect(() => {
        loadCategories();
        AppMode.onAppModeChanged(() => {
            loadCategories();
        });
    }, []);

    useEffect(() => {
        setLoading(!listGenres || listGenres.length === 0 || !listPlatforms || listPlatforms.length === 0);
    }, [listGenres, listPlatforms]);

    const listGenresView = () => {
        return (
            <div className="list-categories-view-container">
                <CircularProgressBar color={'white'} size={'60px'} position={'center'} visible={loading} />
                <h3 className="categorie-title">Genres</h3>
                <div className="list-categories">
                    {listGenres.map(genre =>
                        <div key={genre.id}
                            className="genre"
                            onClick={() => onGenreClick(genre)}>
                            {genre.name}
                        </div>)}
                </div>
                <h3 className="categorie-title">Platforms</h3>
                <div className="list-categories">
                    {listPlatforms.map(genre =>
                        <div key={genre.id}
                            className="genre"
                            onClick={() => onPlatformClick(genre)}>
                            {genre.name}
                        </div>)}
                </div>
            </div >
        )
    }

    return (
        <ModalWindow visible={visible} content={listGenresView()} onCloseClick={() => onCloseClick()} />
    )
}

export default ModalCategories;