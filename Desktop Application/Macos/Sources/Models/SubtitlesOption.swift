struct SubtitlesOption:Hashable, Decodable {
    var language: String
    var subtitlesSourceUrls: [String]
}

