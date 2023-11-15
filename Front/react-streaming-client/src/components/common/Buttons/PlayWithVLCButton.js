import PlayArrowIcon from '@material-ui/icons/PlayArrow';

import ModalWindow from "../../modal/ModalWindow";
import Title from '../text/Title';
import SecondaryInfo from '../text/SecondaryInfo';
import CopyButton from "./CopyButton";
import BaseButton from "./BaseButton";

import AppServices from "../../../services/AppServices";
import { useEffect, useState, useRef } from 'react';

function PlayWithVLCButton({ onClick, videoUrl }) {
    const [showModal, setShowModal] = useState(false);
    const [isWebApp, setIsWebApp] = useState(null);

    useEffect(() => {
        if (isWebApp === null) {
            AppServices.appInfoApiService.isDesktopApplication((isDesktopApp) => {
                setIsWebApp(!isDesktopApp);
            });
        }
    }, []);

    const content = (<div style={{ display: 'flex', justifyContent: 'center' }}>
        <PlayArrowIcon />
        <div style={{ marginLeft: '5px' }}>Play With VLC</div>
    </div>);

    const handleClick = () => {
        setShowModal(true);
        if (onClick) onClick();
    }

    if (isWebApp)
        return (
            <div>
                <BaseButton content={content} color={"orange"} onClick={() => handleClick()} />
                <ModalForVlc visible={showModal} videoUrl={videoUrl} onCloseClick={() => setShowModal(false)} />
            </div>
        );
    else
        return null;
}

export default PlayWithVLCButton;

function ModalForVlc({ visible, videoUrl, onCloseClick }) {

    const containerStyle = {
        backgroundColor: 'black',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        height: '300px',
        width: '500px',
        padding: '0 20px',
        margin: 'auto',
        color: 'white',
        borderRadius: '10px',
        background: 'linear-gradient(342deg, rgba(42,42,42,1) 0%, rgba(0,0,0,1) 65%)'
    };

    const container = (
        <div style={containerStyle}>
            <ModalContent videoUrl={videoUrl} launchPlay={visible} onCloseClick={() => onCloseClick()} />
        </div>
    );

    return <ModalWindow visible={visible} content={container} />;
}

function ModalContent({ videoUrl, onCloseClick }) {
    const inputLinkUrlRef = useRef(null);

    const onCopyClick = () => {
        if (inputLinkUrlRef?.current) {
            inputLinkUrlRef.current.select();
            document.execCommand("copy");
            window.getSelection().removeAllRanges();
        }
    }

    const centerStyle = (vertical) => {
        return {
            display: 'flex',
            alignItems: 'center',
            height: '100%',
            justifyContent: 'space-around',
            flexDirection: vertical ? 'column' : ''
        }
    }

    return (
        <div>
            <Title text={"Play with VLC"} />
            <SecondaryInfo text={"To play the video in VLC, click on 'Copy', open VLC then open a network stream and paste the copied link."} />
            <input ref={inputLinkUrlRef}
                style={{ background: 'none', border: 'none', outline: 'none', color: 'grey', width: '100%', textAlign: 'center', textOverflow: 'ellipsis', margin: '10px 0' }}
                type="text"
                readOnly={true}
                value={Boolean(videoUrl) ? videoUrl : ''} />
            <div style={centerStyle()}>
                <CopyButton onClick={() => onCopyClick()} />
                <BaseButton content={"Close"} color={"red"} onClick={() => onCloseClick()} />
            </div>
        </div>);

}


