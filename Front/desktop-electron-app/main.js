const { app, BrowserWindow, screen, net, ipcMain } = require('electron');
const path = require('path');
const { checkUpdate, downloadUpdate } = require('./update.js');
const { backendUrl, icon } = require('./consts.js');

const createAppWindow = () => {
	let screenSize = screen.getPrimaryDisplay().size;
	const window = new BrowserWindow({
		minWidth: screenSize.width * 0.7,
		minHeight: screenSize.height * 0.7,
		autoHideMenuBar: true,
		icon: path.join(__dirname, icon),
		webPreferences: {
			preload: path.join(__dirname, 'preload.js'),
			enableRemoteModule: true,
			nodeIntegration: false,
		},
	})

	window.maximize();
	window.loadFile('views/app/index.html');

	return window;
}

const createUpdateWindow = () => {
	const window = new BrowserWindow({
		width: 600,
		height: 300,
		frame: false,
		resizable: false,
		parent: app.mainWindow, 
		modal: true,
		icon: path.join(__dirname, 'views/app/favicon.ico'),
		webPreferences: {
			preload: path.join(__dirname, 'preload.js'),
			enableRemoteModule: true,
			nodeIntegration: false,
		},
	});

	ipcMain.on('confirm-update-app', () => {
		const onDownloadSuccess = () => {
			window.webContents.send('update-downloaded', true);
			setTimeout(() => {
				const request = net.request(backendUrl + 'startUpdate');
				request.end();
				app.quit();
			}, 4000);
		}

		const onDownloadFail = () => {
			window.webContents.send('update-downloaded', false);
			setTimeout(() => {
				window.close();
			}, 4000);
		}
		downloadUpdate(onDownloadSuccess, onDownloadFail);
	});

	ipcMain.on('cancel-update-app', () => {
		window.close();
	})


	window.loadFile('views/update/index.html');
}

app.whenReady().then(() => {
	app.mainWindow = createAppWindow();
	setTimeout(() => checkUpdate(createUpdateWindow), 2000);
})

app.on('window-all-closed', () => {
	const request = net.request(backendUrl + 'stop');
	request.end();
	app.quit();
})
