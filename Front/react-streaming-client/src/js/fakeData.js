
const testsubtitleOptions = [
    {
        label: 'Off',
        selected: true,
        data: {
            url: ""
        }
    },
    {
        label: 'En',
        selected: false,
        subOptions: [

            {
                label: 'En 1',
                selected: false,
                data: {
                    url: MoviesAPI.apiSubtitlesUrl + "8199534"
                }
            },
            {
                label: 'En 2',
                selected: false,
                data: {
                    url: MoviesAPI.apiSubtitlesUrl + "8199534"
                }
            }
        ]
    },
    {
        label: 'Fr',
        selected: false,
        subOptions: [

            {
                label: 'Fr 1',
                selected: false,
                data: {
                    url: MoviesAPI.apiSubtitlesUrl + "8199534"
                }
            },
            {
                label: 'Fr 2',
                selected: false,
                data: {
                    url: MoviesAPI.apiSubtitlesUrl + "8199534"
                }
            }
        ]
    }
]


const videoQualities = [
    {
        label: '1080p',
        selected: true,
        data: {
            url: 'https://cdn.fluidplayer.com/videos/valerian-1080p.mkv'
        }
    },
    {
        label: '720p',
        selected: false,
        data: {
            url: 'https://www.learningcontainer.com/wp-content/uploads/2020/05/sample-mp4-file.mp4'
        }
    },
];