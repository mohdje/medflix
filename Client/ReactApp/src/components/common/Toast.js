import "../../style/css/toast.css"

export default function Toast({ message, visible }) {
    return <div className={`toast-message-container ${visible ? 'visible' : ''}`}>
        <h3>{message}</h3>
    </div>
}