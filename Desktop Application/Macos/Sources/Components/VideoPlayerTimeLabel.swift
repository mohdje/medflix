//
//  VideoPlayerTimeLabel.swift
//  Medflix
//
//  Created by Mohamed on 31/01/2024.
//

import SwiftUI

struct VideoPlayerTimeLabel : View {
    @Binding var timeText: String

    init(timeText: Binding<String>) {
        _timeText = timeText
    }
       
    var body: some View{
        Text(timeText).foregroundColor(.white)
            .multilineTextAlignment(.center)
            .font(.system(size: 14))
    }
}

