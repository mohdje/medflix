
const notifier = {
	window: null,
	app: null,
	init(app, window) {
		this.app = app;
		this.window = window;
	},
	notifyError() {
		notifyUI(this.app, this.window, 'An error occured while extracting package. Operation aborted.');
	},
	notifyMissingPackage() {
		notifyUI(this.app, this.window, 'Package to extract not found. Operation aborted.');
	},
	notifySuccess(callback) {
		notifyUI(this.app, this.window, 'Extracting package successfully done. The application will be launched.', callback);
	}
}

const notifyUI = (app, window, message, callback) => {
	window.webContents.send('decompress-state-changed', message);
	setTimeout(() => {
		if (callback) callback();
		app.quit();
	}, 3000);
}

module.exports = { notifier };