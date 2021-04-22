export const FakeSubtitles = {
    getSubtitles(id) {
        if (id === "fr_1") {
            return [
                {
                    startTime: 0,
                    endTime: 3,
                    text: "Voici des sous-titres pour la version demo"
                },
                {
                    startTime: 3,
                    endTime: 5,
                    text: "L'application récupère les sous-titres du film choisi"
                },
                {
                    startTime: 5,
                    endTime: 8,
                    text: "à partir de opensubtitles.org"
                },
                {
                    startTime: 8,
                    endTime: 11,
                    text: "et propose plusieurs versions pour chaque langue"
                }
            ]
        }
        else if (id === "fr_2") {
            return [
                {
                    startTime: 0,
                    endTime: 3,
                    text: "Voici une autre autre version des sous-titres pour la version demo"
                },
                {
                    startTime: 3,
                    endTime: 5,
                    text: "L'application récupère les sous-titres du film choisi"
                },
                {
                    startTime: 5,
                    endTime: 8,
                    text: "à partir de opensubtitles.org"
                },
                {
                    startTime: 8,
                    endTime: 11,
                    text: "et propose plusieurs versions pour chaque langue"
                }
            ]
        }
        else if (id === "eng_1") {
            return [
                {
                    startTime: 0,
                    endTime: 3,
                    text: "Here are subtitles for the demo version"
                },
                {
                    startTime: 3,
                    endTime: 5,
                    text: "The application get the selected movie's subtitles "
                },
                {
                    startTime: 5,
                    endTime: 8,
                    text: "from opensubtitles.org"
                },
                {
                    startTime: 8,
                    endTime: 11,
                    text: "and proposes to the user several versions for each language"
                }
            ]
        }
        else if (id === "eng_2") {
            return [
                {
                    startTime: 0,
                    endTime: 3,
                    text: "Here is an other version of subtitles for the demo version"
                },
                {
                    startTime: 3,
                    endTime: 5,
                    text: "The application get the selected movie's subtitles "
                },
                {
                    startTime: 5,
                    endTime: 8,
                    text: "from opensubtitles.org"
                },
                {
                    startTime: 8,
                    endTime: 11,
                    text: "and proposes to the user several versions for each language"
                }
            ]
        }
    }
}

export const VideoQualities = [
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
            url: 'https://cdn.fluidplayer.com/videos/valerian-720p.mkv'
        }
    },
    {
        label: '480p',
        selected: false,
        data: {
            url: 'https://cdn.fluidplayer.com/videos/valerian-480p.mkv'
        }
    }
];

export const LastSeenMovies = []

export const BookmarkedMovies = []
