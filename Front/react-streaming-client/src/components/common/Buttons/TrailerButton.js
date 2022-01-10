import BaseButton from './BaseButton';
import PlayArrowIcon from '@material-ui/icons/PlayArrow';

function TrailerButton({ visible, onClick }) {
    if (visible) {

        const content = (<div style={{display: 'flex', justifyContent: 'center'}}>
            <PlayArrowIcon />
            <div style={{ marginLeft: '5px' }}>Trailer</div>
        </div>);

        return (
            <BaseButton content={content} color={"dark-red"} onClick={() => onClick()}/>
        )
    }
    else return null;
}

export default TrailerButton;