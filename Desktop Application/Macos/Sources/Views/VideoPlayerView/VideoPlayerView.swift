import SwiftUI
import VLCKit

struct VideoPlayerView: View {
    @Binding private var videoUrl : String
    @Binding private var subtitlesPath : String
    @Binding private var subtitlesFontSize : CGFloat
    @Binding private var subtitlesOffset : Int
    private var mediaInfo : MediaInfo
    
    @State private var showControls = false
    @State private var videoIsLoading = true
    @State private var loadingMessage = ""
    @State private var showMediaInfo = false
    
    @State private var videoProgression = 0.0
    @State private var currentTime = 0
    @State private var remainingTime = 0
    
    @State private var videoIsMuted = false
    @State private var videoPlayerState : VLCMediaPlayerState = .stopped
    
    @StateObject var videoSubtitles : VideoSubtitles = VideoSubtitles()
    @State private var videoSubtitleText = ""
    
    @State private var lastUserActionDateTime : DispatchTime = .now()
    @State private var startVideoAfterStop = false
    @State private var lastSaveProgressionDateTime : DispatchTime = .now()
    
    @State private var videoSize : CGFloat = 0 //change video size to force VLCMediaPlayer to redraw to fill all the VideoView space
    
    @State private var closeRequested = false
    private var onVideoPlayerStopped: () -> Void
    
    private var medflixApiService : MedflixApiService
    
    init(videoUrl: Binding<String>, subtitlesPath: Binding<String>, subtitlesFontSize: Binding<CGFloat>, subtitlesOffset: Binding<Int>, mediaInfo: MediaInfo, medflixApiService: MedflixApiService, onVideoPlayerStopped: @escaping () -> Void) {
        self._videoUrl = videoUrl
        self._subtitlesPath = subtitlesPath
        self._subtitlesFontSize = subtitlesFontSize
        self._subtitlesOffset = subtitlesOffset
        self.mediaInfo = mediaInfo
        self.medflixApiService = medflixApiService
        self.onVideoPlayerStopped = onVideoPlayerStopped
    }
    
    func playMedia(startTimeMilliseconds: Int32){
        //"http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4"
        if(videoUrl != ""){
            VLCVideoPlayer.playMedia(url: "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny99.mp4",startTimeMilliseconds: startTimeMilliseconds, onMediaPlayerStateChanged: onMediaPlayerStateChanged, onTimeChanged: onMediaPlayerTimeChanged, onMuteChanged: {isMuted in videoIsMuted = isMuted})
            
            self.loadingMessage = "Loading..."
            if let base64url = getQueryParameter(url: videoUrl, parameter: "base64Url") {
                if let data = Data(base64Encoded: base64url) {
                    if let torrentUrl = String(data: data, encoding: .utf8) {
                        mediaInfo.torrentUrl = torrentUrl
                    }
                }
                getDownloadingState(base64url: base64url)
            }
        }
    }
    
    func getDownloadingState(base64url: String){
        DispatchQueue.main.asyncAfter(deadline: .now() + 3.0) {
            medflixApiService.getDownloadState(base64url: base64url, onDataFetched: {
                downloadingState in
                self.loadingMessage = downloadingState.message ?? "Loading..."
                if(!downloadingState.error && videoIsLoading){
                    getDownloadingState(base64url: base64url)
                }
                else{
                    videoIsLoading = !downloadingState.error
                }
            })
        }
    }
    
    func getQueryParameter(url: String, parameter: String) -> String? {
        guard let urlComponents = URLComponents(string: url) else { return nil }
        return urlComponents.queryItems?.first(where: { $0.name == parameter })?.value
    }
    
    
    func onMediaPlayerStateChanged(state: VLCMediaPlayerState){
        videoPlayerState = state
        
        switch state {
        case .buffering, .opening:
            videoIsLoading = true
        case .paused:
            DispatchQueue.main.asyncAfter(deadline: .now() + 3.0) {
                if(videoPlayerState == .paused){
                    showMediaInfo = true
                }
            }
        case .stopped:
            videoSize = 0
            if(startVideoAfterStop){
                playMedia(startTimeMilliseconds: Int32(currentTime))
            }
            else if(closeRequested){
                onVideoPlayerStopped()
            }
            else {
                //error
                videoIsLoading = false
                loadingMessage = "An error has occured"
            }
        default:
            break
        }
    }
    
    func onMediaPlayerTimeChanged(time: Int, remainingTime: Int, position: Float){
        videoSize = .infinity
        
        self.videoProgression = Double(position) * 100
        self.currentTime = time
        self.remainingTime = remainingTime
        self.videoIsLoading = false
        self.loadingMessage = ""
        self.showMediaInfo = false
      
        if(videoSubtitles.items.count > 0){
            let timeInSeconds = time/1000
            videoSubtitleText = videoSubtitles.items.first(where: {Int($0.startTime) + subtitlesOffset <= timeInSeconds && Int($0.endTime) + subtitlesOffset >= timeInSeconds})?.text ?? ""
        }
        
        let nanoSecondsBetween = DispatchTime.now().uptimeNanoseconds - lastSaveProgressionDateTime.uptimeNanoseconds
        let seconds = Double(nanoSecondsBetween) /  1_000_000_000
        if(seconds >= 15 ){
            lastSaveProgressionDateTime = .now()
            mediaInfo.currentTime = Double(time/1000)
            mediaInfo.totalDuration = Double(VLCVideoPlayer.getDurationInSeconds())
            medflixApiService.saveProgression(mediaInfo: mediaInfo)
        }
    }
    
    func onUserAction(){
        showControls = true
        lastUserActionDateTime = .now()
        
        DispatchQueue.main.asyncAfter(deadline: .now() + 7.0) {
            let nanoSecondsBetween = DispatchTime.now().uptimeNanoseconds - lastUserActionDateTime.uptimeNanoseconds
            let seconds = Double(nanoSecondsBetween) /  1_000_000_000
            if(seconds >= 5 ){
                showControls = false
            }
        }
    }
    
    var body: some View {
        ZStack {
            VideoView().frame(minWidth: /*@START_MENU_TOKEN@*/0/*@END_MENU_TOKEN@*/, idealWidth: videoSize, maxWidth: videoSize, minHeight: /*@START_MENU_TOKEN@*/0/*@END_MENU_TOKEN@*/, idealHeight: videoSize, maxHeight: videoSize, alignment: /*@START_MENU_TOKEN@*/.center/*@END_MENU_TOKEN@*/)
            
            VideoSubtitleView(subtitleText: $videoSubtitleText, textSize: $subtitlesFontSize)
            
            if(showMediaInfo){
                MediaInfoView(mediaInfo: mediaInfo)
            }
            
            if(showControls){
                VideoPlayerControlsView(
                    videoIsLoading: $videoIsLoading,
                    videoPlayerState: $videoPlayerState,
                    isMuted: $videoIsMuted,
                    currentTime: $currentTime,
                    remainingTime: $remainingTime,
                    videoProgression: $videoProgression,
                    onPlayPauseButtonClick: {VLCVideoPlayer.togglePlay()},
                    onMoveBackwardButtonClick: {VLCVideoPlayer.moveBackward10Seconds()},
                    onMoveForwardButtonClick: {VLCVideoPlayer.moveForeward10Seconds()},
                    onSoundButtonClick: {VLCVideoPlayer.toggleMute()},
                    onVolumeChanged: {value in VLCVideoPlayer.setVolume(value: Int32(value))},
                    onVideoProgressionChanged: {value in VLCVideoPlayer.goToPosition(value: Float(value/100))},
                    onCloseClick: {
                        closeRequested = true
                        VLCVideoPlayer.stop()
                        
                    })
            }
            
            LoadingView(showSpinner: $videoIsLoading, message: $loadingMessage)
           
            MouseHoverArea(onMouseHover: onUserAction)
        }
        .background(Color.black)
        .onAppear{
            playMedia(startTimeMilliseconds: Int32((mediaInfo.currentTime ?? 0)) * 1000)
        }
        .onChange(of: videoUrl, perform: { value in
            startVideoAfterStop = true
            VLCVideoPlayer.stop()
        }).onChange(of: subtitlesPath, perform: { value in
            if(subtitlesPath != ""){
                medflixApiService.getSubtitles(url: value, onDataFetched: {
                    subtitles in
                    videoSubtitles.items = subtitles
                })
            }
            else{
                videoSubtitles.items = []
                videoSubtitleText = ""
            }
        })
    }
}

