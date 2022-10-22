// import logo from './logo.svg';
import './App.css';
import MoviesAPI from "./js/moviesAPI.js";

import NavBar from "./components/navbar/NavBar";

import SplashScreen from "./components/SplashScreen";

import ModalListGenre from "./components/modal/ModalListGenre.js";

import MoviesListGenrePage from "./pages/MoviesListGenrePage";
import MoviePresentationPage from "./pages/MoviePresentationPage";
import HomePage from "./pages/HomePage";
import LastSeenMoviesPage from "./pages/LastSeenMoviesPage";
import BookmarkedMoviesPage from "./pages/BookmarkedMoviesPage";
import SettingsPage from "./pages/SettingsPage";
import ResearchPage from "./pages/ResearchPage";
import TorrentLinkPage from "./pages/TorrentLinkPage";




import Router from "./components/Router";
import { useEffect, useState } from 'react';


function App() {
  const [moviesFullListGenre, setMoviesFullListGenre] = useState('');
  const [loadFullListGenrefromCache, setLoadFullListGenrefromCache] = useState(false);

  const [loadResearchfromCache, setLoadResearchfromCache] = useState(false);

  const [centerToLastClickedBookmark, setCenterToLastClickedBookmark] = useState(false);
  const [centerToLastClickedSeenMovie, setCenterToLastClickedSeenMovie] = useState(false);

  const [splashscreenVisible, setSplashscreenVisible] = useState(true);

  const [modalListGenresVisible, setModalListGenresVisible] = useState(false);
  const [listGenres, setListGenres] = useState([]);

  const [movieId, setMovieId] = useState('');

  const showComponent = (componentId) => {
    setRouterPreviousComponentId(routerActiveComponentId);
    setRouterActiveComponentId(componentId);
  }

  const showMoviesFullListofGenre = (genre) => {
    setMoviesFullListGenre(genre);
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
    setCenterToLastClickedSeenMovie(true);
    setLoadResearchfromCache(true);
    goToPreviousComponent();
  }

  const routerIds = {
    homePage: 'homePage',
    moviesListGenrePage: 'moviesOfGenre',
    moviePresentationPage: 'moviePresentationPage',
    seenMoviesListPage: 'seenMoviesListPage',
    bookmarkedMoviesListPage: 'bookmarkedMoviesListPage',
    settings: 'settings',
    researchPage: 'researchPage',
    torrentLinkPage: 'torrentLinkPage'
  }

  const router = {
    components: [
      {
        id: routerIds.homePage,
        render: (<HomePage
          genres={listGenres}
          onMovieClick={(movieId) => showMovieFullPresentation(movieId)} />)
      },
      {
        id: routerIds.moviesListGenrePage,
        render: (<MoviesListGenrePage
          genre={moviesFullListGenre}
          loadFromCache={loadFullListGenrefromCache}
          onMovieClick={(movieId) => showMovieFullPresentation(movieId)} />)
      },
      {
        id: routerIds.moviePresentationPage,
        render: (<MoviePresentationPage
          movieId={movieId}
          onCloseClick={() => onMovieFullPresentationClose()} />)
      },
      {
        id: routerIds.seenMoviesListPage,
        render: (<LastSeenMoviesPage
          centerToLastClickedMovie={centerToLastClickedSeenMovie}
          onMoreClick={(movieId) => showMovieFullPresentation(movieId)} />)
      },
      {
        id: routerIds.bookmarkedMoviesListPage,
        render: (<BookmarkedMoviesPage
          centerToLastClickedBookmark={centerToLastClickedBookmark}
          onMoreClick={(movieId) => showMovieFullPresentation(movieId)} />)
      },
      {
        id: routerIds.settings,
        render: (<SettingsPage />)
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

  useEffect(() => {
    setTimeout(() => { setSplashscreenVisible(false) }, 4000);
    MoviesAPI.getMoviesGenres(
      (genres) => {
        if (genres && genres.length > 0) {
          setListGenres(genres);
        }
      }
    );
  }, []);

  return (
    <div className="App">
      <SplashScreen visible={splashscreenVisible} />
      <ModalListGenre
        genres={listGenres}
        visible={modalListGenresVisible}
        onGenreClick={(genre) => { setModalListGenresVisible(false); showMoviesFullListofGenre(genre) }}
        onCloseClick={() => setModalListGenresVisible(false)}
      />
      <NavBar
        onSearchClick={() => { setLoadResearchfromCache(false); setRouterActiveComponentId(routerIds.researchPage) }}
        onHomeClick={() => setRouterActiveComponentId(routerIds.homePage)}
        onGenreMenuClick={() => setModalListGenresVisible(true)}
        onLastSeenMoviesClick={() => { setCenterToLastClickedSeenMovie(false); setRouterActiveComponentId(routerIds.seenMoviesListPage) }}
        onBookmarkedMoviesClick={() => { setCenterToLastClickedBookmark(false); setRouterActiveComponentId(routerIds.bookmarkedMoviesListPage) }}
        onTorrentLinkClick={()=>{ setRouterActiveComponentId(routerIds.torrentLinkPage) }}
        onSettingsClick={() => {setRouterActiveComponentId(routerIds.settings)}} />
      <div className="app-content">
        <Router components={router.components} activeComponentId={routerActiveComponentId} />
      </div>
    </div>
  );
}

export default App;
