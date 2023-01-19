import "../../style/nav-bar.css";
import logo from '../../assets/logo.png';
import SearchIcon from '@material-ui/icons/Search';
import HomeIcon from '@material-ui/icons/Home';
import ViewModuleIcon from '@material-ui/icons/ViewModule';
import VisibilityIcon from '@material-ui/icons/Visibility';
import BookmarkIcon from '@material-ui/icons/Bookmark';
import LinkIcon from '@material-ui/icons/Link';
import { useEffect, useState } from 'react';
import AppMode from '../../js/appMode';

function NavBar({ onSearchClick, onHomeClick, onGenreMenuClick, onWatchedMoviesClick, onBookmarkedMoviesClick, onTorrentLinkClick, onAppModeSwitch }) {

    const [modes, setModes] = useState([]);

    useEffect(()=>{
        setModes(AppMode.modes);
    },[]);

    const switchAppMode = () => {
        AppMode.switchActiveMode();
        const updatedModes = [...AppMode.modes];
        setModes(updatedModes);
        onAppModeSwitch(AppMode.getActiveMode());
    }

    const navBarIconStyle = {
        color: 'white',
        width: '30px',
        height: '30px'
    }

    return (
        <div className="nav-bar-container">
            <div className="nav-bar-left">
                {modes.map(mode => <div key={mode.label} className={"mode " + (mode.isActive ? 'active' : '')} onClick={()=> {if(!mode.isActive)switchAppMode()}}>{mode.label}</div>)}
            </div>
            <img className="nav-bar-logo" alt="" src={logo} />
            <div className="nav-bar-right">
                <div className="nav-bar-btn" onClick={() => onHomeClick()}>
                    <HomeIcon style={navBarIconStyle} />
                </div>
                <div className="nav-bar-btn" onClick={() => onWatchedMoviesClick()}>
                    <VisibilityIcon style={navBarIconStyle} />
                </div>
                <div className="nav-bar-btn" onClick={() => onBookmarkedMoviesClick()}>
                    <BookmarkIcon style={navBarIconStyle} />
                </div>
                <div className="nav-bar-btn" onClick={() => onGenreMenuClick()}>
                    <ViewModuleIcon style={navBarIconStyle} />
                </div>
                <div className="nav-bar-btn" onClick={() => onSearchClick()}>
                    <SearchIcon style={navBarIconStyle} />
                </div>
                <div className="nav-bar-btn" onClick={() => onTorrentLinkClick()}>
                    <LinkIcon style={navBarIconStyle} />
                </div>
            </div>
        </div>
    );
}
export default NavBar;