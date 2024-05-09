struct VideoQualityOption : Hashable, Decodable {
    var url: String
    var quality: String
    var selected: Bool
    
    init(url: String, quality: String, selected: Bool) {
        self.url = url
        self.quality = quality
        self.selected = selected
    }
}
