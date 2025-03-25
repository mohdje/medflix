import "../../style/css/modal-window.css";
import CrossIcon from "../../assets/cross.svg";
import Button from "../common/Button.js";
import { useRef } from "react";
import { useFadeTransition } from "../../helpers/customHooks.js";

function ModalWindow({ visible, content, onCloseClick }) {
    const modalRef = useRef(null);
    useFadeTransition(modalRef, visible);

    return (
        <div ref={modalRef} className={"modal-window-container" + (visible ? " visible" : "")}>
            <div className="modal-window-content">
                <div className="close-btn-container" style={{ display: onCloseClick ? '' : "none" }}>
                    <Button imgSrc={CrossIcon} color="transparent" rounded onClick={() => onCloseClick()} />
                </div>
                {content}
            </div>
        </div>
    )
}

export default ModalWindow;