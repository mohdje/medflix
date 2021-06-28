import { useEffect, useState, useRef } from 'react';
import { useOnClickOutside } from '../../../js/customHooks';

export function VideoOptions({ options, icon, onOptionChanged }) {

    const [menuOptions, setMenuOptions] = useState([]);
    const [showMenu, setShowMenu] = useState(false);
    const [optionsDisplay, setOptionsDisplay] = useState('');
    const optionsWindowRef = useRef(null);

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

        var option = updatedOptions.find(op => op.selected);
        if (option.subOptions) onOptionChanged(option.subOptions.find(subOp => subOp.selected));
        else onOptionChanged(option);
    }

    const setSubOptionsVisibility = (option, visible) => {
        var element = document.getElementById('suboption-' + option.label)
        if (!element)
            return;

        if (visible) element.classList.remove("hidden");
        else element.classList.add("hidden");
    }

    const getOptionsDisplay = (options) => {
        return options.map(option =>
        (<div key={option.label} className={"option " + (option.selected ? 'selected' : '')}
            onClick={() => updateMenuOptions(option)}
            onMouseOver={() => setSubOptionsVisibility(option, true)}
            onMouseLeave={() => setSubOptionsVisibility(option, false)}>
            {option.label}
            {getSubOptionsDisplay(option)}
        </div>))
    }

    const getSubOptionsDisplay = (option) => {
        if (!option?.subOptions)
            return null;
        else {
            return (
                <div id={'suboption-' + option.label} className="video-options-menu suboptions hidden">
                    {option.subOptions.map(subOption =>
                    (<div key={subOption.label} className={"option " + (subOption.selected ? 'selected' : '')}
                        onClick={(event) => { updateMenuOptions(option, subOption); event.stopPropagation() }}>
                        {subOption.label}
                    </div>)
                    )}
                </div>
            );
        }
    }

    useEffect(() => {
        if (options) {
            setMenuOptions(options);
        }
    }, [options]);

    useEffect(() => {
        if (menuOptions?.length && menuOptions.length > 0) {
            setOptionsDisplay(getOptionsDisplay(menuOptions));
        }
    }, [menuOptions]);

    useOnClickOutside(optionsWindowRef, () => setShowMenu(false));

    return (
        <div ref={optionsWindowRef} className="video-options-container">
            <div className={"video-options-menu " + (showMenu ? '' : 'hidden')}>
                {optionsDisplay}
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


