
import "../style/css/splashscreen.css";
import logo from '../assets/logo_full.svg';

function SplashScreen({ visible, showErrorMessage }) {
    if (!visible)
        return null;

    return <div className="splash-screen-container">
        <img alt="logo" src={logo} />
        {showErrorMessage ? <h2>The application failed to load, try to relaunch it</h2> : <HorizontalLoadingBar visible={!showErrorMessage} />}
    </div>

}

export default SplashScreen;

function HorizontalLoadingBar({ visible }) {
    if (!visible)
        return null;

    return <div className="horizontal-loading-bar">
        <div className="loading-bar"></div>
    </div>;
}