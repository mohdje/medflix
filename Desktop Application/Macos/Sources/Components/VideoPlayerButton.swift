//
//  VideoPlayerButton.swift
//  Medflix
//
//  Created by Mohamed on 25/01/2024.
//

import SwiftUI

struct VideoPlayerButton : View {
    private var onClick: () -> Void
    @Binding var imageName: String
    
    init(imageName: Binding<String>, onClick: @escaping () -> Void) {
        self.onClick = onClick
        _imageName = imageName
    }
       
    var body: some View{
        Button(action: onClick) {
            Image(systemName: imageName)
                .resizable()
                .frame(width: 40, height: 40)
                .foregroundColor(.white)
        }
        .buttonStyle(PlainButtonStyle())
        .frame(width: 50, height: 50)
    }
}

