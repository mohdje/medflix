function Rating({ rating, size }) {

    const style = {
        color: "white",
        fontSize: size === "small" ? "15px" : "20px",
        fontWeight: "bold",
        margin: "0 auto",
        backgroundColor: "#52525263",
        borderRadius: "50%",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        height: size === "small" ? "35px" : "50px",
        width: size === "small" ? "35px" : "50px",
    };

    return (
        <div style={style}>{rating}</div>
    );
}

export default Rating;