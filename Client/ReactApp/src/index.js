import React from 'react';
import ReactDOM from 'react-dom/client';
import './style/css/index.css';
import logoFull from './assets/logo_full.svg';

const applicationLoaders = {
  consumer: () => import('./App.js'),
  backoffice: () => import('./BackOfficeApp.js')
};

const selectedApplication = process.env.REACT_APP_APP_MODE || 'consumer';
const loadApplication = applicationLoaders[selectedApplication] || applicationLoaders.consumer;
const App = React.lazy(loadApplication);

const root = ReactDOM.createRoot(document.getElementById('root'));
const LoadingFallback = () => (
  <div
    style={{
      alignItems: 'center',
      backgroundColor: '#000',
      display: 'flex',
      height: '100vh',
      justifyContent: 'center',
      width: '100vw'
    }}
  >
    <img src={logoFull} style={{ width: '60%', maxWidth: '300px', height: 'auto' }} alt="Medflix" />
  </div>
);

root.render(
  <React.Suspense fallback={<LoadingFallback />}>
    <App />
  </React.Suspense>
);

