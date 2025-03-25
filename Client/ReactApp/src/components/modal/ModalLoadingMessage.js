import ModalWindow from "./ModalWindow";
import CircularProgressBar from "../common/CircularProgressBar";

function ModalLoadingMessage({ visible, loadingMessage }) {

    const contentStyle = {
        display: 'flex',
        flexDirection: 'column',
        justifyContent: 'center',
        alignItems: 'center',
        height: '100%'
    };

    const messageStyle = {
        color: "white",
        fontSize: '20px',
        fontWeight: 'bold',
        marginTop: '30px'
    };

    const modalContent = (<div style={contentStyle}>
        <CircularProgressBar size="x-large" visible={true} />
        <div style={messageStyle}>{loadingMessage}</div>
    </div>);


    return (
        <ModalWindow visible={visible} content={modalContent} />
    )
}

export default ModalLoadingMessage;