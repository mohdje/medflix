const { app, BrowserWindow, screen, net } = require('electron')
const path = require('path');

const createWindow = () => {
    let screenSize = screen.getPrimaryDisplay().size;
    const win = new BrowserWindow({
        minWidth: screenSize.width * 0.7,
        minHeight: screenSize.height * 0.7,
        autoHideMenuBar: true,
        icon: path.join(__dirname, 'view/favicon.ico'),
        webPreferences: {
            preload: path.join(__dirname, 'preload.js'),
            enableRemoteModule: true,
            nodeIntegration: false,
        },
         
    })
 
    win.maximize();
    win.loadFile('view/index.html');
    // win.webContents.openDevTools();
}

app.whenReady().then(() => {
    createWindow();
})

app.on('window-all-closed', () => {
    const request = net.request('http://localhost:5000/application/stop');
    request.end();
    app.quit();
})
