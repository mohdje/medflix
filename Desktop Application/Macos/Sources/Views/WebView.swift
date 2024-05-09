import SwiftUI
import WebKit

struct WebView: NSViewRepresentable {
    private let url: URL
    private var onVideoPlayerRequested: (VideoPlayerOptions) -> Void
    
    init(url: URL, onVideoPlayerRequested: @escaping (VideoPlayerOptions) -> Void){
        self.onVideoPlayerRequested = onVideoPlayerRequested
        self.url = url
    }
    
    func makeNSView(context: Context) -> WKWebView {
        let webView = WKWebView()
        
        let userContentController = webView.configuration.userContentController
        let messageHandler = MessageHandler(onVideoPlayerRequested: onVideoPlayerRequested)
        userContentController.add(messageHandler, name: "medflixWebViewHandler")
        
        webView.load(URLRequest(url: url))
        return webView
    }
    
    func updateNSView(_ uiView: WKWebView, context: Context) {}
}

class MessageHandler: NSObject, WKScriptMessageHandler {
    private var onVideoPlayerRequested: (VideoPlayerOptions) -> Void
    
    init(onVideoPlayerRequested: @escaping (VideoPlayerOptions) -> Void){
        self.onVideoPlayerRequested = onVideoPlayerRequested
    }
    
    func userContentController(_ userContentController: WKUserContentController, didReceive message: WKScriptMessage) {
        
        do {
            let jsonData = String(describing: message.body)
            let videoPlayerOptions = try JSONDecoder().decode(VideoPlayerOptions.self, from: jsonData.data(using: .utf8)!)
            onVideoPlayerRequested(videoPlayerOptions)
           
        } catch {
            print("Error decoding JSON: \(error)")
        }
    }
}

//window.webkit.messageHandlers.medflixWebViewHandler.postMessage('{"resumeToTime":0,"sources":[{"url":"https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4","quality":"1080p.web","selected":true},{"url":"https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ElephantsDream.mp4","quality":"1080p.web","selected":false},{"url":"https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/Sintel.mp4","quality":"720.web","selected":false}],"subtitles":[{"language":"French","subtitlesSourceUrls":["a","b","c"]},{"language":"English","subtitlesSourceUrls":["d","e","f"]}],"watchedMedia":{"id":"12","seasonNumber":1,"episodeNumber":1,"coverImageUrl":"String","rating":5.8,"synopsis":"String","title":"Dungeons & Dragons: thief story","totalDuration":3409.8,"year":2022,"mediaType":"movies", "resumeToTime": 0}}');
