import AppMode from "../appMode";
import { httpGet, httpPut, httpDelete } from "../HttpRequestService";

export async function bookmarkMedia(mediaDto) {
    return await httpPut(`${AppMode.getActiveMode().urlKey}/bookmarks`, null, mediaDto, false);
}

export async function unbookmarkMedia(mediaDto) {
    return await httpDelete(`${AppMode.getActiveMode().urlKey}/bookmarks?id=${mediaDto.id}`, null, null, false);
}

export async function isMediaBookmarked(mediaId) {
    var parameters = [
        {
            name: 'id',
            value: mediaId
        }
    ];
    return await httpGet(`${AppMode.getActiveMode().urlKey}/bookmarks/exists`, parameters);
}

export async function getBookmarkedMedias() {
    return await httpGet(`${AppMode.getActiveMode().urlKey}/bookmarks`);
}
