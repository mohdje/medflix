struct DownloadingState : Decodable {
    var message : String! = ""
    var error : Bool = false
    
    init (message: String, error: Bool){
        self.message = message
        self.error = error
    }
}
