import { httpGet, httpDelete, HOST_ADRESS } from "../HttpRequestService";

export async function getErrorLogs() {
    return await httpGet('logs/errors');
}

export async function deleteErrorLogs() {
    return await httpDelete('logs/errors', null, null, false);
}

let eventSource;
export function listenToLiveLogs(onLogReceived, onError) {
    if (!eventSource)
        eventSource = new EventSource(`${HOST_ADRESS}/logs/live`);

    eventSource.onerror = (error) => {
        onError(error);
    };
    eventSource.addEventListener("live_logs", (event) => {
        onLogReceived(event.data);
    });
}

export function stopListenToLiveLogs() {
    if (eventSource) {
        eventSource.close();
        eventSource = null;
    }
}
