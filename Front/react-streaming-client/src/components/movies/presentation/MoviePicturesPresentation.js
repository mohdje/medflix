
function MoviePicturesPresentation({ movie, visible, onClick }) {
    return (
        <div className={"movie-pictures " + (!visible ? "hidden" : "")} onClick={() => onClick(movie.id)}>
            <div className="logo-title">
                <img src={movie?.logoImageUrl}></img>
            </div>
            <img className="background-image" src={movie?.backgroundImageUrl}></img>
        </div>
    );
}

export default MoviePicturesPresentation;