function fadeTransition(visible, speedTransition, zIndex){

    var speed = speedTransition ? speedTransition + 's' : '0.5s';

    const transitionStyle = {
            visibility: visible ? 'visible' : 'hidden',
            opacity: visible ? '1' : '0',
            transition: "all " + speed
    }

    if(zIndex)
        transitionStyle.zIndex = visible ? zIndex : '-1';
    

    return transitionStyle;
}

export default fadeTransition;