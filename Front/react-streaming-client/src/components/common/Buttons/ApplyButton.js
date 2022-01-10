import BaseButton from './BaseButton';

function ApplyButton({ onClick }) {
    const content = <div>Apply</div>;

    return (
        <BaseButton content={content} color={"red"} onClick={() => onClick()} />
    );
}

export default ApplyButton;