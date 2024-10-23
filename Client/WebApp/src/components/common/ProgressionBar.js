function ProgressionBar({value, width}){

    const containerStyle = {
        width: width,
        backgroundColor: '#423737',
        height: '5px',
        borderRadius: '2px'
    }

    const progressionStyle = {
        width: value + '%',
        backgroundColor: '#e91919',
        height: '100%',
        borderRadius: '2px'
    }

    return (
        <div style={containerStyle}>
            <div style={progressionStyle}></div>
        </div>
    )
}

export default ProgressionBar;