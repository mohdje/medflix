
import SwiftUI

struct SubtitlesCommand: Commands{
    @Binding private var subtitlesList: [SubtitlesOption]
    @Binding private var selectedSubtitlesPath : String
    private var onOptionsClick : () -> Void
    
    init(subtitlesList: Binding< [SubtitlesOption]>, selectedSubtitlesPath: Binding<String>, onOptionsClick: @escaping () -> Void){
        self._subtitlesList = subtitlesList
        self._selectedSubtitlesPath = selectedSubtitlesPath
        self.onOptionsClick = onOptionsClick
    }
    
    var body: some Commands {
        CommandMenu("Subtitles") {
            ForEach(subtitlesList.indices, id: \.self) { index in
                Menu{
                    ForEach(subtitlesList[index].subtitlesSourceUrls.indices, id: \.self) { subIndex in
                        ButtonCommand(
                            text: "\(subtitlesList[index].language) \(subIndex + 1)",
                            selectedImage: "checkmark",
                            isSelected: selectedSubtitlesPath == subtitlesList[index].subtitlesSourceUrls[subIndex],
                            onClick: {
                                selectedSubtitlesPath = subtitlesList[index].subtitlesSourceUrls[subIndex]
                            })
                    }
                }
                label: {
                    if(subtitlesList[index].subtitlesSourceUrls.contains(selectedSubtitlesPath)){
                        Image(systemName: "checkmark")
                    }
                    Text(subtitlesList[index].language)
                }
            }
            ButtonCommand(
                text: "No subtitles",
                selectedImage: "checkmark",
                isSelected: selectedSubtitlesPath == "",
                onClick: {
                    selectedSubtitlesPath = ""
                })
            Divider()
            Button("Options..."){
                self.onOptionsClick()
            }
        }
    }
}
