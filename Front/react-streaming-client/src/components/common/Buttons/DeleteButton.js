import BaseButton from './BaseButton';
import DeleteIcon from '@material-ui/icons/Delete';

function DeleteButton({ onClick, visible }) {
    if (visible) {
        const content = (<div style={{ display: 'flex', justifyContent: 'center' }}>
             <DeleteIcon />
            <div style={{ marginLeft: '5px' }}>Delete</div>
        </div>);

        return (
            <BaseButton content={content} color={"dark-red"} onClick={() => onClick()} />
        );
    }
    else return null;
}

export default DeleteButton;