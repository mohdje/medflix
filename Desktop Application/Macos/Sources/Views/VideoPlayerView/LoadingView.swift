

import SwiftUI

struct LoadingView : View {
    @Binding var showSpinner : Bool
    @Binding var message : String
    
    @State private var rotation = 0.0
    
    init(showSpinner: Binding<Bool>, message: Binding<String>) {
        self._showSpinner = showSpinner
        self._message = message
    }
    var body: some View{
        VStack{
            if(message != ""){
                Text(message)
                    .padding(10)
                    .foregroundColor(.white)
                    .font(.system(size: 20))
                    .background(Color.black.opacity(0.3))
                    .cornerRadius(10)
            }
            
            if(showSpinner){
                Image("loading_spinner")
                    .resizable()
                    .frame(width: 60, height: 60)
                    .foregroundColor(.red)
                    .rotationEffect(.degrees(rotation))
                    .onAppear {
                        withAnimation(.linear(duration: 1)
                                        .repeatForever(autoreverses: false)) {
                            rotation = 360.0
                        }
                    }.onDisappear{
                        rotation = 0.0
                    }
            }
        }
    }
}
