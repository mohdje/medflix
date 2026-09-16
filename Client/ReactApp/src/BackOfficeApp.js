import AvailableMediasVideosPage from "./pages/AvailableMediasVideosPage";
import ErrorLogsPage from "./pages/ErrorLogsPage";
import "./style/css/back-office-app.css";
import ErrorsIcon from './assets/errors.svg';
import MediaFileIcon from './assets/media_file.svg';


import { useState } from "react";

function BackOfficeApp() {
  const [selectedMenuItemId, setSelectedMenuItemId] = useState(1);

  const handleMenuItemClick = (menuItem) => {
    setSelectedMenuItemId(menuItem.id);
  };

  return <div className="back-office-app">
    <LeftMenu
      selectedMenuItemId={selectedMenuItemId}
      onMenuItemClick={handleMenuItemClick}
    />
    <main className="back-office-content">
      {selectedMenuItemId === 1 && <AvailableMediasVideosPage />}
      {selectedMenuItemId === 2 && <ErrorLogsPage />}
    </main>
  </div>
}

function LeftMenu({ selectedMenuItemId, onMenuItemClick }) {
  const menuItems = [
    { id: 1, label: "Available Medias Videos", icon: MediaFileIcon },
    { id: 2, label: "Error Logs", icon: ErrorsIcon }
  ];

  return <nav className="back-office-menu">
    {menuItems.map((menuItem) => (
      <h3
        key={menuItem.id}
        className={menuItem.id === selectedMenuItemId ? "selected menu-item" : "menu-item"}
        onClick={() => onMenuItemClick(menuItem)}
      >
        <img src={menuItem.icon} alt="" className="menu-item-icon" />
        <span>{menuItem.label}</span>
      </h3>
    ))}
  </nav>
}

export default BackOfficeApp;

