import { useEffect, useState } from 'react';

function Slider({ value, width, height, progressColor, cursorColor, tooltip, onCursorMoved, onCursorEndMove, onMouseOverSlider }) {
    const [cursorPosition, setCursorPosition] = useState(0);
    const [showToolTip, setShowToolTip] = useState(false);

    const positionToPercentage = (XPosition, railBoundings) => {
        if (XPosition < railBoundings.left)
            return 0;
        else if (XPosition > railBoundings.right)
            return 100;
        else
            return (XPosition - railBoundings.left) * 100 / (railBoundings.right - railBoundings.left);
    }

    const initiateCursorMove = () => {
        document.addEventListener('mousemove', moveCursor);
        document.addEventListener('mouseup', endMove);
    }

    const moveCursor = (event) => {
        var percentage = positionToPercentage(event.clientX, event.target.parentElement.getBoundingClientRect());
        setCursorPosition(percentage);
        if (onCursorMoved) onCursorMoved(percentage);
    }

    const endMove = (event) => {
        if (onCursorEndMove) {
            var percentage = positionToPercentage(event.clientX, event.target.parentElement.getBoundingClientRect());
            onCursorEndMove(percentage);
        }
        document.removeEventListener('mouseup', endMove);
        document.removeEventListener('mousemove', moveCursor);
    }

    const onSliderClick = (event) => {
        var percentage = positionToPercentage(event.clientX, event.target.parentElement.getBoundingClientRect());
        setCursorPosition(percentage);
        if (onCursorEndMove) onCursorEndMove(percentage);
    }

    const onMouseMoveOverSlider = (event) => {
        var percentage = positionToPercentage(event.clientX, event.target.parentElement.getBoundingClientRect());
        if (onMouseOverSlider) onMouseOverSlider(percentage);
    }

    const onMouseLeaveSlider = () => {
        setShowToolTip(false);
    }

    useEffect(() => {
        setCursorPosition(value);
    }, [value])

    useEffect(() => {
        if (tooltip) setShowToolTip(true);
    }, [tooltip])

    return (
        <div className="video-player-slider-container"
            style={{ width: width, height: height }}
            onClick={(event) => onSliderClick(event)}
            onMouseMove={(event) => onMouseMoveOverSlider(event)}
            onMouseLeave={() => onMouseLeaveSlider()}>
            <div className="complete-part" style={{ width: cursorPosition + "%", backgroundColor: progressColor }}></div>
            <div className="remaining-part" style={{ width: (100 - cursorPosition) + "%" }}></div>
            <div className="cursor" style={{ left: cursorPosition + '%', backgroundColor: cursorColor }} onMouseDown={() => initiateCursorMove()}></div>
            <div className="tooltip" style={{ left: tooltip?.position + '%', display: showToolTip ? '' : 'none' }}>{tooltip?.text}</div>
        </div>)
}


export default Slider;