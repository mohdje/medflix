import MovieLogo from '../assets/movie.svg';
import TvLogo from '../assets/tv.svg';


const AppMode = {
    onAppModeChanged(callback) {
        const eventName = 'appModeChanged';
        if (!this.modeChangedEvent)
            this.modeChangedEvent = new Event(eventName);
        document.addEventListener(eventName, (e) => {
            if (callback)
                callback()
        }, false);
    },
    modeChangedEvent: null,
    modes: [
        {
            label: 'Films',
            urlKey: 'movies',
            logo: MovieLogo,
            isActive: true
        },
        {
            label: 'Series',
            urlKey: 'series',
            logo: TvLogo,
            isActive: false
        }
    ],
    getActiveMode() {
        return this.modes.find(mode => mode.isActive);
    },
    switchActiveMode() {
        this.modes.forEach(mode => {
            mode.isActive = !mode.isActive;
        });
        if (this.modeChangedEvent)
            document.dispatchEvent(this.modeChangedEvent);
    },
    isSeriesMode() {
        return this.modes.filter(mode => mode.isActive)[0].urlKey === "series"
    }
}

export default AppMode;