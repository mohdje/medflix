import ModalWindow from "./ModalWindow";
import { useEffect, useState } from 'react';

function ModalMovieTrailer({visible, youtubeTrailerUrl, onCloseClick}){

    const [trailerUrl, setTrailerUrl] = useState('');

    useEffect(()=> {
        if(visible && youtubeTrailerUrl)
            setTrailerUrl(youtubeTrailerUrl);
        else 
            setTrailerUrl('');
    },[visible]);

    const trailerStyle = {      
            width: '800px',
            height: '500px',
            marginTop: '120px',
            backgroundColor: 'black',
            border: 'none',
            boxShadow: '1px 1px 14px 7px #101010c9'       
    }

    const content = () => {
        return (
            <iframe style={trailerStyle} src={trailerUrl}></iframe>
        );
    };

    return (
        <ModalWindow visible={visible} content={content()} onCloseClick={() => onCloseClick()} />
    );
}

export default ModalMovieTrailer; 