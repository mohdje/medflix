import { httpGet, httpDelete } from "../HttpRequestService";

export async function getAvailableMediasVideos() {
    return await httpGet(`videos`);
}

export async function deleteMediasVideos(videoIdsToRemove) {
    var queryParameters = videoIdsToRemove.map(videoId => { return { name: 'videosIds', value: videoId } });
    var result = await httpDelete('videos', queryParameters, null, true);
    return result?.success;
}

