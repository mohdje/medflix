import ModalWindow from "./ModalWindow";

function ModalMovieTrailer({visible, youtubeTrailerUrl, onCloseClick}){

    const trailerStyle = {      
            width: '800px',
            height: '500px',
            margin: 'auto 0',
            backgroundColor: 'black',
            border: 'none',
            boxShadow: '1px 1px 14px 7px #101010c9'       
    }

    const content = () => {
        return (
            <iframe style={trailerStyle} src={youtubeTrailerUrl}></iframe>
        );
    };

    return (
        <ModalWindow visible={visible} content={content()} onCloseClick={() => onCloseClick()} />
    );
}

export default ModalMovieTrailer; 