const { app, BrowserWindow, shell } = require('electron');
const path = require('path');
const fs = require('fs');
const { exec } = require('child_process');
const { icon } = require('./consts');
const { notifier } = require('./notifier');

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

const decompressZipFolder = (notifier, zipFilePath, destinationFolderPath, onSuccess) => {

	if (!fs.existsSync(zipFilePath)) {
		notifier.notifyMissingPackage();
	} else {
		if (process.platform === 'darwin') {
			const unzipOperation = exec('unzip -o -d "' + destinationFolderPath + '" "' + zipFilePath + '"', (error, stdout, stderr) => {
				if (error || stderr) {
					notifier.notifyError();
				}
			});

			unzipOperation.on('exit', function (code) {
				if (code === 0) {
					notifier.notifySuccess(onSuccess);
				}
				else {
					notifier.notifyError();
				}
			});
		}
	}
}

app.whenReady().then(() => {
	const window = createWindow();

	notifier.init(app, window);

	const zipFilePath = process.argv[1];
	const destinationFolderPath = process.argv[2];

	setTimeout(() =>
		decompressZipFolder(notifier, zipFilePath, destinationFolderPath, () => {
			if (process.platform === 'darwin') {
				exec('open -a Medflix');
			}
			else {
				const medflixApplicationPath = process.argv[3];
				shell.openExternal(medflixApplicationPath);
			}
		}), 2000);
})

