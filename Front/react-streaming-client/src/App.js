// import logo from './logo.svg';
import './App.css';
import MoviesAPI from "./js/moviesAPI.js";

import NavBar from "./components/navbar/NavBar";

import SplashScreen from "./components/SplashScreen";

import ModalSearch from "./components/modal/ModalSearch";
import ModalListGenre from "./components/modal/ModalListGenre.js";

import MoviesListGenreFull from "./components/movies/list/MoviesListGenreFull";
import MovieFullPresentation from "./pages/MoviePresentationPage";
import HomePage from "./components/HomePage";
import LastSeenMoviesView from "./components/movies/views/LastSeenMoviesView";
import BookmarkedMoviesView from "./components/movies/views/BookmarkedMoviesView";


import Router from "./components/Router";
import { useEffect, useState } from 'react';


function App() {

  const [moviesFullListGenre, setMoviesFullListGenre] = useState('');
  const [loadFullListGenrefromCache, setLoadFullListGenrefromCache] = useState(false);

  const [centerToLastClickedBookmark, setCenterToLastClickedBookmark] = useState(false);
  const [centerToLastClickedSeenMovie, setCenterToLastClickedSeenMovie] = useState(false);

  const [splashscreenVisible, setSplashscreenVisible] = useState(true);
  const [modalSearchVisible, setModalSearchVisible] = useState(false);

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
    showComponent(routerIds.moviesGenreFullList);
  }

  const showMovieFullPresentation = (movieId) => {
    setModalSearchVisible(false);
    setMovieId(movieId);
    showComponent(routerIds.movieFullPresentation);
  }

  const goToPreviousComponent = () => {
    showComponent(routerPreviousComponentId);
  }

  const onMovieFullPresentationClose = () => {
    setLoadFullListGenrefromCache(true);
    setCenterToLastClickedBookmark(true);
    setCenterToLastClickedSeenMovie(true);
    goToPreviousComponent();
  }

  const routerIds = {
    homePage: 'homePage',
    moviesGenreFullList: 'moviesOfGenre',
    movieFullPresentation: 'movieFullPresentation',
    seenMoviesList: 'seenMoviesList',
    bookmarkedMoviesList: 'bookmarkedMoviesList'
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
        id: routerIds.moviesGenreFullList,
        render: (<MoviesListGenreFull
          genre={moviesFullListGenre}
          loadFromCache={loadFullListGenrefromCache}
          onMovieClick={(movieId) => showMovieFullPresentation(movieId)} />)
      },
      {
        id: routerIds.movieFullPresentation,
        render: (<MovieFullPresentation
          movieId={movieId}
          onCloseClick={() => onMovieFullPresentationClose()} />)
      },
      {
        id: routerIds.seenMoviesList,
        render: (<LastSeenMoviesView
          centerToLastClickedMovie={centerToLastClickedSeenMovie}
          onMoreClick={(movieId) => showMovieFullPresentation(movieId)} />)
      },
      {
        id: routerIds.bookmarkedMoviesList,
        render: (<BookmarkedMoviesView
          centerToLastClickedBookmark={centerToLastClickedBookmark}
          onMoreClick={(movieId) => showMovieFullPresentation(movieId)} />)
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
      <ModalSearch
        visible={modalSearchVisible}
        onCloseClick={() => setModalSearchVisible(false)}
        onMovieClick={(movieId) => showMovieFullPresentation(movieId)} />
      <ModalListGenre
        genres={listGenres}
        visible={modalListGenresVisible}
        onGenreClick={(genre) => { setModalListGenresVisible(false); showMoviesFullListofGenre(genre) }}
        onCloseClick={() => setModalListGenresVisible(false)}
      />
      <NavBar
        onSearchClick={() => setModalSearchVisible(true)}
        onHomeClick={() => setRouterActiveComponentId(routerIds.homePage)}
        onGenreMenuClick={() => setModalListGenresVisible(true)}
        onLastSeenMoviesClick={() => { setCenterToLastClickedSeenMovie(false); setRouterActiveComponentId(routerIds.seenMoviesList) }}
        onBookmarkedMoviesClick={() => { setCenterToLastClickedBookmark(false); setRouterActiveComponentId(routerIds.bookmarkedMoviesList) }} />
      <div className="app-content" >
        <Router components={router.components} activeComponentId={routerActiveComponentId} />
      </div>
    </div>
  );
}

export default App;
