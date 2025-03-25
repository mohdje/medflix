import SpinnerIcon from '../../assets/spinner.svg';
import "../../style/css/animations.css"
import "../../style/css/circular-progress-bar.css";
import { useRef } from 'react';
import { useFadeTransition } from "../../helpers/customHooks.js";

export default function CircularProgressBar({ position, size, visible, text }) {
    const containerRef = useRef(null);

    useFadeTransition(containerRef, visible);

    const classNames = [
        "circular-progress-bar-container",
        position === "center" ? "fixed-center" : "normal",
        size
    ].join(" ");

    return (
        <div ref={containerRef} className={classNames}>
            <div>{text}</div>
            <img className="rotate" src={SpinnerIcon}></img>
        </div>
    )
}