import "../../../style/button.css";
import { useState } from 'react';

function BaseButton({ color, content, centered, rounded, onClick }) {

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

    return (
        <div className={computeClass()} onClick={() => handleClick()}>
            <div className={"ripple-effect " + (showRippleEffect ? 'visible' : '')}></div>
            {content}
        </div>
    )
}

export default BaseButton;