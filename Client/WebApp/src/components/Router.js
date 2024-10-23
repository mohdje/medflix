
import { useEffect, useState } from 'react';

function Router({ components, activeComponentId }) {

    const [activeComponent, setActiveComponent] = useState({});

    useEffect(()=>{
        var component = components.find(c => c.id === activeComponentId);
        if(component) setActiveComponent(component);
    },[components, activeComponentId]);

    return <div style={activeComponent?.containerStyle}>{activeComponent?.render}</div>;
}

export default Router;