import "../../style/css/modal-error-log-infos.css";
import { formatDateTime } from "../../helpers/formatHelper";
import ModalWindow from "./ModalWindow";

function ModalErrorLogInfos({ visible, errorLog, onCloseClick }) {
    const errorLogInfosView = (
        <div className="modal-error-log-infos">
            <div>
                <strong>Timestamp</strong>
                <span>{formatDateTime(errorLog?.dateTime)}</span>
            </div>
            <div>
                <strong>Error message</strong>
                <span>{errorLog?.message ?? "-"}</span>
            </div>
            <div>
                <strong>Request URL</strong>
                <span>{errorLog?.requestUrl ?? "-"}</span>
            </div>
            <div>
                <strong>Stack trace</strong>
                <pre>{errorLog?.stackTrace ?? "-"}</pre>
            </div>
        </div>
    );

    return (
        <ModalWindow
            visible={visible}
            content={errorLogInfosView}
            onCloseClick={onCloseClick}
        />
    );
}

export default ModalErrorLogInfos;
