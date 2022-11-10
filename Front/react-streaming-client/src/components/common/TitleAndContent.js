function TitleAndContent({ title, content, justify }) {
    let justifyContent;
    if(justify === 'left')
        justifyContent = 'flex-start';
    else 
        justifyContent = 'center';

    const containerStyle = {
        fontSize: '18px',
        fontWeight: '500',
        display: 'flex',
        alignItems: 'center',
        justifyContent: justifyContent,
        margin: '10px 0',
    }

    const titleStyle = {
        color: 'white',
        marginRight : '15px'
    }

    const contentStyle = {
        color: "#959595",
        display: "flex"
    }

    return (
        <div style={containerStyle}>
            <div style={titleStyle}>{title}</div>
            <div style={contentStyle}>{content}</div>
        </div>
    )

}

export default TitleAndContent;