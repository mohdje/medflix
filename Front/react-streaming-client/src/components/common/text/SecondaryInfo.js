

function SecondaryInfo({text, center}){
    const style = {
        fontSize: "18px",
        fontWeight: "500",
        color: "#959595",
        textAlign: center ? "center" : "left",
        margin: '10px 0'
    }

    return (
        <div style={style}>{text}</div>
    )
}

export default SecondaryInfo;