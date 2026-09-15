import AvailableMediasVideosPage from "./pages/AvailableMediasVideosPage";
import "./style/css/back-office-app.css";
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
    </main>
  </div>
}

function LeftMenu({ selectedMenuItemId, onMenuItemClick }) {
  const menuItems = [
    { id: 1, label: "Available Medias Videos" },
    { id: 2, label: "Errors" }
  ];

  return <nav className="back-office-menu">
    {menuItems.map((menuItem) => (
      <h3
        key={menuItem.id}
        className={menuItem.id === selectedMenuItemId ? "selected" : ""}
        onClick={() => onMenuItemClick(menuItem)}
      >
        {menuItem.label}
      </h3>
    ))}
  </nav>
}

export default BackOfficeApp;

