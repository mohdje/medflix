import BaseButton from './BaseButton';
import BookmarkIcon from '@material-ui/icons/Bookmark';

function BookmarkButton({ onClick, visible }) {

    if (visible) {
        const content = (<div style={{ display: 'flex', justifyContent: 'center' }}>
            <BookmarkIcon />
            <div style={{ marginLeft: '5px' }}>Add bookmark</div>
        </div>);

        return (
            <BaseButton content={content} color={"grey"} onClick={() => onClick()} />
        );
    }
    else return null;

}

export default BookmarkButton;