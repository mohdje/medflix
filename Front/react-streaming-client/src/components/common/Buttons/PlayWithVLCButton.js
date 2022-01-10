import BaseButton from './BaseButton';
import PlayArrowIcon from '@material-ui/icons/PlayArrow';

function PlayWithVLCButton({ onClick }) {
    const content = (<div style={{ display: 'flex', justifyContent: 'center' }}>
        <PlayArrowIcon />
        <div style={{ marginLeft: '5px' }}>Play With VLC</div>
    </div>);

    return (
        <BaseButton content={content} color={"orange"} onClick={() => onClick()} />
    );
}

export default PlayWithVLCButton;