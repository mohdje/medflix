import CircularProgress from '@material-ui/core/CircularProgress';
import { useEffect, useState } from 'react';
import fadeTransition from "../js/customStyles.js";

function CircularProgressBar({ position, color, size, visible }) {

    const [positionStyle, setpositionStyle] = useState({});
    useEffect(() => {
        if (position === 'center') {
            setpositionStyle({
                position: 'fixed',
                top: '50%',
                left: '50%',
                transform: 'translate(-50%, -50%)'
            })
        }
    }, [position]);

    return (
        <div style={fadeTransition(visible)}>
            <div style={positionStyle}>
                <CircularProgress style={{ color: color, width: size, height: 'auto' }} />
            </div>
        </div>
    )
}

export default CircularProgressBar;