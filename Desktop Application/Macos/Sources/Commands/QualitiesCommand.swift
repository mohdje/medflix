import SwiftUI

struct QualitiesCommand: Commands{
    private var videoQualityOptions: [VideoQualityOption]
    @Binding private var selectedVideoUrl : String
   
    init(videoQualityOptions: [VideoQualityOption], selectedVideoUrl: Binding<String>){
        self._selectedVideoUrl = selectedVideoUrl
        
        let sortedQualityOptions = videoQualityOptions.sorted(by: { (q1, q2) -> Bool in
            return q1.quality.trimmingCharacters(in: .whitespacesAndNewlines).lowercased() < q2.quality.trimmingCharacters(in: .whitespacesAndNewlines).lowercased()
        })
        
        var qualitiesIndexes : [String: Int] = [:]
        
        self.videoQualityOptions = sortedQualityOptions.map { videoQualityOption in
            let key = videoQualityOption.quality.trimmingCharacters(in: .whitespacesAndNewlines).lowercased()
            
            let newIndex = qualitiesIndexes[key] == nil ? 0 : qualitiesIndexes[key]! + 1
            qualitiesIndexes[key] = newIndex
            
            let quality = newIndex == 0 ? videoQualityOption.quality : "\(videoQualityOption.quality) (\(String(describing: newIndex)))"
            
            return VideoQualityOption(url: videoQualityOption.url, quality: quality,  selected: videoQualityOption.selected)
        }
    }

    var body: some Commands {
        CommandMenu("Qualities") {
            ForEach(videoQualityOptions, id: \.self) { videoQualityOption in
                ButtonCommand(
                    text: videoQualityOption.quality,
                    selectedImage: "play.fill",
                    isSelected: selectedVideoUrl == videoQualityOption.url,
                    onClick: {
                        selectedVideoUrl = videoQualityOption.url
                    })
            }
        }
    }
}
