struct VideoPlayerOptions : Decodable {
    var sources: [VideoQualityOption]
    var resumeToTime: Int!
    var subtitles: [SubtitlesOption]
    var watchedMedia : MediaInfo
    var mediaType : String
}

