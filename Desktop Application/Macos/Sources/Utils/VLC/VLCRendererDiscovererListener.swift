import VLCKit

class VLCRendererDiscovererListener : NSObject, VLCRendererDiscovererDelegate {
    private var onRendererListChanged: ([VLCRendererItem]) -> Void
    private var availableRenderers : [VLCRendererItem]
    
    init(onRendererListChanged: @escaping ([VLCRendererItem]) -> Void ) {
        self.onRendererListChanged = onRendererListChanged
        self.availableRenderers = []
    }
    
    func rendererDiscovererItemAdded(_ rendererDiscoverer: VLCRendererDiscoverer, item: VLCRendererItem) {
        availableRenderers.append(item)
        onRendererListChanged(availableRenderers)
    }
    
    func rendererDiscovererItemDeleted(_ rendererDiscoverer: VLCRendererDiscoverer, item: VLCRendererItem) {
        availableRenderers = availableRenderers.filter({ rendererItem in
            rendererItem.name != item.name
        })
        onRendererListChanged(availableRenderers)
    }
}
