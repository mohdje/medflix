const AppMode = {
    onAppModeChanged(callback){
        const eventName = 'appModeChanged';
        if(!this.modeChangedEvent)
            this.modeChangedEvent = new Event(eventName);
        document.addEventListener(eventName, (e) => { 
            if(callback)
                callback() 
        }, false);
    },
    modeChangedEvent: null,
    modes: [
        {
            label: 'Films',
            urlKey: 'movies',
            isActive: true
        },
        {
            label: 'Series',
            urlKey: 'series',
            isActive: false
        }
    ],
    getActiveMode(){
        return this.modes.find(mode => mode.isActive);
    },
    switchActiveMode(){
        this.modes.forEach(mode => {
            mode.isActive = !mode.isActive;
        });
        if(this.modeChangedEvent)
            document.dispatchEvent(this.modeChangedEvent);
    }
}

export default AppMode;