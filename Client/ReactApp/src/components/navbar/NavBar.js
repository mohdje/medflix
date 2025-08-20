import "../../style/css/nav-bar.css";
import logo from '../../assets/logo_full.svg';
import SearchIcon from '../../assets/search.svg';
import HomeIcon from '../../assets/home.svg';
import CategoriesIcon from '../../assets/categories.svg';
import EyeIcon from '../../assets/eye.svg';
import BookmarkIcon from '../../assets/bookmark.svg';
import LinkIcon from '../../assets/link.svg';
import BurgerMenuIcon from '../../assets/burger_menu.svg';

import { useOnClickOutside } from '../../helpers/customHooks';
import { useState, useRef } from 'react';
import AppMode from '../../services/appMode';

function NavBar({ onSearchClick, onHomeClick, onGenreMenuClick, onWatchedMediasClick, onBookmarkedMediasClick, onAppModeSwitch }) {
    const [showSideMenu, setShowSideMenu] = useState(false);
    const [showDropDownModeMenu, setShowDropDownModeMenu] = useState(false);
    const dropdownRef = useRef(null);
    const [renderKey, setRenderKey] = useState(0);

    useOnClickOutside(dropdownRef, () => setShowDropDownModeMenu(false));

    const menus = [
        {
            label: "Home",
            icon: HomeIcon,
            onClick: () => onHomeClick()
        },
        {
            label: "Search",
            icon: SearchIcon,
            onClick: () => onSearchClick()
        },
        {
            label: "Watch history",
            icon: EyeIcon,
            onClick: () => onWatchedMediasClick()
        },
        {
            label: "Bookmark list",
            icon: BookmarkIcon,
            onClick: () => onBookmarkedMediasClick()
        },
        {
            label: "Categories",
            icon: CategoriesIcon,
            onClick: () => onGenreMenuClick()
        }
    ]

    const onAppModeClick = (selectedMode) => {
        setShowDropDownModeMenu(false);
        if (!selectedMode.isActive) {
            AppMode.switchActiveMode();
            onAppModeSwitch(AppMode.getActiveMode());
            setRenderKey(prevKey => prevKey + 1);// to refresh list of app modes (active or inactive) displayed in desktop-menu div
        }
    }

    return (
        <div className="nav-bar-container">
            <div className="nav-bar-left">
                <div className="desktop-menu">
                    {AppMode.modes.map(mode => <h3 key={mode.label} className={"mode " + (mode.isActive ? 'active' : '')} onClick={() => onAppModeClick(mode)}>{mode.label}</h3>)}
                </div>
                <div className="mobile-menu">
                    <div className="nav-bar-btn" onClick={() => setShowDropDownModeMenu(!showDropDownModeMenu)}>
                        <img alt={AppMode.getActiveMode().label} src={AppMode.getActiveMode().logo}></img>
                    </div>
                    <div ref={dropdownRef} className={`drop-down-menu ${showDropDownModeMenu ? 'show' : 'hide'}`}>
                        {AppMode.modes.map(mode => <MenuItem label={mode.label} key={mode.label} icon={mode.logo} onClick={() => onAppModeClick(mode)} />
                        )}
                    </div>
                </div>
            </div>
            <div className="nav-bar-middle">
                <img className="nav-bar-logo" alt="logo" src={logo} />
            </div>
            <div className="nav-bar-right">
                <div className="desktop-menu">
                    {menus.map(menu =>
                        <div className="nav-bar-btn" key={menu.label} onClick={() => menu.onClick()}>
                            <img alt={menu.label} src={menu.icon}></img>
                        </div>
                    )}
                </div>
                <div className="mobile-menu">
                    <div className="nav-bar-btn" onClick={() => setShowSideMenu(true)}>
                        <img alt="menu" src={BurgerMenuIcon}></img>
                    </div>
                    <div className={`slide-right-menu-container ${showSideMenu ? 'show' : 'hide'}`} onClick={() => setShowSideMenu(false)}>
                        <div className="slide-right-menu">
                            {menus.map(menu =>
                                <MenuItem label={menu.label} key={menu.label} icon={menu.icon} onClick={() => menu.onClick()} />
                            )}
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}
export default NavBar;

function MenuItem({ label, icon, onClick }) {
    return <div className="menu-item" key={label} onClick={() => onClick()}>
        <img alt={label} src={icon}></img>
        <h3>{label}</h3>
    </div>
}