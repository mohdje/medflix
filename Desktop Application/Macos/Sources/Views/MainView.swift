import SwiftUI

struct MainView: View {
    private var onVideoPlayerRequested: (VideoPlayerOptions) -> Void
    private var medflixApiService : MedflixApiService
    
    init(onVideoPlayerRequested: @escaping (VideoPlayerOptions) -> Void, medflixApiService: MedflixApiService){
        self.onVideoPlayerRequested = onVideoPlayerRequested
        self.medflixApiService = medflixApiService
    }
    //http://192.168.1.61:3000/
    //"https://jsfiddle.net/"
    var body: some View {
        WebView(url: URL(string: medflixApiService.getWebviewUrl())!, onVideoPlayerRequested: onVideoPlayerRequested)
            .edgesIgnoringSafeArea(.all)
    }
}


