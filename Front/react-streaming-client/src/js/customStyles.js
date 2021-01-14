function fadeTransition(visible, speedTransition){

    var speed = speedTransition ? speedTransition + 's' : '0.5s';
    return {
        visibility: visible ? 'visible' : 'hidden',
        opacity: visible ? '1' : '0',
        transition: "all " + speed
    }
}

export default fadeTransition;