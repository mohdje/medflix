//
//  VideoPlayerSlider.swift
//  Medflix
//
//  Created by Mohamed on 30/01/2024.
//

import SwiftUI

struct VideoPlayerSlider: NSViewRepresentable {
    @Binding var value: Double
    var color: NSColor
    var onValueChanged: (Double) -> Void
    
    func makeCoordinator() -> Coordinator {
        Coordinator(self)
    }

    func makeNSView(context: Context) -> NSSlider {
        let slider = NSSlider()
        slider.minValue = 0.0
        slider.maxValue = 100.0
        slider.doubleValue = value
        slider.sliderType = .linear
        slider.numberOfTickMarks = 0
        
        let colorFilter = CIFilter(name: "CIFalseColor")!
            colorFilter.setDefaults()
            colorFilter.setValue(CIColor(cgColor: color.cgColor), forKey: "inputColor0")
        colorFilter.setValue(CIColor(cgColor: color.cgColor), forKey: "inputColor1")

        slider.contentFilters = [colorFilter]
        
        slider.target = context.coordinator
        slider.action = #selector(Coordinator.valueChanged(_:))
        return slider
    }

    func updateNSView(_ nsView: NSSlider, context: Context) {
        nsView.doubleValue = value
    }

    class Coordinator: NSObject {
        var parent: VideoPlayerSlider

        init(_ parent: VideoPlayerSlider) {
            self.parent = parent
        }

        @objc func valueChanged(_ sender: NSSlider) {
            parent.onValueChanged(sender.doubleValue)
        }
    }
}

