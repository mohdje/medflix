const { app, BrowserWindow, ipcMain, screen } = require('electron');
const { launchVLC } = require('./vlc.js');
const { getHostAdress, saveHostAdress, loadConfig } = require('./config.js');
const path = require('path');

function createWindow() {
  const { width: screenWidth, height: screenHeight } = screen.getPrimaryDisplay().workAreaSize;
  const width = Math.round(screenWidth * 0.9);
  const height = Math.round(screenHeight * 0.95);

  const win = new BrowserWindow({
    width,
    height,
    autoHideMenuBar: true,
    webPreferences: {
      preload: path.join(__dirname, 'preload.js'),
      contextIsolation: true,
      nodeIntegration: false,
    },
  });

  win.removeMenu();

  win.webContents.on('will-navigate', (event, url) => {
    if (url === `http://${getHostAdress()}/home/index.html`)
      return;

    event.preventDefault();
    if (url === "http://playmedia/") {
      win.webContents.executeJavaScript("window.getMediaPlayerParameters()").then(async result => {
        await launchVLC(win, result, () => win.webContents.executeJavaScript("window.closeNativeMediaPlayer()"));
      }).catch(err => {
        console.error('Execution failed:', err);
        win.webContents.executeJavaScript("window.closeNativeMediaPlayer()");
      })
    }
  });

  //win.webContents.openDevTools()
  win.loadFile(path.join(__dirname, "index.html"));

  return win;
}

app.whenReady().then(async () => {
  await loadConfig();

  ipcMain.handle('app:getHostAdress', () => getHostAdress());
  ipcMain.handle('app:updateHostAdress', async (e, hostAdress) => {
    await saveHostAdress(hostAdress);
  });

  const window = createWindow();

  app.on('activate', () => {
    if (BrowserWindow.getAllWindows().length === 0) {
      createWindow();
    }
  });
});

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    app.quit();
  }
});


