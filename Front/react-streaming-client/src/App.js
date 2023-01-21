import './App.css';
import NavBar from "./components/navbar/NavBar";
import SplashScreen from "./components/SplashScreen";
import ModalListGenre from "./components/modal/ModalListGenre.js";

import MediaListGenrePage from "./pages/MediaListGenrePage";
import MediaPresentationPage from "./pages/MediaPresentationPage";
import HomePage from "./pages/HomePage";
import WatchedMediasPage from "./pages/WatchedMediasPage";
import BookmarkedMediasPage from "./pages/BookmarkedMediasPage";
import ResearchPage from "./pages/ResearchPage";
import TorrentLinkPage from "./pages/TorrentLinkPage";

import Router from "./components/Router";
import { useState } from 'react';


function App() {
  const [selectedGenre, setSelectedGenre] = useState(null);
  const [loadFullListGenrefromCache, setLoadFullListGenrefromCache] = useState(false);

  const [loadResearchfromCache, setLoadResearchfromCache] = useState(false);

  const [centerToLastClickedBookmark, setCenterToLastClickedBookmark] = useState(false);
  const [centerToLastClickedWatchedMedia, setCenterToLastClickedWatchedMedia] = useState(false);

  const [splashscreenVisible, setSplashscreenVisible] = useState(true);
  const [homePageFailed, setHomePageFailed] = useState(false);

  const [modalListGenresVisible, setModalListGenresVisible] = useState(false);

  const [mediaId, setMediaId] = useState('');

  const showComponent = (componentId) => {
    setRouterPreviousComponentId(routerActiveComponentId);
    setRouterActiveComponentId(componentId);
  }

  const showMediasFullListofGenre = (genre) => {
    setSelectedGenre(genre);
    setLoadFullListGenrefromCache(false);
    showComponent(routerIds.mediasListGenrePage);
  }

  const showMediaFullPresentation = (mediaId) => {
    setMediaId(mediaId);
    showComponent(routerIds.mediaPresentationPage);
  }

  const goToPreviousComponent = () => {
    showComponent(routerPreviousComponentId);
  }

  const onMediaFullPresentationClose = () => {
    setLoadFullListGenrefromCache(true);
    setCenterToLastClickedBookmark(true);
    setCenterToLastClickedWatchedMedia(true);
    setLoadResearchfromCache(true);
    goToPreviousComponent();
  }

  const routerIds = {
    homePage: 'homePage',
    mediasListGenrePage: 'mediasOfGenre',
    mediaPresentationPage: 'mediaPresentationPage',
    watchedMediasListPage: 'watchedMediasListPage',
    bookmarkedMediasListPage: 'bookmarkedMediasListPage',
    researchPage: 'researchPage',
    torrentLinkPage: 'torrentLinkPage'
  }

  const router = {
    components: [
      {
        id: routerIds.homePage,
        render: (<HomePage
          onReady={() => { setSplashscreenVisible(false) }}
          onFail={() => { setHomePageFailed(true)}}
          onMediaClick={(mediaId) => showMediaFullPresentation(mediaId)}/>)
      },
      {
        id: routerIds.mediasListGenrePage,
        render: (<MediaListGenrePage
          genre={selectedGenre}
          loadFromCache={loadFullListGenrefromCache}
          onMediaClick={(mediaId) => showMediaFullPresentation(mediaId)} />),
        containerStyle: { height: '100%'}
      },
      {
        id: routerIds.mediaPresentationPage,
        render: (<MediaPresentationPage
          mediaId={mediaId}
          onCloseClick={() => onMediaFullPresentationClose()} />)
      },
      {
        id: routerIds.watchedMediasListPage,
        render: (<WatchedMediasPage
          centerToLastClickedMedia={centerToLastClickedWatchedMedia}
          onMediaClick={(mediaId) => showMediaFullPresentation(mediaId)} />)
      },
      {
        id: routerIds.bookmarkedMediasListPage,
        render: (<BookmarkedMediasPage
          centerToLastClickedMedia={centerToLastClickedBookmark}
          onMediaClick={(mediaId) => showMediaFullPresentation(mediaId)} />)
      },
      {
        id: routerIds.researchPage,
        render: (<ResearchPage loadFromCache={loadResearchfromCache}  onMediaClick={(mediaId) => showMediaFullPresentation(mediaId)}/>)
      },
      {
        id: routerIds.torrentLinkPage,
        render: (<TorrentLinkPage />)
      }
    ]
  };
  const [routerActiveComponentId, setRouterActiveComponentId] = useState(routerIds.homePage);
  const [routerPreviousComponentId, setRouterPreviousComponentId] = useState(routerIds.homePage);

  return (
    <div className="App">
      <SplashScreen visible={splashscreenVisible} showErrorMessage={homePageFailed}/>
      <ModalListGenre
        visible={modalListGenresVisible}
        onGenreClick={(genre) => { setModalListGenresVisible(false); showMediasFullListofGenre(genre) }}
        onCloseClick={() => setModalListGenresVisible(false)}
      />
      <NavBar
        onSearchClick={() => { setLoadResearchfromCache(false); setRouterActiveComponentId(routerIds.researchPage) }}
        onHomeClick={() => setRouterActiveComponentId(routerIds.homePage)}
        onGenreMenuClick={() => setModalListGenresVisible(true)}
        onWatchedMediasClick={() => { setCenterToLastClickedWatchedMedia(false); setRouterActiveComponentId(routerIds.watchedMediasListPage) }}
        onBookmarkedMediasClick={() => { setCenterToLastClickedBookmark(false); setRouterActiveComponentId(routerIds.bookmarkedMediasListPage) }}
        onTorrentLinkClick={()=>{ setRouterActiveComponentId(routerIds.torrentLinkPage) }}
        onAppModeSwitch={()=> {setRouterActiveComponentId(routerIds.homePage)}}/>
      <div className="app-content">
        <Router components={router.components} activeComponentId={routerActiveComponentId} />
      </div>
    </div>
  );
}

export default App;
