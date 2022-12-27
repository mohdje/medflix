
import "../style/splashscreen.css";
import logo from '../assets/medflix_logo.png';
import CircularProgressBar from "./common/CircularProgressBar";

function SplashScreen({visible, showErrorMessage}) {
    
    return (
        <div className={"splash-screen-container" + (visible ? '' : ' hidden')}>
           <img alt="" src={logo} />
           <CircularProgressBar color='red' size='60px' visible={!showErrorMessage}/>
           <div className="error-message" style={{display: showErrorMessage ? '' : 'none'}}>
                The application failed to load, try to relaunch it
           </div>
        </div>
    )
}

export default SplashScreen;