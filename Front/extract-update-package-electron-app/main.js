const { app, BrowserWindow, shell } = require('electron');
const path = require('path');
const fs = require('fs');
const DecompressZip = require('decompress-zip');
const { exec } = require('child_process');
const { icon } = require('./consts');

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

const decompressZipFolder = (window, zipFilePath, destinationFolderPath, onSuccess) => {
	
	if (!fs.existsSync(zipFilePath)) {
		window.webContents.send('decompress-state-changed', 'Package to extract not found. Operation aborted.');
		setTimeout(() => {
			app.quit();
		}, 3000);
	} else {
		const unzipper = new DecompressZip(zipFilePath);

		unzipper.on('error', function (err) {
			window.webContents.send('decompress-state-changed', 'An error occured while extracting package. Operation aborted.');
			setTimeout(() => {
				app.quit();
			}, 3000);
		});

		unzipper.on('progress', function (fileIndex, fileCount) {
			const percentage = ((fileIndex + 1) / fileCount) * 100;
			window.webContents.send('decompress-state-changed', `Extracting package in application folder, please wait... (${percentage.toFixed(1)} %)`);
		});

		// Notify when everything is extracted
		unzipper.on('extract', function (log) {
			window.webContents.send('decompress-state-changed', 'Extracting package successfully done. The application will be launched.');
			setTimeout(() => {
				onSuccess();
				app.quit();
			}, 3000);
		});

		// Start extraction of the content
		unzipper.extract({
			path: destinationFolderPath
			// You can filter the files that you want to unpack using the filter option
			//filter: function (file) {
			//console.log(file);
			//return file.type !== "SymbolicLink";
			//}
		});
	}
}

app.whenReady().then(() => {
	const window = createWindow();

	const zipFilePath = process.argv[1];
	const destinationFolderPath = process.argv[2];
	
	setTimeout(() => 
		decompressZipFolder(window, zipFilePath, destinationFolderPath, () => {
			if (process.platform === 'darwin') {
				exec('open -a Medflix');
			}
			else{
				const medflixApplicationPath = process.argv[3];
				shell.openExternal(medflixApplicationPath);
			}
	}), 2000);
})

