import SwiftUI

struct VideoPlayerControlsView : View {
    
    @Binding var videoIsLoading : Bool
    @Binding var currentTime : Int
    @Binding var remainingTime : Int
    @Binding var videoProgression : Double
    @Binding var videoPlayerState : VLCMediaPlayerState
    @Binding var isMuted : Bool
    
    @State private var volumeValue = 70.0
    @State private var playPauseButtonImageName = VideoPlayerIcons.pause
    @State private var moveBackwardButtonImageName = VideoPlayerIcons.backward10
    @State private var moveForwardButtonImageName = VideoPlayerIcons.forward10
    @State private var soundButtonImageName = VideoPlayerIcons.speakerOn
    @State private var closeButtonImageName = VideoPlayerIcons.close
    @State var currentTimeText : String = ""
    @State var remainingTimeText : String = ""
    
    private var onPlayPauseButtonClick: () -> Void
    private var onMoveBackwardButtonClick: () -> Void
    private var onMoveForwardButtonClick: () -> Void
    private var onSoundButtonClick: () -> Void
    private var onVolumeChanged: (Double) -> Void
    private var onVideoProgressionChanged: (Double) -> Void
    private var onCloseClick: () -> Void
    
    @State private var keyboardEventsInitialized = false
    
    init(videoIsLoading: Binding<Bool>, videoPlayerState: Binding<VLCMediaPlayerState>, isMuted: Binding<Bool>, currentTime: Binding<Int>, remainingTime: Binding<Int>, videoProgression: Binding<Double>, onPlayPauseButtonClick: @escaping () -> Void, onMoveBackwardButtonClick: @escaping () -> Void, onMoveForwardButtonClick: @escaping () -> Void, onSoundButtonClick: @escaping () -> Void, onVolumeChanged: @escaping (Double) -> Void, onVideoProgressionChanged: @escaping (Double) -> Void, onCloseClick: @escaping () -> Void) {
        _videoIsLoading = videoIsLoading
        _currentTime = currentTime
        _remainingTime = remainingTime
        _videoProgression = videoProgression
        _videoPlayerState = videoPlayerState
        _isMuted = isMuted
        
        self.onPlayPauseButtonClick = onPlayPauseButtonClick
        self.onMoveBackwardButtonClick = onMoveBackwardButtonClick
        self.onMoveForwardButtonClick = onMoveForwardButtonClick
        self.onSoundButtonClick = onSoundButtonClick
        self.onVolumeChanged = onVolumeChanged
        self.onVideoProgressionChanged = onVideoProgressionChanged
        self.onCloseClick = onCloseClick
    }
    
    func toFormattedTimeString (milliseconds: Int) -> String {
        let seconds = Double(milliseconds) / 1000.0
        let date = Date(timeIntervalSince1970: seconds)
        
        let formatter = DateFormatter()
        formatter.timeZone = TimeZone(abbreviation: "UTC")
        formatter.dateFormat = "HH:mm:ss"
        
        return formatter.string(from: date)
    }
    
    var body: some View{
        VStack{
            HStack{
                Spacer()
                VideoPlayerButton(imageName: $closeButtonImageName, onClick: onCloseClick)
            }.padding(.trailing, 20)
            .padding(.top, 20)
            Spacer()
            if(!videoIsLoading){
                HStack(spacing: 20){
                    VideoPlayerTimeLabel(timeText: $currentTimeText)
                    VideoPlayerSlider(value: $videoProgression, color: NSColor.red, onValueChanged: onVideoProgressionChanged)
                    VideoPlayerTimeLabel(timeText: $remainingTimeText)
                }.padding(.leading, 20)
                .padding(.trailing, 20)
                
                HStack(spacing: 20){
                    VideoPlayerButton(imageName: $playPauseButtonImageName, onClick: onPlayPauseButtonClick)
                    VideoPlayerButton(imageName: $moveBackwardButtonImageName, onClick: onMoveBackwardButtonClick)
                    VideoPlayerButton(imageName: $moveForwardButtonImageName, onClick: onMoveForwardButtonClick)
                    VideoPlayerButton(imageName: $soundButtonImageName, onClick: onSoundButtonClick)
                    VideoPlayerSlider(value: $volumeValue, color: NSColor.white, onValueChanged: onVolumeChanged)
                        .frame(width: 100)
                    Spacer()
                }.padding(20)
            }
        }
        .frame(maxWidth: .infinity, maxHeight: .infinity)
        .background(Color.black.opacity(0.3))
        .transition(.opacity)
        .onChange(of: videoPlayerState) { value in
            switch videoPlayerState {
            case .playing:
                playPauseButtonImageName = VideoPlayerIcons.pause
            case .paused:
                playPauseButtonImageName = VideoPlayerIcons.play
            default:
                break
            }
        }.onChange(of: isMuted) { value in
            soundButtonImageName = value ? VideoPlayerIcons.speakerOff : VideoPlayerIcons.speakerOn
        }.onChange(of: currentTime) { value in
            playPauseButtonImageName = VideoPlayerIcons.pause
            currentTimeText = toFormattedTimeString(milliseconds: currentTime)
        }.onChange(of: remainingTime) { value in
            remainingTimeText = "-\(toFormattedTimeString(milliseconds: remainingTime))"
        }.onAppear{
            if(!keyboardEventsInitialized){
                keyboardEventsInitialized = true
                
                NSEvent.addLocalMonitorForEvents(matching: .keyDown) { event in
                    //space
                    if(event.keyCode == 49){
                        onPlayPauseButtonClick()
                    }
                    //left arrow
                    else if(event.keyCode == 123){
                        onMoveBackwardButtonClick()
                    }
                    //right arrow
                    else if(event.keyCode == 124){
                        onMoveForwardButtonClick()
                    }
                    //up arrow
                    else if(event.keyCode == 126){
                        let newVolume = volumeValue + 10
                        volumeValue = newVolume > 100 ? 100 : newVolume
                        onVolumeChanged(volumeValue)
                    }
                    //down arrow
                    else if(event.keyCode == 125){
                        let newVolume = volumeValue - 10
                        volumeValue = newVolume < 0 ? 0 : newVolume
                        onVolumeChanged(volumeValue)
                    }
                    //m letter
                    else if(event.keyCode == 41){
                        onSoundButtonClick()
                    }
                    
                    return event
                }
            }
        }
    }
}
