import "../../style/css/text-input.css";

import ClearIcon from '../../assets/cross.svg';
import { useFadeTransition } from "../../helpers/customHooks.js";

import { useRef, useState } from 'react';

function TextInput({ placeHolder, onTextChanged, integerOnly = false, minValue = 1, maxValue = 999 }) {
    const inputRef = useRef(null);
    const crossImgRef = useRef(null);

    const [showCleanTextButton, setShowCleanTextButton] = useState(false);
    useFadeTransition(crossImgRef, showCleanTextButton);

    const onValueChanged = (value) => {
        if (integerOnly) {
            value = sanitizeIntegerInput(value);
            inputRef.current.value = value;
        }

        if (!value)
            inputRef.current.value = "";

        setShowCleanTextButton(value);
        onTextChanged(value);
    }

    const sanitizeIntegerInput = (inputValue) => {
        let value = inputValue.replace(/[^0-9]/g, '');
        // Remove leading zeros
        value = value.replace(/^0+/, '') || '';

        if (value === '')
            return;

        let num = parseInt(value, 10);
        if (num < minValue) num = minValue;
        if (num > maxValue) num = maxValue;

        return num;
    };


    return (
        <div className="text-input-container">
            <input
                ref={inputRef}
                type={"text"}
                inputmode={integerOnly ? "numeric" : undefined}
                min={integerOnly ? minValue : undefined}
                max={integerOnly ? maxValue : undefined}
                maxlength={integerOnly ? maxValue.toString().length : undefined}
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