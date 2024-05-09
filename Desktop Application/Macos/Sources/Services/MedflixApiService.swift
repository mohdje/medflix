class MedflixApiService : HttpRequestService {
    private let hostUrl = "http://localhost:5000"
    
    var mediaType = ""
    
    func startService(){
        let executablePath = Bundle.main.resourcePath ?? ""
        let executableURL = URL(fileURLWithPath: executablePath).appendingPathComponent("MedflixWebHost/MedflixWebHost")

        let process = Process()
        process.executableURL = executableURL
        process.arguments = ["-d"]
        
        do {
            try process.run()
        } catch {
            print("Failed to launch the executable: \(error)")
        }
        
    }
    
    func getWebviewUrl() -> String {
        return "\(hostUrl)/home/index.html"
    }
    
    func isServiceReady(onComplete: @escaping (Bool)-> Void, tries: Int) {
        ping(url: "\(hostUrl)/application/ping", completion: {
            result in
            switch result {
            case .success(let response):
                onComplete(response)
            case .failure:
                if(tries == 0){
                    onComplete(false)
                }
                else {
                    DispatchQueue.main.asyncAfter(deadline: .now() + 2.0) {
                        self.isServiceReady(onComplete: onComplete, tries: tries - 1)
                    }
                }
            }
        })
    }
    
    func getDownloadState (base64url: String, onDataFetched: @escaping (DownloadingState) -> Void) {
        fetchObject(url: "\(hostUrl)/torrent/streamdownloadstate?base64TorrentUrl=\(base64url)", completion: {
            (result: Result<DownloadingState, Error>) in
            switch result {
            case .success(let data):
                onDataFetched(data)
            case .failure:
                onDataFetched(DownloadingState(message: "An error occured", error: true))
            }
        })
    }
    
    func getSubtitles (url: String, onDataFetched: @escaping ([VideoSubtitle]) -> Void) {
        fetchObject(url: "\(hostUrl)/subtitles?sourceUrl=\(url)", completion: {
            (result: Result<[VideoSubtitle] , Error>) in
            switch result {
            case .success(let data):
                onDataFetched(data)
            case .failure:
                onDataFetched([])
            }
        })
    }
    
    func saveProgression (mediaInfo: MediaInfo){
        do{
            let encoder = JSONEncoder()
            let jsonData = try encoder.encode(mediaInfo)
            
            putRequest(url: "\(hostUrl)/\(mediaType)/watchedMedia", jsonData: jsonData, completion: {_ in})
        }
        catch{
            
        }
    }
}
