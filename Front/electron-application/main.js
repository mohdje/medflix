const electron = require('electron');
const app = electron.app;
const BrowserWindow = electron.BrowserWindow;
const path = require('path');
const mainTitleBar = require('custom-electron-titlebar/main');

mainTitleBar.setupTitlebar();

function createWindow () {
    let screenSize = electron.screen.getPrimaryDisplay().size;
    mainWindow = new BrowserWindow({
      width: screenSize.width * 0.8,
      height: screenSize.height * 0.8,
      minWidth: screenSize.width * 0.7,
      minHeight: screenSize.height * 0.7,
      frame: false,
      icon: path.join(__dirname, 'view/favicon.ico'),
      webPreferences: {
        preload: path.join(__dirname, 'preload.js'),
        enableRemoteModule: true,
        nodeIntegration: false,
      },
      
    });
    mainWindow.maximize();
    mainWindow.loadFile('view/index.html');
    mainTitleBar.attachTitlebarToWindow(mainWindow);
    
   // mainWindow.webContents.openDevTools();
}

app.whenReady().then(() => {
  createWindow()

  app.on('activate', () => {
    if (BrowserWindow.getAllWindows().length === 0) {
      createWindow()
    }
  })
})

app.on('window-all-closed', () => {
    app.quit()
})
