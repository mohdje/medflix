import "../../style/button.css";
import PlayArrowIcon from '@material-ui/icons/PlayArrow';

function PlayButton({ onClick }) {
    return (
        <div className="standard-button red" onClick={() => onClick()}>
            <PlayArrowIcon />
            <div style={{ marginLeft: '5px' }}>Play</div>
        </div>
    )
}

export default PlayButton;