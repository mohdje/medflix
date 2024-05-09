import SwiftUI

struct CastToDeviceCommand: Commands{

    private var onDevicesMenuClick : () -> Void
    init(onDevicesMenuClick: @escaping () -> Void){
        self.onDevicesMenuClick = onDevicesMenuClick
    }
    
    var body: some Commands {
        CommandMenu("Cast") {
            ButtonCommand(
                text: "Devices...",
                selectedImage: "checkmark",
                isSelected: false,
                onClick: {
                    onDevicesMenuClick()
                })
        }
    }
}
