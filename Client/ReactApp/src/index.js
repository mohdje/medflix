import React from 'react';
import ReactDOM from 'react-dom/client';
import './style/css/index.css';

const applicationLoaders = {
  consumer: () => import('./App.js'),
  backoffice: () => import('./BackOfficeApp.js')
};

const selectedApplication = process.env.REACT_APP_APP_MODE || 'consumer';
const loadApplication = applicationLoaders[selectedApplication] || applicationLoaders.consumer;
const App = React.lazy(loadApplication);

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <React.Suspense fallback={<div>Loading application...</div>}>
    <App />
  </React.Suspense>
);

