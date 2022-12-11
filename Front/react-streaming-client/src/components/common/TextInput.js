import "../../style/text-input.css";

import TextField from '@material-ui/core/TextField';
import ClearIcon from '@material-ui/icons/Clear';
import fadeTransition from "../../js/customStyles.js";

import { useEffect, useState, useRef, useReducer } from 'react';

function TextInput({ placeHolder, onTextChanged, large, value}){

    useEffect(()=>{
        setText(value);
    },[value]);

    const [text, setText] = useState('');

    const onValueChanged = (value) => {
        setText(value);
        onTextChanged(value);
    }

    return (
        <div className="text-input-container">
            <div className={"text-input " + (large ? 'large' : '')}>
                <TextField value={text} onChange={(e) => { onValueChanged(e.target.value) }} placeholder={ placeHolder } />
                <ClearIcon style={fadeTransition(text)} className='delete-text-cross' onClick={() => { onValueChanged('') }} />
            </div>
        </div>
    )
}

export default TextInput;