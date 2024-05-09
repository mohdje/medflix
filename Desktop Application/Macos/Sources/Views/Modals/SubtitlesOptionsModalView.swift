import SwiftUI

struct SubtitlesOptionsModalView: View {
    @Binding private var subtitlesOffset : Int
    @Binding private var subtitlesSize : CGFloat
    
    @State private var offsetValue : String = ""
    @State private var fontSizeValue : String = ""
    
    private var onCloseClick : () -> Void
    
    init(subtitlesOffset: Binding<Int>, subtitlesSize: Binding<CGFloat>, onCloseClick: @escaping ()-> Void) {

        self._subtitlesSize = subtitlesSize
        self._subtitlesOffset = subtitlesOffset
        self.onCloseClick = onCloseClick
    }
    
    var body: some View {
        VStack {
            Text("Subtitles font size:")
            TextField("between 30 and 50", text: $fontSizeValue)
                .padding(.top, 5)
                .padding(.bottom, 10)
                .onChange(of: fontSizeValue, perform: { value in
                    if let newSize = Int(value){
                        if(newSize >= 30 && newSize <= 50){
                            subtitlesSize = CGFloat(newSize)
                        }
                    }

                    return
                })
            
            Text("Subtitles offset (in seconds):")
            TextField("offset", text: $offsetValue)
                .padding(.top, 5)
                .padding(.bottom, 10)
                .onChange(of: offsetValue, perform: { value in
                    if let newOffset = Int(value){
                        subtitlesOffset = newOffset
                    }

                    return
                })
            
            Button("Close"){
               onCloseClick()
            }
            .background(Color.blue)
            .cornerRadius(10)
            .padding()
        }
        .padding()
        .onAppear{
            offsetValue = String(subtitlesOffset)
            fontSizeValue = String(Int(subtitlesSize))
        }
    }
}
