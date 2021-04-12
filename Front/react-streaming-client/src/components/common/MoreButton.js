import "../../style/button.css";

function MoreButton({ onClick, color, center }) {
    return (
        <div className={"standard-button " + color + (center ? " center": "")} onClick={() => onClick()}>More</div>
    );
}

export default MoreButton;