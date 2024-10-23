import BaseButton from './BaseButton';
import TrailerIcon from '@material-ui/icons/LocalMovies';

function TrailerButton({ visible, onClick }) {
    if (visible) {

        const content = (<div style={{display: 'flex', justifyContent: 'center'}}>
            <TrailerIcon style={{ color: 'black' }}/>
            <div style={{ marginLeft: '5px' }}>Trailer</div>
        </div>);

        return (
            <BaseButton content={content} color={"white"} onClick={() => onClick()}/>
        )
    }
    else return null;
}

export default TrailerButton;