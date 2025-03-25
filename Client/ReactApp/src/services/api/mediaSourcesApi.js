import AppMode from "../appMode";
import { httpGet } from "../HttpRequestService";

export async function getAvailableSubtitles(imdbId, seasonNumber, episodeNumber) {
    const parameters = [
        {
            name: 'imdbId',
            value: imdbId
        },
        {
            name: 'seasonNumber',
            value: seasonNumber
        },
        {
            name: 'episodeNumber',
            value: episodeNumber
        }
    ];

    let frenchSubtitlesUrls;
    let englishSubtitlesUrls;
    const mediaType = AppMode.getActiveMode().urlKey;

    await Promise.all(
        [
            httpGet(`subtitles/${mediaType}/fr`, parameters).then((urls) => frenchSubtitlesUrls = urls),
            httpGet(`subtitles/${mediaType}/en`, parameters).then((urls) => englishSubtitlesUrls = urls)
        ]
    )

    const result = [];

    if (frenchSubtitlesUrls?.length > 0)
        result.push({
            language: "French",
            urls: frenchSubtitlesUrls
        })

    if (englishSubtitlesUrls?.length > 0)
        result.push({
            language: "English",
            urls: englishSubtitlesUrls
        })

    return result;
}

export async function getAvailableVersionsSources(mediaId, title, year, imdbId, seasonNumber, episodeNumber) {
    const parameters = [
        {
            name: 'mediaId',
            value: mediaId
        },
        {
            name: 'title',
            value: encodeURIComponent(title)
        },
        {
            name: 'year',
            value: year
        },
        {
            name: 'seasonNumber',
            value: seasonNumber
        },
        {
            name: 'episodeNumber',
            value: episodeNumber
        },
        {
            name: 'imdbId',
            value: imdbId
        }
    ];

    let frenchSources;
    let originalSources;

    const mediaType = AppMode.getActiveMode().urlKey;
    await Promise.all(
        [
            httpGet(`mediasource/${mediaType}/vf`, parameters).then((urls) => frenchSources = urls),
            httpGet(`mediasource/${mediaType}/vo`, parameters).then((urls) => originalSources = urls)
        ]
    )

    const result = [];

    if (frenchSources?.length > 0)
        result.push({
            language: "French",
            sources: frenchSources
        })

    if (originalSources?.length > 0)
        result.push({
            language: "Original",
            sources: originalSources
        })

    return result;
}
