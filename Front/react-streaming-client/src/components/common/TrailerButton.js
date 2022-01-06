import "../../style/button.css";
import PlayArrowIcon from '@material-ui/icons/PlayArrow';

function TrailerButton({ visible, onClick }) {
    if(visible){
        return (
            <div className="standard-button dark-red" onClick={() => onClick()}>
                <PlayArrowIcon />
                <div style={{ marginLeft: '5px' }}>Trailer</div>
            </div>
        )
    }
    else return null;
}

export default TrailerButton;