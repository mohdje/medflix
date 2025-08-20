import './style/css/App.css';
import NavBar from "./components/navbar/NavBar";
import SplashScreen from "./components/SplashScreen";
import ModalCategories from "./components/modal/ModalCategories.js";
import { AppPagesManager, PageIds } from './pages/AppPagesManager.js';

import { useState } from 'react';

function App() {
  const [splashscreenVisible, setSplashscreenVisible] = useState(true);
  const [homePageFailed, setHomePageFailed] = useState(false);

  const [modalListCategoriesVisible, setModalListCategoriesVisible] = useState(false);

  const [activePage, setActivePage] = useState({ id: PageIds.homePage });

  return (
    <>
      <SplashScreen visible={splashscreenVisible} showErrorMessage={homePageFailed} />
      <div className="App">
        <NavBar
          onSearchClick={() => setActivePage({ id: PageIds.researchPage })}
          onHomeClick={() => setActivePage({ id: PageIds.homePage })}
          onGenreMenuClick={() => setModalListCategoriesVisible(true)}
          onWatchedMediasClick={() => setActivePage({ id: PageIds.watchedMediasListPage })}
          onBookmarkedMediasClick={() => setActivePage({ id: PageIds.bookmarkedMediasListPage })}
          onAppModeSwitch={() => setActivePage({ id: PageIds.homePage })} />
        <ModalCategories
          visible={modalListCategoriesVisible}
          onGenreClick={(genre) => {
            setModalListCategoriesVisible(false);
            setActivePage({
              id: PageIds.mediasListOfCategoriePage,
              parameter: { categorie: genre, type: "genre" }
            })
          }}
          onPlatformClick={(platform) => {
            setModalListCategoriesVisible(false);
            setActivePage({
              id: PageIds.mediasListOfCategoriePage,
              parameter: { categorie: platform, type: "platform" }
            })
          }}
          onCloseClick={() => setModalListCategoriesVisible(false)}
        />

        <AppPagesManager
          activePage={activePage}
          onHomeReady={() => setSplashscreenVisible(false)}
          onHomeFail={() => setHomePageFailed(true)} />
      </div>
    </>
  );
}

export default App;
