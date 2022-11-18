import BaseButton from './buttons/BaseButton';
import ArrowDropDownOutlinedIcon  from '@material-ui/icons/ArrowDropDownOutlined';
import { useEffect, useState } from 'react';

function DropDown({values, width, onValueChanged}){

    const [showList, setShowList] = useState(false);
    const [selectedValue, setSelectedValue] = useState('');
    const [dropDownValues, setDropDownValues] = useState([]);

    useEffect(()=>{
        if(shouldUpdateDropDownValues(values)){
            setDropDownValues(values);
            setSelectedValue(values[0]);
        }
    },[values])

    const shouldUpdateDropDownValues = (values) => {
        if (dropDownValues.length !== values.length)
            return true;

        let nbDifferentValues = 0;
        dropDownValues.forEach((v, i) => {
            if(values[i] !== v) 
                nbDifferentValues += 1;
        });

        return nbDifferentValues > 0;
    }

    const containerStyle = {    
        position: 'relative'
    }

    const listStyle = {
        display: showList ? 'block' : 'none',
        position: 'absolute',
        bottom: '0',
        left: '50%',
        transform: 'translate(-50%, 100%)',
        borderRadius: '5px',
        boxShadow: 'rgb(0 0 0 / 91%) 0px 19px 20px 8px, rgb(0 0 0) 0px 15px 20px 0px',
        backgroundColor: 'white',
        color: 'black',
        fontSize: '16px',
        maxHeight: '100px',
        width: width,
        overflowY: 'scroll',
        zIndex: '5'
    }

    const buttonContentStyle = {
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        width: width,
    }

    const buttonContent = (
    <div style={buttonContentStyle}>
        <div>{selectedValue}</div>
        <ArrowDropDownOutlinedIcon />
    </div>)

    const listElementStyle = {
        padding: '5px',
        cursor: 'pointer'
    }

    const onElementMouseHover = (element) =>{
        element.style.backgroundColor = '#959595';
    }

    const onElementMouseOut = (element) =>{
        element.style.backgroundColor = '';
    }

    const OnElementClick = (index) => {
        setShowList(false);
        setSelectedValue(values[index]);
        onValueChanged(index);
    }

    return (
        <div style={containerStyle}>
            <BaseButton color="red" content={buttonContent} onClick={() => setShowList(!showList)}/>
            <div style={listStyle}>
                {dropDownValues.map((v,i) => <div 
                    key={v} 
                    style={listElementStyle}
                    onMouseOver={(e)=> onElementMouseHover(e.target)}
                    onMouseOut={(e)=> onElementMouseOut(e.target)} 
                    onClick={()=> OnElementClick(i)}>
                        {v}
                </div>)}
            </div>
        </div>
    )
}

export default DropDown;