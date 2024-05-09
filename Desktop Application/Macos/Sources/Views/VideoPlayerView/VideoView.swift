import SwiftUI

struct VideoView: NSViewRepresentable {
    func makeNSView(context: Context) -> NSView {
        return NSView()
    }
    
    func updateNSView(_ nsView: NSView, context: Context) {
        VLCVideoPlayer.setView(view: nsView)
    }
}

