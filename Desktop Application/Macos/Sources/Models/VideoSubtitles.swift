
class VideoSubtitles : ObservableObject {
    var items : [VideoSubtitle] = []
}

struct VideoSubtitle : Decodable, Hashable {
    var startTime : Double
    var endTime : Double
    var text : String
}
