import "../../style/nav-bar.css";
import logo from '../../assets/medflix.png';
import SearchIcon from '@material-ui/icons/Search';
import HomeIcon from '@material-ui/icons/Home';
import SettingsIcon from '@material-ui/icons/Settings';
import ViewModuleIcon from '@material-ui/icons/ViewModule';
import VisibilityIcon from '@material-ui/icons/Visibility';
import BookmarkIcon from '@material-ui/icons/Bookmark';

import MovieServicesMenu from './MovieServicesMenu';

import { useState, useRef } from 'react';
import { useOnClickOutside } from '../../js/customHooks';

function NavBar({ onSearchClick, onHomeClick, onGenreMenuClick, onLastSeenMoviesClick, onBookmarkedMoviesClick }) {

    const [showMovieServicesMenu, setShowMovieServicesMenu] = useState(false);
    const movieServicesMenuRef = useRef(null);

    const navBarIconStyle = {
        color: 'white',
        width: '30px',
        height: '30px'
    }

    useOnClickOutside(movieServicesMenuRef, () => setShowMovieServicesMenu(false));

    return (
        <div className="nav-bar-container">
            <div className="nav-bar-left">
                <div className="nav-bar-label">Demo version</div>
            </div>
            <img className="nav-bar-logo" alt="" src={logo} />
            <div className="nav-bar-right">
                <div className="nav-bar-btn" onClick={() => onHomeClick()}>
                    <HomeIcon style={navBarIconStyle} />
                </div>
                <div className="nav-bar-btn" onClick={() => onLastSeenMoviesClick()}>
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
            </div>
        </div>
    );
}
export default NavBar;