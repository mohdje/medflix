const { app, dialog, shell } = require('electron');
const { exec, spawn } = require('child_process')
const fs = require('fs')
const path = require('path');
const { getCliendId, getHostAdress } = require('./config.js');
const { url } = require('inspector');

async function launchVLC(mainWindow, parameters, callback) {
    const isVlcInstalled = await checkVlcInstalledAsync();
    if (!isVlcInstalled) {
        callback();
        const result = await dialog.showMessageBox(mainWindow, {
            buttons: ['Go to website', 'Close'],
            message: "VLC media player is needed. Install it from the official website and try again."
        });
        if (result.response === 0) {
            shell.openExternal('https://www.videolan.org/vlc/');
        }
        return;
    }

    let sourcesList = sortSources(parameters);

    const hostUrl = `http://${getHostAdress()}`;

    const playListFile = await buildPlaylist(hostUrl, sourcesList, parameters.watchMedia);

    const subfiles = await buildSubtitlesCommandLine(mainWindow, hostUrl, parameters.subtitlesSources);

    const openVlcCommand = process.platform === 'darwin' ? "open -a VLC --args" : "vlc";
    const command = `${openVlcCommand} --extraintf rc --rc-host=localhost:4212 --no-loop --no-repeat --no-rc-fake-tty --start-time=${parameters.watchMedia.currentTime} ${playListFile} ${subfiles} :http-user-agent="${getCliendId()}"`;

    const event = listenToPlaybackProgress(async (time, videoUrl, totalDuration) => onPlaybackProgress(hostUrl, parameters.watchMedia, time, videoUrl, totalDuration)
        , (error) => {
            console.error(error);
            onExitVlcPlayer();
        });

    const onExitVlcPlayer = () => {
        clearInterval(event);
        if (fs.existsSync(playListFile))
            fs.rmSync(playListFile);
        callback();
    }

    exec(command, { shell: '/bin/bash' }, (error, stdout, stderr) => {
        if (error)
            console.error("error:", error);

        if (process.platform !== 'darwin')
            onExitVlcPlayer();
    })
}

async function buildPlaylist(hostUrl, sourcesList, watchMedia) {
    let content = "";
    sourcesList.forEach(ms => {
        content += `#EXTINF:-1,${ms.language} – ${ms.quality}\n`;
        content += `${buildStreamUrl(hostUrl, ms, watchMedia.media.id, watchMedia.episodeNumber, watchMedia.seasonNumber)}\n`;
    });
    const playListFile = path.join(app.getPath('videos'), "Medflix.m3u");
    await fs.promises.writeFile(playListFile, content, 'utf-8');
    return playListFile;
}

function checkVlcInstalledAsync() {
    return new Promise((resolve, reject) => {
        const command = process.platform === 'darwin' ? "mdfind \"kMDItemCFBundleIdentifier == 'org.videolan.vlc'\"" : "which vlc";
        exec(command, { shell: '/bin/bash' }, (error, stdout, stderr) => {
            if (error)
                resolve(false);
            resolve(Boolean(stdout));
        });
    });
}

async function buildSubtitlesCommandLine(mainWindow, hostUrl, subtitlesSources) {
    const languages = subtitlesSources.map(ss => ss.language);
    if (languages?.length > 0) {
        const none = "None";
        if (process.platform === 'darwin')
            languages.unshift(none);
        else
            languages.push(none);

        const result = await dialog.showMessageBox(mainWindow, {
            buttons: languages,
            message: `Subtitles are available for this media. Pick the language of your choice.`
        });
        if (result.response !== languages.indexOf(none)) {
            var urls = subtitlesSources.find(ss => ss.language === languages[result.response]).urls.slice(0, 2);
            return `:input-slave=${urls.map((url) => `${hostUrl}/subtitles/file/${encodeToBase64(url)}.srt`).join("#")}`;
        }
    }
    return "";
}

function sortSources(parameters) {
    const lastSelectedSource = parameters.mediaSources.find(ms => ms.filePath === parameters.watchMedia.videoSource || ms.torrentUrl === parameters.watchMedia.videoSource);
    const firstFileSource = parameters.mediaSources.find(ms => ms.filePath);
    let sourcesList;

    if (lastSelectedSource) {
        sourcesList = parameters.mediaSources.filter(ms => ms !== lastSelectedSource);
        sourcesList.unshift(lastSelectedSource);
    } else if (firstFileSource) {
        sourcesList = parameters.mediaSources.filter(ms => ms !== firstFileSource);
        sourcesList.unshift(firstFileSource);
    }
    else {
        sourcesList = parameters.mediaSources;
    }
    return sourcesList;
}

function buildStreamUrl(hostUrl, mediaSource, mediaId, episodeNumber, seasonNumber) {
    if (mediaSource.filePath)
        return `${hostUrl}/videos/stream?base64VideoPath=${encodeToBase64(mediaSource.filePath)}`;
    else if (mediaSource.torrentUrl) {
        if (episodeNumber > 0 && seasonNumber > 0)
            return `${hostUrl}/torrent/stream/series?base64TorrentUrl=${encodeToBase64(mediaSource.torrentUrl)}&episodeNumber=${episodeNumber}&seasonNumber=${seasonNumber}&mediaId=${mediaId}&language=${mediaSource.language}&quality=${mediaSource.quality}`;
        else
            return `${hostUrl}/torrent/stream/movies?base64TorrentUrl=${encodeToBase64(mediaSource.torrentUrl)}&mediaId=${mediaId}&language=${mediaSource.language}&quality=${mediaSource.quality}`;
    }
}

function listenToPlaybackProgress(onProgress, onError) {
    const containingFolder = path.join(__dirname, app.isPackaged ? "../.." : "", "scripts");
    const scriptFile = path.join(containingFolder, "vlc_script.sh");
    return setInterval(() => {
        const script = spawn("bash", [scriptFile]);
        script.stdout.on('data', (stdout) => {
            try {
                const data = JSON.parse(stdout);
                if (data.status !== "playing")
                    onError();
                else if (data.time && data.url) {
                    onProgress(data.time, data.url, data.totalDuration);
                }
            } catch (err) {
                if (onError)
                    onError(err);
            }
        });

        script.stderr.on('data', (err) => {
            if (onError)
                onError(err);
        });

        script.on('exit', (code) => {
            if (code !== 0 && onError)
                onError("Exit code = " + code);
        });
    }, 15000);
}

async function onPlaybackProgress(hostUrl, watchMedia, time, videoUrl, totalDuration) {
    watchMedia.currentTime = time;
    watchMedia.totalDuration = totalDuration;
    const src = new URL(videoUrl);
    let base64torrentUrl = src.searchParams.get("base64TorrentUrl");
    let base64VideoPath = src.searchParams.get("base64VideoPath");
    if (src.href.startsWith(`${hostUrl}/torrent/stream`) && base64torrentUrl)
        watchMedia.videoSource = decodeFromBase64(base64torrentUrl);
    else if (src.href.startsWith(`${hostUrl}/videos/stream`) && base64VideoPath)
        watchMedia.videoSource = decodeFromBase64(base64VideoPath);
    else
        return;

    const mediaType = watchMedia.episodeNumber > 0 && watchMedia.seasonNumber > 0 ? "series" : "movies";
    try {
        await fetch(`${hostUrl}/${mediaType}/watchedmedia`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(watchMedia)
        });
    } catch (error) {
        console.error('Failed to save watchMedia:', error);
    }
}

function encodeToBase64(text) {
    return Buffer.from(unescape(encodeURIComponent(text))).toString("base64");
}

function decodeFromBase64(text) {
    return Buffer.from(text, "base64").toString();
}

module.exports = { launchVLC };