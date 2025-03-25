import AppMode from "../appMode";
import { httpGet } from "../HttpRequestService";

export async function getWatchedMedias() {
    return await httpGet(`${AppMode.getActiveMode().urlKey}/watchedmedia`);
}

export async function getWatchedEpisodes(mediaId, seasonNumber) {
    return await httpGet(`${AppMode.getActiveMode().urlKey}/watchedmedia/${mediaId}/${seasonNumber}`);
}

export async function getLastWatchedMediaInfo(mediaId) {
    const watchedMedias = await getWatchedMedias();
    return watchedMedias.find(watchedMedia => watchedMedia.media.id === mediaId);
}
