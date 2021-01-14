// import logo from './logo.svg';
import './App.css';
import NavBar from "./components/NavBar";

import SplashScreen from "./components/SplashScreen";

import ModalSearch from "./components/ModalSearch";
import ModalListGenre from "./components/ModalListGenre";

import MoviesListGenreFull from "./components/MoviesListGenreFull";
import MovieFullPresentation from "./components/MovieFullPresentation";
import HomePage from "./components/HomePage";

import Router from "./components/Router";
import { useEffect, useState } from 'react';


function App() {

  const [moviesFullListGenre, setMoviesFullListGenre] = useState('');
  const [loadFullListGenrefromCache, setLoadFullListGenrefromCache] = useState(false);

  const [splashscreenVisible, setSplashscreenVisible] = useState(true);
  const [modalSearchVisible, setModalSearchVisible] = useState(false);
  const [modalListGenresVisible, setModalListGenresVisible] = useState(false);

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

  const routerIds = {
    homePage: 'homePage',
    moviesGenreFullList: 'moviesOfGenre',
    movieFullPresentation: 'movieFullPresentation'
  }

  const router = {
    components: [
      {
        id: routerIds.homePage,
        render: (<HomePage
          onMovieClick={(movieId) => showMovieFullPresentation(movieId)}
          onShowGenreFullList={(genre) => showMoviesFullListofGenre(genre)} />)
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
          onCloseClick={() => { setLoadFullListGenrefromCache(true); goToPreviousComponent(); }} />)
      }
    ]
  };
  const [routerActiveComponentId, setRouterActiveComponentId] = useState(routerIds.homePage);
  const [routerPreviousComponentId, setRouterPreviousComponentId] = useState(routerIds.homePage);

  useEffect(()=>{
    setTimeout(()=>{setSplashscreenVisible(false)}, 4000)
  }, []);

  return (
    <div className="App">
      <SplashScreen visible={splashscreenVisible}/>
      <ModalSearch
        visible={modalSearchVisible}
        onCloseClick={() => setModalSearchVisible(false)}
        onMovieClick={(movieId) => showMovieFullPresentation(movieId)} />
      <ModalListGenre
        visible={modalListGenresVisible}
        onGenreClick={(genre)=>{ setModalListGenresVisible(false); showMoviesFullListofGenre(genre)}}
        onCloseClick={() =>setModalListGenresVisible(false)}
      />
      <NavBar
        onSearchClick={() => setModalSearchVisible(true)}
        onHomeClick={() => setRouterActiveComponentId(routerIds.homePage)} 
        onGenreMenuClick={()=> setModalListGenresVisible(true)}/>
      <div className="app-content" >
        <Router components={router.components} activeComponentId={routerActiveComponentId} />
      </div>
    </div>
  );
}

export default App;
