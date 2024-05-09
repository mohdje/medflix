import SwiftUI

struct VideoSubtitleView : View {
    @Binding var subtitleText : String
    @Binding var textSize: CGFloat
    
    private let shadowWidth = CGFloat(1)
    var body: some View {
        if(subtitleText != ""){
            VStack{
                Spacer()
                ZStack {
                    ZStack {
                        Text(subtitleText).offset(x:  shadowWidth, y:  shadowWidth).multilineTextAlignment(.center)
                        Text(subtitleText).offset(x: -shadowWidth, y: -shadowWidth).multilineTextAlignment(.center)
                        Text(subtitleText).offset(x: -shadowWidth, y:  shadowWidth).multilineTextAlignment(.center)
                        Text(subtitleText).offset(x:  shadowWidth, y: -shadowWidth).multilineTextAlignment(.center)
                    }
                    .foregroundColor(.black)
                    .font(.system(size: textSize))
                    Text(subtitleText)
                        .foregroundColor(.white)
                        .font(.system(size: textSize))
                        .multilineTextAlignment(.center)
                }.padding(.bottom, 50)
            }
            .padding(.leading, 100)
            .padding(.trailing, 100)
        }
    }
}
