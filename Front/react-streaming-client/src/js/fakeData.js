export const FakeSubtitles = {
    getSubtitles(id) {
        if (id === "fr_1") {
            return [
                {
                    startTime: 0,
                    endTime: 20,
                    text: "Voici des sous-titres pour la version demo"
                },
                {
                    startTime: 20,
                    endTime: 40,
                    text: "L'application récupère les sous-titres du film choisi"
                },
                {
                    startTime: 40,
                    endTime: 60,
                    text: "à partir de opensubtitles.org"
                },
                {
                    startTime: 60,
                    endTime: 100,
                    text: "et propose plusieurs versions pour chaque langue"
                }
            ]
        }
        else if (id === "fr_2") {
            return [
                {
                    startTime: 0,
                    endTime: 20,
                    text: "Voici une autre autre version des sous-titres pour la version demo"
                },
                {
                    startTime: 20,
                    endTime: 50,
                    text: "L'application récupère les sous-titres du film choisi"
                },
                {
                    startTime: 50,
                    endTime: 80,
                    text: "à partir de opensubtitles.org"
                },
                {
                    startTime: 80,
                    endTime: 100,
                    text: "et propose plusieurs versions pour chaque langue"
                }
            ]
        }
        else if (id === "eng_1") {
            return [
                {
                    startTime: 0,
                    endTime: 30,
                    text: "Here are subtitles for the demo version"
                },
                {
                    startTime: 30,
                    endTime: 50,
                    text: "The application get the selected movie's subtitles "
                },
                {
                    startTime: 50,
                    endTime: 80,
                    text: "from opensubtitles.org"
                },
                {
                    startTime: 80,
                    endTime: 100,
                    text: "and proposes to the user several versions for each language"
                }
            ]
        }
        else if (id === "eng_2") {
            return [
                {
                    startTime: 0,
                    endTime: 30,
                    text: "Here is an other version of subtitles for the demo version"
                },
                {
                    startTime: 30,
                    endTime: 50,
                    text: "The application get the selected movie's subtitles "
                },
                {
                    startTime: 50,
                    endTime: 80,
                    text: "from opensubtitles.org"
                },
                {
                    startTime: 80,
                    endTime: 100,
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
