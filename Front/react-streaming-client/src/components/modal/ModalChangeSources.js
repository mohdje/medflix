import "../../style/vlc-player-instructions.css";

import ModalWindow from "./ModalWindow";
import Settings from "../Settings";
import { useState } from "react";

function ModalChangeVfSource({visible, setting, onCloseClick}) {
    const [changesApplied, setChangesApplied] = useState(false);

    const content = () => {
        if (!visible) return null;
        else return <Settings settings={[setting]} onApplyClick={()=> setChangesApplied(true)}/>;
    }

    return (
        <ModalWindow visible={visible} 
            content={content()} 
            onCloseClick={() =>{onCloseClick(changesApplied); setChangesApplied(false); } } 
            />
    )
}

export default ModalChangeVfSource;