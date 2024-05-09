import VLCKit

//static ref is needed to avoid crash when mediaPlayer is stopped
struct VLCVideoPlayer {
    private static let mediaPlayer = VLCMediaPlayer()
    private static var mediaPlayerListener : VLCMediaPlayerListener?
    
    private static var onMuteChanged: (Bool) -> Void = {_ in }
    
    static func playMedia(url: String, startTimeMilliseconds: Int32, onMediaPlayerStateChanged: @escaping (VLCMediaPlayerState) -> Void, onTimeChanged: @escaping (Int, Int, Float) -> Void, onMuteChanged: @escaping (Bool) -> Void){
        mediaPlayerListener = VLCMediaPlayerListener(mediaPlayer: mediaPlayer, onStateChanged: onMediaPlayerStateChanged, onTimeChanged: onTimeChanged)
        mediaPlayer.delegate = mediaPlayerListener
    
        self.onMuteChanged = onMuteChanged
        mediaPlayer.media = VLCMedia(url: URL(string: url)!)
        mediaPlayer.play()
        
        if(startTimeMilliseconds > 0){
            DispatchQueue.main.asyncAfter(deadline: .now() + 1.5) {
                mediaPlayer.time = VLCTime(int: startTimeMilliseconds)
            }
        }
    }
    
    static func togglePlay(){
        if(mediaPlayer.state == .paused){
            mediaPlayer.play()
        }
        else{
            mediaPlayer.pause()
        }
    }
    
    static func moveBackward10Seconds(){
        mediaPlayer.jumpBackward(10)
    }
    
    static func moveForeward10Seconds(){
        mediaPlayer.jumpForward(10)
    }
    
    static func toggleMute(){
        mediaPlayer.audio!.isMuted = !mediaPlayer.audio!.isMuted
        self.onMuteChanged(mediaPlayer.audio!.isMuted)
    }
    
    static func setVolume(value: Int32){
        mediaPlayer.audio!.volume = value
    }
    
    static func goToPosition(value: Float){
        mediaPlayer.position = value
    }

    static func stop(){
        mediaPlayer.stop()
    }
    
    static func setView(view: NSView){
        mediaPlayer.videoAspectRatio = .none
        mediaPlayer.drawable = view
    }
    
    static func getDurationInSeconds() -> Int {
        return Int(truncating: mediaPlayer.media!.length.value) / 1000
    }
    
    static func castToDevice(device: VLCRendererItem){
        mediaPlayer.setRendererItem(device)
    }
    
    static func stopCastToDevice(){
        mediaPlayer.setRendererItem(nil)
    }
}

