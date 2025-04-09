export function addEvent(eventName, callback) {
    document.addEventListener(eventName, callback)
}

export function removeEvent(eventName, callback) {
    document.removeEventListener(eventName, callback);
}

export function raiseEvent(eventName) {
    document.dispatchEvent(new Event(eventName));
}

export const eventsNames = {
    bookmarkUpdated: "bookmarkUpdated",
    watchProgressUpdated: "watchProgressUpdated"
}