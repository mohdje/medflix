import "../../style/nav-bar.css";
import logo from '../../assets/medflix.png';
import SearchIcon from '@material-ui/icons/Search';
import HomeIcon from '@material-ui/icons/Home';
import SettingsIcon from '@material-ui/icons/Settings';
import ViewModuleIcon from '@material-ui/icons/ViewModule';
import VisibilityIcon from '@material-ui/icons/Visibility';
import BookmarkIcon from '@material-ui/icons/Bookmark';

function NavBar({ onSearchClick, onHomeClick, onGenreMenuClick, onLastSeenMoviesClick, onBookmarkedMoviesClick, onSettingsClick }) {

    const navBarIconStyle = {
        color: 'white',
        width: '30px',
        height: '30px'
    }

    return (
        <div className="nav-bar-container">
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
                <div className="nav-bar-btn" onClick={() => onSettingsClick()}>
                    <SettingsIcon style={navBarIconStyle} />
                </div>
            </div>
        </div>
    );
}
export default NavBar;