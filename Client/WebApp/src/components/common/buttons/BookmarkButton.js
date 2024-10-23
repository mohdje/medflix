import BaseButton from './BaseButton';
import BookmarkIcon from '@material-ui/icons/Bookmark';
import AddIcon from '@material-ui/icons/Add';
import RemoveIcon from '@material-ui/icons/Remove';


export function AddBookmarkButton({ onClick, visible }) {

    return (<BookmarkButton 
                onClick={onClick} 
                visible={visible} 
                color={"grey"}
                insideIcon={<AddIcon style={{ fontSize: 22, color: 'white' }} />}/>);
      
}

export function RemoveBookmarkButton({ onClick, visible }) {

    return (<BookmarkButton 
                onClick={onClick} 
                visible={visible} 
                color={"dark-red"}
                insideIcon={<RemoveIcon style={{ fontSize: 22, color: 'white' }} />}/>);

}

function BookmarkButton({ onClick, visible, color, insideIcon }) {

    if (visible) {
        const contentStyle = {
            position: 'relative',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center'
        };

        const insideIconStyle = {
            position: 'absolute',
            top: '-2px',
            right: '-2px',
            padding: '1px',
            zIndex: 3,
            width: '15px',
            height: '15px',
            borderRadius: '100%',
            backgroundColor: 'rgba(45,44,44,0.5)',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center'
        };

        const content = (
            <div style={contentStyle}>
                <BookmarkIcon style={{ fontSize: 28 }} />
                <div style={insideIconStyle}>{insideIcon}</div>
            </div>
        )

        return (
            <BaseButton content={content} color={color} onClick={() => onClick()} />
        );
    }
    else return null;

}
