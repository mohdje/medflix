import "../../../style/button.css";
import { useState } from 'react';

function BaseButton({ color, content, centered, rounded, width, onClick }) {

    const [showRippleEffect, setShowRippleEffect] = useState(false);

    const handleClick = () => {
        setShowRippleEffect(true)
        setTimeout(() => {
            setShowRippleEffect(false);
            onClick();
        }, 300);
    };

    const computeClass = () => {
        let className = "standard-button";
        if (color) className += " " + color;
        if (centered) className += " center";
        if(rounded) className += " rounded";
        return className;
    }

    const computeStyle = () => {
        if(width){
            return {
                width: width
            }
        }
        else
            return null;
    }

    return (
        <div className={computeClass()} style={computeStyle()} onClick={() => handleClick()}>
            <div className={"ripple-effect " + (showRippleEffect ? 'visible' : '')}></div>
            {content}
        </div>
    )
}

export default BaseButton;