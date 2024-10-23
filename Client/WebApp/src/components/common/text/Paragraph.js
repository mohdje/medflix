function Paragraph({text, visible}){

    if(visible === false)
        return null;
        
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