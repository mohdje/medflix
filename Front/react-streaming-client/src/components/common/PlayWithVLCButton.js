import "../../style/button.css";
import PlayArrowIcon from '@material-ui/icons/PlayArrow';

function PlayWithVLCButton({ onClick }) {
    return (
        <div className="standard-button orange" onClick={() => onClick()}>
            <PlayArrowIcon />
            <div style={{ marginLeft: '5px' }}>Play With VLC</div>
        </div>
    )
}

export default PlayWithVLCButton;