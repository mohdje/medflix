import "../../style/button.css";
import BookmarkIcon from '@material-ui/icons/Bookmark';

function BookmarkButton({ onClick, visible }) {
    return (
        <div className="standard-button grey" style={{display: visible ? '' : 'none'}} onClick={() => onClick()}>
            <BookmarkIcon />
            <div style={{ marginLeft: '5px' }}>Add bookmark</div>
        </div>
    )
}

export default BookmarkButton;