import "../../../style/button.css";
import { useState } from 'react';

function BaseButton({ color, content, centered, onClick }) {

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