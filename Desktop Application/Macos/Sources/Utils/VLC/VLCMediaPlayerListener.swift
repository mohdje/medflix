import SwiftUI
import VLCKit

class VLCMediaPlayerListener : NSObject, VLCMediaPlayerDelegate {
    var mediaPlayer: VLCMediaPlayer
    private var onStateChanged: (VLCMediaPlayerState) -> Void
    private var onTimeChanged: (Int, Int, Float) -> Void
    
    init(mediaPlayer: VLCMediaPlayer, onStateChanged: @escaping (VLCMediaPlayerState) -> Void, onTimeChanged: @escaping (Int, Int, Float) -> Void) {
        self.mediaPlayer = mediaPlayer
        self.onStateChanged = onStateChanged
        self.onTimeChanged = onTimeChanged
    }
    
    func mediaPlayerStateChanged(_ aNotification: Notification) {
        self.onStateChanged(mediaPlayer.state)
    }
    
    func mediaPlayerTimeChanged(_ aNotification: Notification) {
        self.onTimeChanged(Int(mediaPlayer.time.intValue), Int(-mediaPlayer.remainingTime!.intValue), mediaPlayer.position)
    }
}
