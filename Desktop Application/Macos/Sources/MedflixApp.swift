import SwiftUI

@main
struct MedflixApp: App {
    @NSApplicationDelegateAdaptor(AppDelegate.self) var appDelegate
    
    @State private var showMainView = false
    @State private var showVideoPlayerView = false
    @State private var selectedVideoUrl = ""
    @State private var selectedSubtitlesPath = ""
    @State private var subtitlesFontSize = CGFloat(40)
    @State private var subtitlesOffset = 0
    
    @State private var showSubtitlesOptionsModal = false
    @State private var showAppUpdateModal = false
    @State private var showCastToDeviceModal = false
    
    @State private var qualitiesList: [VideoQualityOption] = []
    @State private var subtitlesList: [SubtitlesOption] = []
    @State private var mediaInfo: MediaInfo = MediaInfo()
    
    private let medflixApiService = MedflixApiService()
    private let appUpdateService = AppUpdateService()
    
    var body: some Scene {
        WindowGroup {
            ZStack{
                if(showMainView){
                    MainView(
                        onVideoPlayerRequested: {videoPlayerOptions in
                            qualitiesList = videoPlayerOptions.sources
                            subtitlesList = videoPlayerOptions.subtitles
                            selectedVideoUrl = videoPlayerOptions.sources.first(where: {$0.selected})?.url ?? ""
                            mediaInfo = videoPlayerOptions.watchedMedia
                            mediaInfo.currentTime = Double(videoPlayerOptions.resumeToTime ?? 0)
                            
                            medflixApiService.mediaType = videoPlayerOptions.mediaType
                            showVideoPlayerView = true
                        }, medflixApiService: medflixApiService)
                }
                else {
                    SplashScreenView()
                }
                
                if(showVideoPlayerView){
                    VideoPlayerView(videoUrl: $selectedVideoUrl,
                                    subtitlesPath: $selectedSubtitlesPath,
                                    subtitlesFontSize: $subtitlesFontSize,
                                    subtitlesOffset: $subtitlesOffset,
                                    mediaInfo: mediaInfo,
                                    medflixApiService: medflixApiService,
                                    onVideoPlayerStopped: {
                                        showVideoPlayerView = false
                                    })
                }
                
               
            }.onAppear(perform: {
                medflixApiService.startService()
                medflixApiService.isServiceReady(onComplete: {
                    isReady in
                    if(isReady){
                        showMainView = true
                        appUpdateService.isNewRealeaseAvailable(onDataFetched: {
                            newUpdateAvailable in
                            showAppUpdateModal = newUpdateAvailable
                        })
                    }
                }, tries: 3)
            })
            .onChange(of: showVideoPlayerView, perform: { value in
                appDelegate.showVideoPlayerCommands = value
            })
            .preferredColorScheme(.dark)
            .sheet(isPresented: $showSubtitlesOptionsModal, content: {
                SubtitlesOptionsModalView(subtitlesOffset: $subtitlesOffset, subtitlesSize: $subtitlesFontSize, onCloseClick: {showSubtitlesOptionsModal = false})
            })
            .sheet(isPresented: $showAppUpdateModal, content: {
                AppUpdateModalView(appUpdateService: appUpdateService, onCloseClick: { updatePackageURL in
                    if(updatePackageURL != nil){
                        appUpdateService.startExtractUpdate(packageURL: updatePackageURL!)
                    }
                    showAppUpdateModal = false
                })
            })
            .sheet(isPresented: $showCastToDeviceModal, content: {
                CastToDeviceModalView(onCloseClick: {
                    showCastToDeviceModal = false
                })
            })
            .frame(minWidth:  900, minHeight:  500)
        }
        .commands {
            QualitiesCommand(videoQualityOptions: qualitiesList, selectedVideoUrl: $selectedVideoUrl)
            SubtitlesCommand(subtitlesList: $subtitlesList, selectedSubtitlesPath: $selectedSubtitlesPath, onOptionsClick: {showSubtitlesOptionsModal = true})
            CastToDeviceCommand(onDevicesMenuClick: {
                showCastToDeviceModal = true
            })
        }
    }
}


