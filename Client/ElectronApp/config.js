const { app } = require('electron');
const fs = require('fs')
const path = require('path');

const configDir = app.getPath('userData');
const configFile = path.join(configDir, "config.json");

let config = {
    hostAdress: "",
    medflixClientId: ""
};

async function loadConfig() {
    if (fs.existsSync(configFile)) {
        const configJson = await fs.promises.readFile(configFile, 'utf-8');
        config = JSON.parse(configJson);
    }
}

function getHostAdress() {
    return config?.hostAdress;
}

function getCliendId() {
    return config?.medflixClientId;
}

async function saveHostAdress(hostAdress) {
    config.hostAdress = hostAdress;
    if (!config.medflixClientId)
        config.medflixClientId = "MEDFLIX_CLIENT_" + Date.now();

    await fs.promises.writeFile(configFile, JSON.stringify(config), 'utf-8');
}

module.exports = { getHostAdress, getCliendId, saveHostAdress, loadConfig };