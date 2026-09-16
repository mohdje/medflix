export function ToTimeFormat(totalMinutes) {
    const totalHours = totalMinutes / 60;

    const hours = Math.trunc(totalHours);
    const minutes = Math.trunc((totalHours - hours) * 60);

    let timeFormat = '';
    if (hours > 0)
        timeFormat += hours + 'h';

    timeFormat += (minutes < 10 ? "0" + minutes : minutes) + 'min';

    return timeFormat;
}

export function formatFileSize(bytesLength) {
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    if (bytesLength === 0) return '0 Bytes';
    const i = Math.floor(Math.log(bytesLength) / Math.log(1024));
    return Math.round(bytesLength / Math.pow(1024, i), 2) + ' ' + sizes[i];
}

export function formatDateTime(dateTime) {
    if (dateTime === null || dateTime === undefined || dateTime === '') return '-';

    const date = new Date(dateTime);
    if (Number.isNaN(date.getTime())) return '-';

    const pad = (value, length = 2) => String(value).padStart(length, '0');

    return `${pad(date.getDate())}-${pad(date.getMonth() + 1)}-${date.getFullYear()} `
        + `${pad(date.getHours())}:${pad(date.getMinutes())}:${pad(date.getSeconds())}.${pad(date.getMilliseconds(), 3)}`;
}