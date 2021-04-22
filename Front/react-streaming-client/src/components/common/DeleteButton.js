import "../../style/button.css";
import DeleteIcon from '@material-ui/icons/Delete';

function DeleteButton({ onClick, visible }) {
    return (
        <div className="standard-button dark-red" style={{display: visible ? '' : 'none'}} onClick={() => onClick()}>
            <DeleteIcon />
            <div style={{ marginLeft: '5px' }}>Delete</div>
        </div>
    )
}

export default DeleteButton;