
import { useEffect, useState } from 'react';

function Router({ components, activeComponentId }) {

    const [activeComponent, setActiveComponent] = useState({});

    const routerStyle = {
        height: '100%'
    }

    useEffect(()=>{
        var component = components.find(c => c.id === activeComponentId);
        if(component) setActiveComponent(component);
    },[components, activeComponentId]);

    return (
        <div style={routerStyle}>
            {activeComponent.render}
        </div>
    );
}

export default Router;