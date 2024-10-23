import BaseButton from './BaseButton';

function MoreButton({ onClick, color, center }) {
    const content = <div>More</div>;

    return (
        <BaseButton content={content} color={color} centered={center} onClick={() => onClick()} />
    );
}

export default MoreButton;