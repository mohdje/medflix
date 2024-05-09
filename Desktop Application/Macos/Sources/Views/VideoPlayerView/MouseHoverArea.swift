//
//  MouseHoverArea.swift
//  Medflix
//
//  Created by Mohamed on 04/02/2024.
//

import SwiftUI

struct MouseHoverArea : View {
    private var onMouseHover: () -> Void
    
    init(onMouseHover: @escaping () -> Void) {
        self.onMouseHover = onMouseHover
    }
    var body: some View{
        VStack {
            Text("")
                .frame(minWidth: 0, maxWidth: .infinity, minHeight: 100, maxHeight: 100)
                .onHover(perform: { hovering in
                    onMouseHover()
                })
            Spacer()
            Text("")
                .frame(minWidth: 0, maxWidth: .infinity, minHeight: 100, maxHeight: 100)
                .onHover(perform: { hovering in
                    onMouseHover()
                })
        }
    }
}
