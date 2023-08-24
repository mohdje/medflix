export function ToTimeFormat(totalMinutes) {
    const totalHours = totalMinutes / 60;

    const hours = Math.trunc(totalHours);
    const minutes = Math.trunc((totalHours - hours) * 60);

    let timeFormat = '';
    if (hours > 0)
        timeFormat += hours + 'h ';

    timeFormat += (minutes < 10 ? "0" + minutes : minutes) + 'min';

    return timeFormat;
}