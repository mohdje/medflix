
import logo from '../assets/medflix.png';

function NotAvailableOnMobilePage({ visible }) {
    const componentStyle = {
        position : 'fixed',
        zIndex: '10',
        width: '100vw',
        height: '100vh',
        backgroundColor: 'black',
        color: 'white',
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center',
        fontSize: '18px',
        fontWeight: '600'
    }

    if(visible){
        return (
            <div style={componentStyle}>
                <img alt="" src={logo} />
                <div>Sorry, it is not yet available on mobile.</div>
            </div>
        )
    }
    else 
        return null;
   
}

export default NotAvailableOnMobilePage;