const { contextBridge, ipcRenderer } = require('electron')

contextBridge.exposeInMainWorld('electronAPI', {
  confirmUpdateApp: () => ipcRenderer.send('confirm-update-app'),
  cancelUpdateApp: () => ipcRenderer.send('cancel-update-app'),
  onUpdateDownloaded: (callback) => ipcRenderer.on('update-downloaded', callback),
})