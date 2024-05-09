class AppUpdateService : HttpRequestService {
    private let version = Bundle.main.infoDictionary?["CFBundleShortVersionString"]
    private var appRelease : AppRelease = AppRelease(name: "", assets: [])

    func isNewRealeaseAvailable (onDataFetched: @escaping (Bool) -> Void) {
        let headers = [
            "X-GitHub-Api-Version": "2022-11-28",
            "User-Agent": "Swift App",
            "Content-Type" : "application/vnd.github+json"
        ]
        fetchObject(url: "https://api.github.com/repos/mohdje/medflix/releases/latest", completion: {
            (result: Result<AppRelease, Error>) in
            switch result {
            case .success(let data):
                if(data.name != "Medflix \(self.version ?? "")"){
                    self.appRelease = data
                    onDataFetched(true)
                }
            case .failure:
                onDataFetched(false)
            }
        }, headers: headers)
    }
    
    func downloadNewversion(completion: @escaping (Bool, URL?) -> Void) {
        let headers = [
            "X-GitHub-Api-Version": "2022-11-28",
            "User-Agent": "Swift App"
        ]
        
        let releaseUrl = appRelease.assets.first(where: {$0.name.range(of: "macos", options: .caseInsensitive) != nil})?.browser_download_url ?? ""
        
        if releaseUrl != "" {
            downloadFile(fileUrl: releaseUrl, destinationFileName: "Medlfix_MacOS.zip", headers: headers, completion: completion)
        }
        else{
            completion(false, URL(string: "")!)
        }
    }
    
    func startExtractUpdate(packageURL: URL){
        let resourcesPath = Bundle.main.resourcePath ?? ""
        let extractAppPath = resourcesPath + "/extract_medflix_package.app"
        let tempExtractAppPath = resourcesPath + "/Extract Medflix Package.app"
      
        do {
            let fileManager = FileManager.default

            if fileManager.fileExists(atPath: tempExtractAppPath) {
                try fileManager.removeItem(atPath: tempExtractAppPath)
            }
            
            try fileManager.copyItem(at: URL(fileURLWithPath: extractAppPath), to: URL(fileURLWithPath: tempExtractAppPath))
        } catch {
            print("Error app: \(error)")
            return
        }
        
        let applicationsFolder = FileManager.default.urls(for: .applicationDirectory, in: .localDomainMask).first!
        
        let configuration = NSWorkspace.OpenConfiguration()
        configuration.arguments = [packageURL.path, applicationsFolder.path]
        
        NSWorkspace.shared.openApplication(at: URL(fileURLWithPath: tempExtractAppPath), configuration: configuration, completionHandler: {_,_ in})
    }
}

