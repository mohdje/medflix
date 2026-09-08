import { httpGet, httpDelete, httpPost } from "../HttpRequestService";

export async function getAvailableMediasVideos() {
    return await httpGet(`videos`);
}

export async function deleteMediasVideos(videoIdsToRemove) {
    var queryParameters = videoIdsToRemove.map(videoId => { return { name: 'videosIds', value: videoId } });
    var result = await httpDelete('videos', queryParameters, null, true);
    return result?.success;
}

export async function uploadMediasVideos(mediaId, seasonNumber, episodeNumber, language, quality, file, onUploadProgress) {

    const interval = setInterval(async () => {
        const uploadedSize = await httpGet('videos/size', [{ name: 'fileName', value: file.name }], true);
        if (uploadedSize && onUploadProgress) {
            var progress = (uploadedSize / file.size) * 100;
            onUploadProgress(progress.toFixed(1));
        }
    }, 2000);
    const result = await httpPost('videos', null, { mediaId, seasonNumber: !seasonNumber || isNaN(seasonNumber) ? 0 : seasonNumber, episodeNumber: !episodeNumber || isNaN(episodeNumber) ? 0 : episodeNumber, languageVersion: language.value, mediaQuality: quality, file }, false);
    clearInterval(interval);
    return Boolean(result);
}

