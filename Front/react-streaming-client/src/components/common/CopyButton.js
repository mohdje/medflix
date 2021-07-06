import "../../style/button.css";
import FilterNoneIcon from '@material-ui/icons/FilterNone';

function CopyButton({ onClick }) {
    return (
        <div className="standard-button grey" onClick={() => onClick()}>
            <FilterNoneIcon />
            <div style={{ marginLeft: '5px' }}>Copy</div>
        </div>
    )
}

export default CopyButton;