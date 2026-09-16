import { httpGet, httpDelete } from "../HttpRequestService";

export async function getErrorLogs() {
    return await httpGet('errorlogs');
}

export async function deleteErrorLogs() {
    return await httpDelete('errorlogs', null, null, false);
}
