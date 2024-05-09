import SwiftUI

struct MediaInfoView : View {
    
    private var mediaInfo : MediaInfo
    
    init(mediaInfo: MediaInfo){
        self.mediaInfo = mediaInfo
    }
    
    func formatSecondsToHoursMinutes(seconds: Double) -> String {
        let hours = Int(seconds) /  3600
        let minutes = (Int(seconds) %  3600) /  60
        
        var result = ""
        
        if(hours > 0){
                result += "\(hours)h "
        }
        
        if(minutes > 0){
                result += "\(minutes)min"
        }
        
        return result
    }
    
    var body: some View{
        ZStack{
            VStack(alignment: .leading){
                Text(mediaInfo.title)
                    .bold()
                    .foregroundColor(.white)
                    .multilineTextAlignment(.leading)
                    .font(.system(size: 18))
                    .padding(.top, 100)
                    .padding(.bottom, 10)
                
                HStack{
                    Text(String(mediaInfo.year))
                        .padding(.trailing, 10)
                        .foregroundColor(.gray)
                        .font(.system(size: 15))
                    
                    Text(formatSecondsToHoursMinutes(seconds: mediaInfo.totalDuration))
                        .foregroundColor(.gray)
                        .font(.system(size: 15))
                }
                .padding(.bottom, 10)
                
                Text(mediaInfo.synopsis)
                    .foregroundColor(.white)
                    .multilineTextAlignment(.leading)
                    .font(.system(size: 14))
                    .padding(.trailing, 40)
            }.padding(.leading, 40)
            
        }.frame(maxWidth: .infinity, maxHeight: .infinity)
        .background(Color.black.opacity(0.3))
    }
}
