struct AppRelease : Decodable {
    var name : String
    var assets : [ReleaseAsset]
}

struct ReleaseAsset : Decodable, Hashable {
    var name : String
    var browser_download_url : String
}
