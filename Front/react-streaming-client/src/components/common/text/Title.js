function Title({text}){

    const style={
        fontSize: '28px',
        fontWeight: '500',
        color: 'white',
        textAlign: 'center'
    }

    return (
        <div style={style}>{text}</div>
    )
}

export default Title;