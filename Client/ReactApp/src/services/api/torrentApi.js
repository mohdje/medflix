import { httpGet } from "../HttpRequestService";

export async function getTorrentFiles(torrentUrl) {
    const base64Url = window.btoa(torrentUrl);

    var parameters = [
        {
            name: 'base64torrentUrl',
            value: base64Url
        }
    ];

    return await httpGet(`torrent/files`, parameters);
}

export async function getTorrentHistory() {
    return await httpGet(`torrent/history`);
}