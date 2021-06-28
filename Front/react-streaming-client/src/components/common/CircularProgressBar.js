import CircularProgress from '@material-ui/core/CircularProgress';
import { useEffect, useState } from 'react';
import fadeTransition from "../../js/customStyles.js";

function CircularProgressBar({ position, color, size, visible, text }) {

    const [positionStyle, setpositionStyle] = useState({});
    const textStyle = {
        color: 'white',
        fontSize: '20px',
        fontWeigth: 'bold',
        marginBottom: '10px'
    };

    useEffect(() => {
        if (position === 'center') {
            setpositionStyle({
                position: 'fixed',
                top: '50%',
                left: '50%',
                transform: 'translate(-50%, -50%)',
                textAlign: 'center'
            })
        }
    }, [position]);

    return (
        <div style={fadeTransition(visible)}>
            <div style={positionStyle}>
                <div style={textStyle}>{text}</div>
                <CircularProgress style={{ color: color, width: size, height: 'auto' }} />
            </div>
        </div>
    )
}

export default CircularProgressBar;