import Button from './Button';
import DropdownArrow from '../../assets/dropdown_arrow.svg'
import { useOnClickOutside } from '../../helpers/customHooks';
import { useEffect, useState, useRef } from 'react';

function DropDown({ values, width, onValueChanged, textAlignement }) {

    const [showList, setShowList] = useState(false);
    const [selectedValue, setSelectedValue] = useState('');
    const [dropDownValues, setDropDownValues] = useState([]);
    const dropdownRef = useRef(null);

    useEffect(() => {
        if (shouldUpdateDropDownValues(values)) {
            setDropDownValues(values);
            setSelectedValue(values[0]);
        }
    }, [values])

    useOnClickOutside(dropdownRef, () => setShowList(false));

    const shouldUpdateDropDownValues = (values) => {
        if (dropDownValues.length !== values.length)
            return true;

        let nbDifferentValues = 0;
        dropDownValues.forEach((v, i) => {
            if (values[i] !== v)
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
        backgroundColor: '#141414',
        color: 'white',
        fontSize: '16px',
        fontWeight: '500',
        maxHeight: '100px',
        width: width,
        overflowY: 'scroll',
        zIndex: '5',
        textAlign: textAlignement ? textAlignement : 'left'
    }

    const listElementStyle = {
        padding: '5px',
        cursor: 'pointer'
    }

    const onElementMouseHover = (element) => {
        element.style.backgroundColor = '#959595';
    }

    const onElementMouseOut = (element) => {
        element.style.backgroundColor = '';
    }

    const OnElementClick = (element, index) => {
        element.style.backgroundColor = '#959595';
        setTimeout(() => {
            element.style.backgroundColor = '';
            setShowList(false);
            setSelectedValue(values[index]);
            onValueChanged(index);
        }, 200);

    }

    return (
        <div ref={dropdownRef} style={containerStyle}>
            <Button color="red" large imgSrc={DropdownArrow} text={selectedValue} onClick={() => setShowList(!showList)} />
            <div style={listStyle}>
                {dropDownValues.map((v, i) => <div
                    key={v}
                    style={listElementStyle}
                    onMouseOver={(e) => onElementMouseHover(e.target)}
                    onMouseOut={(e) => onElementMouseOut(e.target)}
                    onClick={(e) => OnElementClick(e.target, i)}>
                    {v}
                </div>)}
            </div>
        </div>
    )
}

export default DropDown;