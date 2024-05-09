final class AppDelegate: NSObject, NSApplicationDelegate {
    var showVideoPlayerCommands = false
    var onAppQuit : () -> Void = {}
    
    func applicationWillUpdate(_ notification: Notification) {
        if let menu = NSApplication.shared.mainMenu {
            for menuItem in menu.items {
                if(menuItem.title == "File" || menuItem.title == "View" || menuItem.title == "Window" || menuItem.title == "Help" || menuItem.title == "Edit"){
                    menuItem.isHidden = true
                }
                else if(menuItem.title == "Qualities" || menuItem.title == "Subtitles" || menuItem.title == "Cast" ){
                    menuItem.isHidden = !showVideoPlayerCommands
                }
            }
        }
    }
    
    func applicationWillTerminate(_ notification: Notification) {
        let task = Process()
        task.launchPath = "/bin/sh"
        task.arguments = ["-c", "pkill -9 MedflixWebHost"]
        task.launch()
    }
}
