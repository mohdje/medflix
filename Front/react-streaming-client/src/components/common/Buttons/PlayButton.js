import BaseButton from './BaseButton';
import PlayArrowIcon from '@material-ui/icons/PlayArrow';

function PlayButton({center, onClick }) {

    const content = (<div style={{ display: 'flex', justifyContent: 'center' }}>
        <PlayArrowIcon />
            <div style={{ marginLeft: '5px' }}>Play</div>
    </div>);

    return (
        <BaseButton content={content} centered={center} color={"red"} onClick={() => onClick()} />
    );
}

export default PlayButton;