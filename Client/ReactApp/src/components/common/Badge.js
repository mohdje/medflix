import "../../style/css/badge.css";

function Badge({ text, active = false, onClick }) {
    return <span className={`badge ${active ? 'active' : 'inactive'}`} onClick={onClick}>{text}</span>;
}

export default Badge;