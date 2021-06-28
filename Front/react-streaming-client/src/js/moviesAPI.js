import { LastSeenMovies, BookmarkedMovies } from "./fakeData"

const MoviesAPI = {
    apiBaseUrl: 'https://yts.mx/api/v2/list_movies.json',

    getMoviesGenres(onSuccess, onFail) {
        onSuccess(["Thriller", "Sci-Fi", "Horror", "Romance", "Action", "Comedy", "Drama", "Crime", "Animation", "Adventure", "Fantasy"]);
    },

    getLastMoviesByGenre(genre, onSuccess, onFail) {
        var parameters = [
            {
                name: 'genre',
                value: genre
            },
            {
                name: 'limit',
                value: 15
            },
            {
                name: 'minimum_rating',
                value: 7
            },
            {
                name: 'with_rt_ratings',
                value: 'true'
            }
        ];
        this.sendRequest(this.apiBaseUrl, parameters, onSuccess, onFail);
    },

    getSuggestedMovies(onSuccess, onFail) {
        var parameters = [

            {
                name: 'limit',
                value: 5
            },
            {
                name: 'minimum_rating',
                value: 8
            }
        ];
        this.sendRequest(this.apiBaseUrl, parameters, onSuccess, onFail);
    },

    getMoviesByGenre(genre, page, onSuccess, onFail) {
        var parameters = [
            {
                name: 'genre',
                value: genre
            },
            {
                name: 'limit',
                value: 20
            },
            {
                name: 'page',
                value: page
            },
            {
                name: 'with_rt_ratings',
                value: 'true'
            },
            {
                name: 'sort_by',
                value: 'year'
            }
        ];
        this.sendRequest(this.apiBaseUrl, parameters, onSuccess, onFail);
    },
    getMovieDetails(id, onSuccess, onFail) {
        var parameters = [
            {
                name: 'movie_id',
                value: id
            },
            {
                name: 'with_cast',
                value: true
            }
        ];
        this.sendRequest("https://yts.mx/api/v2/movie_details.json", parameters, onSuccess, onFail);
    },

    searchMovies(text, onSuccess, onFail) {
        var parameters = [
            {
                name: 'query_term',
                value: text
            },
            {
                name: 'sort_by',
                value: 'year'
            }
        ];
        this.sendRequest(this.apiBaseUrl, parameters, onSuccess, onFail);
    },

    getAvailableSubtitles(onSuccess) {
        var subtitles = [
            {
                language: "French",
                subtitlesIds: ['fr_1', 'fr_2']
            },
            {
                language: "English",
                subtitlesIds: ['eng_1', 'eng_2']
            }
        ]
        onSuccess(subtitles);
    },

    changeMovieService(serviceName, onSuccess) {
        onSuccess();
    },

    getLastSeenMovies(onSuccess, onFail) {
        console.log("LastSeenMovies", LastSeenMovies);
        onSuccess(LastSeenMovies);
    },

    saveLastSeenMovie(movie) {
        var index = LastSeenMovies.findIndex(m => m.movie.id === movie.id);

        if(index === -1){
            LastSeenMovies.push({
                movie: movie,
                serviceName: "YtsApiMx"
            });
        }
    },

    getBookmarkedMovies(onSuccess, onFail) {
        onSuccess(BookmarkedMovies);
    },

    addBookmarkedMovie(movie, onSuccess) {
        var index = BookmarkedMovies.findIndex(m => m.movie.id === movie.id);

        if(index === -1){
            BookmarkedMovies.push({
                movie: movie,
                serviceName: "YtsApiMx"
            });
        }
      
        onSuccess();
    },

    deleteBookmarkedMovie(movieBookmark, onSuccess) {
        var index = BookmarkedMovies.findIndex(m => m.movie.id === movieBookmark.movie.id);
        BookmarkedMovies.splice(index, 1);
        onSuccess();
    },

    isMovieBookmarked(movieId, serviceName, onSuccess, onFail) {
        var index = BookmarkedMovies.findIndex(m => m.movie.id === movieId);
        onSuccess(index > -1 ? 'true' : 'false');
    },

    sendRequest(url, parameters, onSuccess, onFail) {
        var xhttp = new XMLHttpRequest();
        const self = this;
        xhttp.onreadystatechange = function () {
            if (this.readyState === 4) {
                if (this.status === 200) {
                    var result = JSON.parse(this.response);
                    if (onSuccess)
                        onSuccess(self.extractObjectFromResult(result));
                }
                else {
                    if (onFail)
                        onFail();
                }
            }
        };

        if (parameters && parameters.length > 0) {
            url += '?';
            for (let i = 0; i < parameters.length; i++) {
                const parameter = parameters[i];
                url += parameter.name + '=' + parameter.value;
                if (i !== parameters.length - 1)
                    url += '&'
            }
        }

        xhttp.open("GET", url, true);
        xhttp.send();
    },

    extractObjectFromResult(result) {
        if (result.data?.movies) return result.data?.movies.map(m => this.toMovieDto(m));
        else if (result.data?.movie) return this.toMovieDto(result.data.movie);
        else if (result.data?.movie_count === 0) return [];
        else return result;
    },
    toMovieDto(baseMovieObject) {
        return {
            id: baseMovieObject.id,
            coverImageUrl: baseMovieObject.medium_cover_image,
            title: baseMovieObject.title,
            year: baseMovieObject.year,
            rating: baseMovieObject.rating,
            synopsis: baseMovieObject.description_full,
            backgroundImageUrl: baseMovieObject.background_image,
            genres: baseMovieObject.genres?.join(", "),
            duration: baseMovieObject.runtime + " min.",
            torrents: baseMovieObject.torrents,
            cast: baseMovieObject.cast?.map(c => c.name).join(", "),
            director: baseMovieObject.cast?.map(c => c.name)[0],
            youtubeTrailerUrl: 'https://www.youtube.com/embed/' + baseMovieObject.yt_trailer_code,
            imdbCode: 'tt_000'
        }
    }
}

export default MoviesAPI;

