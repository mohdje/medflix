
const { exec } = require('child_process');
const fs = require('fs');

const unzipper = {
    unzip(zipFilePath, destinationFolderPath, notifier, onSuccess) {
        if (!fs.existsSync(zipFilePath)) {
            notifier.notifyMissingPackage();
        }
        else if (process.platform === 'darwin') {
            unzipFile('unzip -o -d "' + destinationFolderPath + '" "' + zipFilePath + '"', notifier, onSuccess);
        }
        else {
            unzipFile('tar -xf "' + zipFilePath + '" -C "' + destinationFolderPath + '"', notifier, onSuccess);
        }
    }
};

function unzipFile(command, notifier, onSuccess) {

    const unzipOperation = exec(command, (error, stdout, stderr) => {
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
};

module.exports = { unzipper };