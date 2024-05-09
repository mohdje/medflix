import SwiftUI
import VLCKit

struct CastToDeviceModalView : View {
    private var onCloseClick : () -> Void
    private var rendererDiscoverer : VLCRendererDiscoverer
    @State private var rendererDiscovererListener : VLCRendererDiscovererListener = VLCRendererDiscovererListener(onRendererListChanged: {_ in})
    @StateObject private var availableCastDevices : CastDevices = CastDevices()
    
    @State private var showLoading : Bool = true
    
    init(onCloseClick: @escaping () -> Void){
        self.onCloseClick = onCloseClick
        
        let name = VLCRendererDiscoverer.list()?[0].name ?? ""
        
        rendererDiscoverer = VLCRendererDiscoverer.init(name: name)!
    }
    
    var body: some View {
        VStack{
            if(showLoading){
                Text("Searching devices")
                    .multilineTextAlignment(.center)
                    .font(.system(size: 14))
                    .padding(10)
                    .foregroundColor(.white)
                ProgressView()
                    .progressViewStyle(CircularProgressViewStyle())
            }
            else{
                ForEach(availableCastDevices.items, id: \.self) { device in
                    Button(action: {
                        VLCVideoPlayer.castToDevice(device: device)
                        onCloseClick()
                    }, label: {
                        Text(device.name).multilineTextAlignment(.center)
                            .font(.system(size: 14))
                            .foregroundColor(.white)
                    }).buttonStyle(PlainButtonStyle())
                }
            }
            
            Button("Don't cast"){
                VLCVideoPlayer.stopCastToDevice()
                rendererDiscoverer.stop()
                onCloseClick()
            }.background(Color.black)
            .cornerRadius(10)
            .padding(10)
                        
        }.onAppear(){
            rendererDiscovererListener = VLCRendererDiscovererListener(onRendererListChanged: {
                rendererList in
                availableCastDevices.items = rendererList
                showLoading = rendererList.isEmpty
            })
            
            rendererDiscoverer.delegate = rendererDiscovererListener
            rendererDiscoverer.start()
        }
        .padding(10)
        .frame(width: 350, height: 120, alignment: .center)
    }
}

