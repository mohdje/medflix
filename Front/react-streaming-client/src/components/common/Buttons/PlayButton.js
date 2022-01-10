import BaseButton from './BaseButton';
import PlayArrowIcon from '@material-ui/icons/PlayArrow';

function PlayButton({ onClick }) {

    const content = (<div style={{ display: 'flex', justifyContent: 'center' }}>
        <PlayArrowIcon />
            <div style={{ marginLeft: '5px' }}>Play</div>
    </div>);

    return (
        <BaseButton content={content} color={"red"} onClick={() => onClick()} />
    );
}

export default PlayButton;