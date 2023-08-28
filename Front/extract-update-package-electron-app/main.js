const { app, BrowserWindow, shell } = require('electron');
const path = require('path');
const { exec } = require('child_process');
const { icon } = require('./consts');
const { notifier } = require('./notifier');
const { unzipper } = require('./unzipper');

const createWindow = () => {
	const window = new BrowserWindow({
		width: 600,
		height: 300,
		frame: false,
		resizable: false,
		icon: path.join(__dirname, icon),
		webPreferences: {
			preload: path.join(__dirname, 'preload.js'),
			enableRemoteModule: true,
			nodeIntegration: false,
		},
	});

	window.loadFile('view/index.html');

	return window;
}

app.whenReady().then(() => {
	const window = createWindow();

	notifier.init(app, window);

	const zipFilePath = process.argv[1];
	const destinationFolderPath = process.argv[2];

	setTimeout(() =>
		unzipper.unzip(zipFilePath, destinationFolderPath, notifier, () => {
			if (process.platform === 'darwin') {
				exec('open -a Medflix');
			}
			else {
				const medflixApplicationPath = process.argv[3];
				shell.openExternal(medflixApplicationPath);
			}
		}), 2000);
})

