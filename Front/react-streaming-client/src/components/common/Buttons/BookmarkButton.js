import BaseButton from './BaseButton';
import BookmarkIcon from '@material-ui/icons/Bookmark';
import ControlPointOutlinedIcon from '@material-ui/icons/ControlPointOutlined';
import RemoveCircleOutlineOutlinedIcon from '@material-ui/icons/RemoveCircleOutlineOutlined';


export function AddBookmarkButton({ onClick, visible }) {

    return (<BookmarkButton 
                onClick={onClick} 
                visible={visible} 
                color={"grey"}
                insideIcon={<ControlPointOutlinedIcon style={{ fontSize: 16, color: 'grey' }} />}/>);
      
}

export function RemoveBookmarkButton({ onClick, visible }) {

    return (<BookmarkButton 
                onClick={onClick} 
                visible={visible} 
                color={"grey"}
                insideIcon={<RemoveCircleOutlineOutlinedIcon style={{ fontSize: 16, color: 'grey' }} />}/>);

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
            top: '50%',
            left: '50%',
            transform: 'translate(-50%, -50%)',
            zIndex: 3
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
