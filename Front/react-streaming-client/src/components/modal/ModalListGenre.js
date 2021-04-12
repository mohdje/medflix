import "../../style/list-genres-view.css";
import ModalWindow from "./ModalWindow";
import CircularProgressBar from "../common/CircularProgressBar";

import { useEffect, useState } from 'react';

function ModalListGenre({ genres, visible, onCloseClick, onGenreClick }) {
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
        setLoading(!genres || genres.length === 0);
    }, [genres]);

    return (
        <ModalWindow visible={visible} content={listGenresView()} onCloseClick={() => onCloseClick()} />
    )
}

export default ModalListGenre;