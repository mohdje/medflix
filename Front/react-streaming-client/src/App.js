import './App.css';
import NavBar from "./components/navbar/NavBar";
import SplashScreen from "./components/SplashScreen";
import ModalListGenre from "./components/modal/ModalListGenre.js";

import MoviesListGenrePage from "./pages/MoviesListGenrePage";
import MoviePresentationPage from "./pages/MoviePresentationPage";
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
  const [centerToLastClickedWatchedMovie, setCenterToLastClickedWatchedMovie] = useState(false);

  const [splashscreenVisible, setSplashscreenVisible] = useState(true);
  const [homePageFailed, setHomePageFailed] = useState(false);

  const [modalListGenresVisible, setModalListGenresVisible] = useState(false);

  const [movieId, setMovieId] = useState('');

  const showComponent = (componentId) => {
    setRouterPreviousComponentId(routerActiveComponentId);
    setRouterActiveComponentId(componentId);
  }

  const showMoviesFullListofGenre = (genre) => {
    setSelectedGenre(genre);
    setLoadFullListGenrefromCache(false);
    showComponent(routerIds.moviesListGenrePage);
  }

  const showMovieFullPresentation = (movieId) => {
    setMovieId(movieId);
    showComponent(routerIds.moviePresentationPage);
  }

  const goToPreviousComponent = () => {
    showComponent(routerPreviousComponentId);
  }

  const onMovieFullPresentationClose = () => {
    setLoadFullListGenrefromCache(true);
    setCenterToLastClickedBookmark(true);
    setCenterToLastClickedWatchedMovie(true);
    setLoadResearchfromCache(true);
    goToPreviousComponent();
  }

  const routerIds = {
    homePage: 'homePage',
    moviesListGenrePage: 'moviesOfGenre',
    moviePresentationPage: 'moviePresentationPage',
    watchedMoviesListPage: 'watchedMoviesListPage',
    bookmarkedMoviesListPage: 'bookmarkedMoviesListPage',
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
          onMediaClick={(mediaId) => showMovieFullPresentation(mediaId)}/>)
      },
      {
        id: routerIds.moviesListGenrePage,
        render: (<MoviesListGenrePage
          genre={selectedGenre}
          loadFromCache={loadFullListGenrefromCache}
          onMovieClick={(movieId) => showMovieFullPresentation(movieId)} />),
        containerStyle: { height: '100%'}
      },
      {
        id: routerIds.moviePresentationPage,
        render: (<MoviePresentationPage
          movieId={movieId}
          onCloseClick={() => onMovieFullPresentationClose()} />)
      },
      {
        id: routerIds.watchedMoviesListPage,
        render: (<WatchedMediasPage
          centerToLastClickedMedia={centerToLastClickedWatchedMovie}
          onMediaClick={(mediaId) => showMovieFullPresentation(mediaId)} />)
      },
      {
        id: routerIds.bookmarkedMoviesListPage,
        render: (<BookmarkedMediasPage
          centerToLastClickedMedia={centerToLastClickedBookmark}
          onMediaClick={(mediaId) => showMovieFullPresentation(mediaId)} />)
      },
      {
        id: routerIds.researchPage,
        render: (<ResearchPage loadFromCache={loadResearchfromCache}  onMovieClick={(movieId) => showMovieFullPresentation(movieId)}/>)
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
        onGenreClick={(genre) => { setModalListGenresVisible(false); showMoviesFullListofGenre(genre) }}
        onCloseClick={() => setModalListGenresVisible(false)}
      />
      <NavBar
        onSearchClick={() => { setLoadResearchfromCache(false); setRouterActiveComponentId(routerIds.researchPage) }}
        onHomeClick={() => setRouterActiveComponentId(routerIds.homePage)}
        onGenreMenuClick={() => setModalListGenresVisible(true)}
        onWatchedMoviesClick={() => { setCenterToLastClickedWatchedMovie(false); setRouterActiveComponentId(routerIds.watchedMoviesListPage) }}
        onBookmarkedMoviesClick={() => { setCenterToLastClickedBookmark(false); setRouterActiveComponentId(routerIds.bookmarkedMoviesListPage) }}
        onTorrentLinkClick={()=>{ setRouterActiveComponentId(routerIds.torrentLinkPage) }}
        onAppModeSwitch={()=> {setRouterActiveComponentId(routerIds.homePage)}}/>
      <div className="app-content">
        <Router components={router.components} activeComponentId={routerActiveComponentId} />
      </div>
    </div>
  );
}

export default App;
