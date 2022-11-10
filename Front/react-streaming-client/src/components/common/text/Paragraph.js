function Paragraph({text}){

    const style = {
        fontSize: '17px',
        fontWeight: '500',
        color: 'white'
    }

    return(
        <p style={style}>{text}</p>
    )
}

export default Paragraph;