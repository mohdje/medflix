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