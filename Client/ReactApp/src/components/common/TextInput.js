import "../../style/css/text-input.css";

import ClearIcon from '../../assets/cross.svg';
import { useFadeTransition } from "../../helpers/customHooks.js";

import { useRef, useState } from 'react';

function TextInput({ placeHolder, onTextChanged }) {
    const inputRef = useRef(null);
    const crossImgRef = useRef(null);

    const [showCleanTextButton, setShowCleanTextButton] = useState(false);
    useFadeTransition(crossImgRef, showCleanTextButton);

    const onValueChanged = (value) => {
        setShowCleanTextButton(value);
        onTextChanged(value);
        if (!value)
            inputRef.current.value = "";
    }

    return (
        <div className="text-input-container">
            <input
                ref={inputRef}
                type="text"
                placeholder={placeHolder}
                onChange={(e) => onValueChanged(e.target.value)}>
            </input>
            <div className="bottom-outline"></div>
            <img
                ref={crossImgRef}
                className='delete-text-cross'
                onClick={() => onValueChanged('')}
                src={ClearIcon} />
        </div>
    )
}

export default TextInput;