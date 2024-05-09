class MediaInfo : Codable, ObservableObject {
    var id : String
    var seasonNumber : Int
    var episodeNumber : Int
    var coverImageUrl : String
    var rating : Double
    var synopsis : String
    var title : String
    var totalDuration : Double
    var year : Int
    var torrentUrl: String!
    var currentTime: Double!
    var genres : [Genre]!
    
    init(){
        self.id = ""
        self.seasonNumber = 0
        self.episodeNumber = 0
        self.coverImageUrl = ""
        self.rating = 0.0
        self.synopsis = ""
        self.title = ""
        self.totalDuration = 0.0
        self.year = 0
        self.torrentUrl = ""
        self.currentTime = 0.0
        self.genres = []
    }
}

struct Genre : Codable, Hashable {
    var id : Int
    var name : String
}
