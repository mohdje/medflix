import BaseButton from './BaseButton';

function OpenButton({ onClick, visible }) {

    if(!visible)
        return null;

    const content = <div>Open</div>;

    return (
        <BaseButton content={content} centered color={"red"} onClick={() => onClick()} />
    );
}

export default OpenButton;