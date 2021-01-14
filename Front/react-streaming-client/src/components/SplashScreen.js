
import "../style/splashscreen.css";
import logo from '../assets/medflix.png';

function SplashScreen({visible}) {
    
    return (
        <div className={"splash-screen-container" + (visible ? '' : ' hidden')}>
           <img alt="" src={logo} />
        </div>
    )
}

export default SplashScreen;