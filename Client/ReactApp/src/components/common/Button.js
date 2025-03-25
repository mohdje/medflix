import "../../style/css/button.css"
import { useRef } from "react";
import { useRippleEffect } from "../../helpers/customHooks";

export default function Button({ text, imgSrc, color, rounded, large, disabled, onClick }) {
    const buttonRef = useRef(null);

    useRippleEffect(buttonRef);

    const handleClick = () => {
        setTimeout(() => {
            onClick();
        }, 300);
    }

    const className = [
        color,
        rounded ? 'rounded' : '',
        large ? 'large' : '',
        imgSrc && !text ? 'img-only' : ''
    ].join(' ');

    return <button ref={buttonRef} disabled={disabled} className={className} onClick={() => handleClick()}>
        {imgSrc ? <img src={imgSrc}></img> : null}
        {text ? <h5>{text}</h5> : null}
    </button >
}