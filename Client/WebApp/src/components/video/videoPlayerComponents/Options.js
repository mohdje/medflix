import { useEffect, useState } from 'react';


export function VideoOptions({ options, icon, onOptionChanged }) {

    const [menuOptions, setMenuOptions] = useState([]);
    const [menuSubOptions, setMenuSubOptions] = useState([]);
    const [showMenu, setShowMenu] = useState(false);
    const [showSubMenu, setShowSubMenu] = useState(false);

    const [selectedOption, setSelectedOption] = useState(null);
    const [selectedSubOption, setSelectedSubOption] = useState(null);

    const updateMenuOptions = (selectedOption, selectedSubOption) => {

        const oldSelectedOption = menuOptions.find(op => op.selected);
        const oldSelectedSubOption = selectedOption?.subOptions?.find(subOp => subOp.selected);

        if (selectedOption?.subOptions && !selectedSubOption)
            return;

        if (selectedOption?.subOptions
            && oldSelectedSubOption?.label === selectedSubOption?.label)
            return;

        if (!selectedOption?.subOptions && oldSelectedOption?.label === selectedOption.label)
            return;

        var updatedOptions = [];
        menuOptions.forEach(op => {
            var updatedOption = {
                label: op.label,
                selected: op.label === selectedOption.label,
                data: op.data
            };
            var updatedSubOptions = [];
            if (op.subOptions) {
                op.subOptions.forEach(subOp => {
                    var updatedsubOption =
                    {
                        label: subOp.label,
                        data: subOp.data,
                        selected: subOp.label === selectedSubOption?.label
                    }
                    updatedSubOptions.push(updatedsubOption);
                })
                updatedOption.subOptions = updatedSubOptions;
            }
            updatedOptions.push(updatedOption);
        });

        setMenuOptions(updatedOptions);

        setShowMenu(false);
        setShowSubMenu(false);

        var option = updatedOptions.find(op => op.selected);
        if (option.subOptions) onOptionChanged(option.subOptions.find(subOp => subOp.selected));
        else onOptionChanged(option);
    }

    const getOptionDisplay = (option) => {
        return  (<div key={option.label} className={"option " + (option.selected ? 'selected' : '')}
            onClick={() => {if(!option.subOptions)setSelectedOption(option)}}
            onMouseEnter={() => {if(option.subOptions){setSelectedOption(option);setMenuSubOptions(!option || !option.subOptions ? [] : option.subOptions)}}}
            >
            {option.label}
        </div>)
    }

    const getSubOptionDisplay = (subOption) => {
        return  (<div key={subOption.label} className={"option " + (subOption.selected ? 'selected' : '')}
            onClick={() => setSelectedSubOption(subOption)}
            >
            {subOption.label}
        </div>)
    }

    useEffect(() => {
        if (options) {
            setMenuOptions(options);
        }
    }, [options]);

    useEffect(() => {
        if(selectedOption && !selectedOption.subOptions)
            updateMenuOptions(selectedOption, null);
    }, [selectedOption]);

    useEffect(() => {
        if(selectedOption && selectedSubOption)
            updateMenuOptions(selectedOption, selectedSubOption);
    }, [selectedSubOption]);


    return (
        <div className="video-options-container" onMouseLeave={() => {setShowMenu(false);setShowSubMenu(false)}}>
            <div className={"video-options-menu " + (showMenu ? '' : 'hidden')}
                onMouseEnter={() => {setShowSubMenu(true)}}>
                {menuOptions.map(option => getOptionDisplay(option))}
            </div>
            <div className={"video-options-menu suboptions " + (showSubMenu && menuSubOptions && menuSubOptions.length > 0 ? '' : 'hidden')}
                onMouseLeave={() => {setShowSubMenu(false)}}>
                {menuSubOptions.map(subOption => getSubOptionDisplay(subOption))}
            </div>
            <VideoOptionsButton icon={icon} onClick={() => setShowMenu(!showMenu)} />
        </div>
    )
}

export function VideoOptionsButton({ icon, hide, onClick }) {
    return (
        <div className="video-options-btn" style={{ display: hide ? 'none' : '' }} onClick={() => onClick()}>
            {icon}
        </div>
    )
}


