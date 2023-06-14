const { net } = require('electron');
const { backendUrl } = require('./consts.js');

function checkUpdate(onUpdateAvailableCallback) {
	const request = net.request(backendUrl + 'checkUpdate');
	request.on('response', (response) => {
		if (response.statusCode === 200) {
			const data = [];
			response.on('end', () => {
				var result = JSON.parse(data.join(''))
				if (result.updateAvailable && onUpdateAvailableCallback) {
					onUpdateAvailableCallback();
				}
			});
			response.on('data', (chunk) => {
				data.push(chunk);
			});
		}
	});
	request.end();
}

function downloadUpdate (onSuccess, onFail){
	const request = net.request(backendUrl + 'downloadNewVersion');
	request.on('response', (response) => {
		if (response.statusCode === 200) {
			onSuccess();
		}
		else {
			onFail()
		}
	});
	request.end();
}

//in async function: await sleep(2000);
const sleep = (millis) => {
	return new Promise(resolve => setTimeout(resolve, millis));
}

module.exports = { checkUpdate, downloadUpdate }