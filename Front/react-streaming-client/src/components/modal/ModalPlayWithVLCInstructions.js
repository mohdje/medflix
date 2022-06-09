import "../../style/vlc-player-instructions.css";

import ModalWindow from "./ModalWindow";
import CopyButton from "../common/buttons/CopyButton";
import PlayButton from "../common/buttons/PlayButton";

import MoviesAPI from "../../js/moviesAPI";

import { useEffect, useState, useRef } from 'react';

function ModalPlayWithVLCInstructions({ sources, movieDetails, visible, isDesktopApp, onCloseClick }) {

    const [links, setLinks] = useState([]);
    const [selectedLinkUrl, setSelectedLinkUrl] = useState('');
    const [showLinkCopiedMessage, setShowLinkCopiedMessage] = useState(false);
    const inputLinkUrlRef = useRef(null);
    const [showVlcNotInstalledMsg, setShowVlcNotInstalledMsg]= useState(false);

    const onLinkClick = (linkIndex) => {
        let updatedLinks = [];
        links.forEach(link => {
            link.selected = false;
            updatedLinks.push(link);
        });
        updatedLinks[linkIndex].selected = true;
        setSelectedLinkUrl(updatedLinks[linkIndex].linkUrl);
        setLinks(updatedLinks);
        setShowLinkCopiedMessage(false);
    };

    const onCopyClick = () => {
        if (inputLinkUrlRef?.current) {
            inputLinkUrlRef.current.select();
            document.execCommand("copy");
            window.getSelection().removeAllRanges();
            setShowLinkCopiedMessage(true);
        }
    };

    const onPlayClick = () => {
        if (selectedLinkUrl) {
            MoviesAPI.saveLastSeenMovie(movieDetails)
            MoviesAPI.playWithVlc(selectedLinkUrl, null, ()=>{
                setShowVlcNotInstalledMsg(true);
            });
        }
    };

    useEffect(() => {
        setShowLinkCopiedMessage(false);
        if (sources && sources.length > 0) {
            let newLinks = [];
            sources.forEach(source => {
                var qualities = newLinks.filter(link => link.quality === source.quality);
                newLinks.push({
                    quality: source.quality + (qualities.length > 0 ? " (" + qualities.length + ")" : ''),
                    linkUrl: MoviesAPI.apiStreamUrl(source.downloadUrl),
                    selected: false
                });
            });
            newLinks[0].selected = true;
            setSelectedLinkUrl(newLinks[0].linkUrl);
            setLinks(newLinks);
        }

    }, [sources]);

    const instructionsForWebApp = (
        <div>
            <div className="instruction">
                <div>2. Click on 'Copy' button just below to copy the following link in the clipboard :</div>
                <div className="copy-link">
                    <input ref={inputLinkUrlRef} type="text" readOnly={true} value={selectedLinkUrl} />
                    <CopyButton onClick={() => onCopyClick()} />
                    <div className={"link-copied-message " + (showLinkCopiedMessage ? '' : 'hidden')}>Link copied into clipboard !</div>
                </div>
            </div>
            <div className="instruction">
                <div>3. Open VLC Player on your desktop, click on Media &#62; Open network stream &#62; Network tab &#62; Paste the link and click on Play</div>
            </div>
        </div>
    );

    const instructionsForDesktopApp = (
        <div className="instruction">
            <div>2. Click on 'Play' button just below to open VLC player and start playing the movie :</div>
            <PlayButton onClick={() => onPlayClick()} center={true} color={"orange"} />
            <div style={{display:  showVlcNotInstalledMsg ? '' : 'none'}}>It seems you don't have VLC installed on your computer</div>
        </div>
    );

    const instructions = () => {
        const commonInstructions = (
            <div className="instruction">
                <div>1. Click on one of the available qualities below :</div>
                <div className="qualities">
                    {links.map((link, index) =>
                        <div key={index}
                            className={"standard-button " + (link.selected ? 'red' : 'grey')}
                            onClick={() => onLinkClick(index)}>
                            {link.quality}
                        </div>)}
                </div>
            </div>
        )
        return (
            <div className="vlc-player-instructions-container">
                {commonInstructions}
                {isDesktopApp ? instructionsForDesktopApp : instructionsForWebApp}
            </div>
        )
    }


    return (
        <ModalWindow visible={visible} content={instructions()} onCloseClick={() => onCloseClick()} />
    )
}

export default ModalPlayWithVLCInstructions;