import BaseButton from './BaseButton';
import FilterNoneIcon from '@material-ui/icons/FilterNone';

function CopyButton({ onClick }) {
    const content = (<div style={{ display: 'flex', justifyContent: 'center' }}>
        <FilterNoneIcon />
        <div style={{ marginLeft: '5px' }}>Copy</div>
    </div>);

    return (
        <BaseButton content={content} color={"grey"} onClick={() => onClick()} />
    );
}

export default CopyButton;