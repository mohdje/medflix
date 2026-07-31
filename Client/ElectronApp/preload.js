const { contextBridge, ipcRenderer } = require('electron');

contextBridge.exposeInMainWorld('api', {
  getHostAdress: () => ipcRenderer.invoke('app:getHostAdress'),
  updateHostAdress: (hostAdress) => ipcRenderer.invoke('app:updateHostAdress', hostAdress),
});
