import "../style/modal-window.css";
import ClearIcon from '@material-ui/icons/Clear';
import fadeTransition from "../js/customStyles.js";

function ModalWindow({visible, content, onCloseClick}){

    return (
        <div style={fadeTransition(visible)} className="modal-window-container">
        <div className="modal-window-content">
            <div className="modal-window-close-btn" onClick={() => onCloseClick()}>
                <ClearIcon className="close-cross" />
            </div>
            {content}
        </div>
    </div>
    )
}

export default ModalWindow;