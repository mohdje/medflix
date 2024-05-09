import SwiftUI

struct AppUpdateModalView : View {
    @State private var message = "A new version of Medflix is available, do you want to install it ?"
    @State private var showYesNoButtons = true
    @State private var showCloseButton = false
    @State private var updatePackageUrl : URL?
    
    private var onCloseClick : (URL?) -> Void
    private var appUpdateService : AppUpdateService
    
    init(appUpdateService: AppUpdateService, onCloseClick: @escaping (URL?) -> Void){
        self.onCloseClick = onCloseClick
        self.appUpdateService = appUpdateService
    }
    
    var body: some View {
        VStack{
            Text(message)
                .multilineTextAlignment(.center)
                .font(.system(size: 14))
                .padding(5)
                .foregroundColor(.white)
            
            HStack{
                if(showYesNoButtons){
                    Spacer()
                    Button("Yes"){
                        showYesNoButtons = false
                        message = "Downloading new version, please wait..."
                        appUpdateService.downloadNewversion(completion: {
                            success, zipURL in
                         
                            if success {
                                message = "The new version has been downloaded with success. Once you close this window, the application will restart to install the new version."
                                updatePackageUrl = zipURL
                                showCloseButton = true
                            } else {
                                message = "An error occured while downloading. Please try again later."
                                showCloseButton = true
                            }
                        })
                    }.background(Color.blue)
                    .cornerRadius(10)
                    .padding(5)
                    
                    Button("No"){
                        self.onCloseClick(updatePackageUrl)
                    }.background(Color.black)
                    .cornerRadius(10)
                    .padding(5)
                }
                
                if(showCloseButton) {
                    Button("Close"){
                        self.onCloseClick(updatePackageUrl)
                    }.background(Color.black)
                    .cornerRadius(10)
                    .padding(5)
                }
            }
        }.padding(10)
        .frame(width: 350, height: 120, alignment: .center)
    }
}
