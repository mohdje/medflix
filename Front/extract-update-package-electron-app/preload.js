const { contextBridge, ipcRenderer } = require('electron')

contextBridge.exposeInMainWorld('electronAPI', {
  onDecompressStateChanged: (callback) => ipcRenderer.on('decompress-state-changed', callback)
})