import SwiftUI

struct SplashScreenView : View {
    @State private var version : String = Bundle.main.infoDictionary?["CFBundleShortVersionString"] as? String ?? ""
    
    var body: some View {
        VStack{
            Spacer()
            Image("logo")
            Text(version)
                .multilineTextAlignment(.center)
                .font(.system(size: 14))
                .padding(.top, 30)
                .foregroundColor(.white)
            Spacer()
        }
        .frame(maxWidth: .infinity, maxHeight: .infinity, alignment: /*@START_MENU_TOKEN@*/.center/*@END_MENU_TOKEN@*/)
        .background(Color.black)
    }
}
