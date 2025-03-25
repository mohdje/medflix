import AppMode from "../appMode";
import { httpGet } from "../HttpRequestService";

export async function getMediasOfToday() {
    return await httpGet(`${AppMode.getActiveMode().urlKey}/mediasoftoday`);
}

export async function getPopularNetflixMedias() {
    return await httpGet(`${AppMode.getActiveMode().urlKey}/netflix`);
}

export async function getPopularDisneyPlusMedias() {
    return await httpGet(`${AppMode.getActiveMode().urlKey}/disneyplus`);
}

export async function getPopularAmazonPrimeMedias() {
    return await httpGet(`${AppMode.getActiveMode().urlKey}/amazonprime`);
}

export async function getPopularAppleTvMedias() {
    return await httpGet(`${AppMode.getActiveMode().urlKey}/appletv`);
}

export async function getPopularMedias() {
    return await httpGet(`${AppMode.getActiveMode().urlKey}/popular`);
}

export async function getRecommandedMedias() {
    return await httpGet(`${AppMode.getActiveMode().urlKey}/recommandations`);
}

export async function getMediasByGenre(genreId, page) {
    return await httpGet(`${AppMode.getActiveMode().urlKey}/genre/${genreId}/${page}`);
}

export async function getMediasByPlatform(platformId, page) {
    return await httpGet(`${AppMode.getActiveMode().urlKey}/platform/${platformId}/${page}`);
}

export async function getEpisodes(mediaId, seasonNumber) {
    return await httpGet(`${AppMode.getActiveMode().urlKey}/episodes/${mediaId}/${seasonNumber}`);
}

export async function getMediaDetails(mediaId) {
    return await httpGet(`${AppMode.getActiveMode().urlKey}/details/${mediaId}`);
}

export async function getSimilarMedias(mediaId) {
    return await httpGet(`${AppMode.getActiveMode().urlKey}/similar/${mediaId}`);
}

export async function searchMedias(text) {
    var parameters = [
        {
            name: 't',
            value: text
        }
    ]
    return await httpGet(`${AppMode.getActiveMode().urlKey}/search`, parameters);
}

export async function getMediaGenres() {
    return await httpGet(`${AppMode.getActiveMode().urlKey}/genres`);
}

export async function getMediaPlatforms() {
    return await httpGet(`${AppMode.getActiveMode().urlKey}/platforms`);
}

