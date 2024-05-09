import SwiftUI

struct ButtonCommand : View {
    private var text: String
    private var isSelected: Bool
    private var selectedImage: String
    private var onClick: () -> Void
    
    init(text: String, selectedImage: String, isSelected: Bool, onClick: @escaping ()-> Void) {
        self.isSelected = isSelected
        self.text = text
        self.onClick = onClick
        self.selectedImage = selectedImage
    }
    
    var body: some View{
        Button(action: {
            self.onClick()
        }) {
            if(isSelected){
                Image(systemName: selectedImage)
            }
            Text(text)
        }
    }
   
}
