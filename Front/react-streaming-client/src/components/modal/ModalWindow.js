import "../../style/modal-window.css";
import ClearIcon from '@material-ui/icons/Clear';
import fadeTransition from "../../js/customStyles.js";

function ModalWindow({ visible, content, onCloseClick }) {
    return (
        <div style={fadeTransition(visible)} className={"modal-window-container" + (visible ? " visible": "")}>
            <div className="modal-window-content">
                <div className="modal-window-close-btn" style={{ display: onCloseClick ? '' : "none" }} onClick={() => onCloseClick()}>
                    <ClearIcon className="close-cross" />
                </div>
                {content}
            </div>
        </div>
    )
}

export default ModalWindow;