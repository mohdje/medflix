function TitleAndContent({ title, content, justify }) {
    let justifyContent;
    if (justify === 'left')
        justifyContent = 'flex-start';
    else
        justifyContent = 'center';

    const containerStyle = {
        fontSize: '18px',
        fontWeight: '500',
        display: !!content ? 'flex' : 'none',
        alignItems: 'center',
        justifyContent: justifyContent,
        margin: '5px 0',
        height: '40px'
    }

    const titleStyle = {
        color: 'white',
        marginRight: '15px'
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