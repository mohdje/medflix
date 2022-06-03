import "../style/button.css";
import "../style/settings.css";
import fadeTransition from "../js/customStyles.js";

import CircularProgressBar from "./common/CircularProgressBar";
import ApplyButton from "./common/buttons/ApplyButton";
import MoviesAPI from "../js/moviesAPI";

import { useEffect, useState } from 'react';

function SettingsPage({settings, onApplyClick}) {
    const [loading, setLoading] = useState(true);
    const [settingsSections, setSettingsSections] = useState([]);
    const settingsSectionsResult = [];

    const nbSections = settings.length;

    useEffect(() => {
        if(settings.includes('vo')){
            MoviesAPI.getVOMovieServices((services) => {
                if (services?.length > 0) {
                    addSection('VO movies services', 'vo', services, 1);
                }
            });
        }

        if(settings.includes('subs')){
            MoviesAPI.getSubtitlesMovieServices((services) => {
                if (services?.length > 0) {
                    addSection('Subtitles services', 'subs', services, 2);
                }
            });
        }
      
        if(settings.includes('vf')){
            MoviesAPI.getVFMovieServices((services) => {
                if (services?.length > 0) {
                    addSection('VF movies services', 'vf', services, 3);
                }
            });
        }
       
    }, []);

    const addSection = (title, sectionId, services, order) => {
        settingsSectionsResult.push(
            {
                title: title,
                services: services,
                order: order,
                id: sectionId
            }
        );
        if (settingsSectionsResult.length === nbSections) {
            let sortedSections = [...settingsSectionsResult];
            sortedSections.sort((a, b) => { return a.order - b.order })
            setSettingsSections(sortedSections);
            setLoading(false);
        }
    };

    const selectService = (serviceToSelect, settingsSectionId) => {
        if (settingsSections && settingsSections.length > 0) {
            var updatedSettingsSections = [...settingsSections];
            var section = updatedSettingsSections.filter(s => s.id === settingsSectionId)[0];
            var service = section.services.filter(s => s.id === serviceToSelect.id);

            if (service && service.length === 1) {
                section.services.forEach(s => { s.selected = false });
                service[0].selected = true;
                setSettingsSections(updatedSettingsSections);
            }
        }
    };

    const applyChanges = () => {
        setLoading(true);
        if (onApplyClick) onApplyClick();

        const selectedServices = {
            vo: settingsSections.find(s => s.id === 'vo')?.services.find(s => s.selected).id,
            vf: settingsSections.find(s => s.id === 'vf')?.services.find(s => s.selected).id,
            subs: settingsSections.find(s => s.id === 'subs')?.services.find(s => s.selected).id
        };
        MoviesAPI.saveSelectedServices(selectedServices , () => { 
            setTimeout(()=>{
                setLoading(false)
            }, 1000)});
    };

    return (
        <div className="settings-container">
            <CircularProgressBar color={'white'} size={'80px'} position={"center"} visible={loading} />
            <div style={fadeTransition(!loading)}>
                {settingsSections.map((settingsSection) =>
                    <ServicesSection
                        key={settingsSection.id}
                        sectionTitle={settingsSection.title}
                        services={settingsSection.services}
                        onClick={(service) => selectService(service, settingsSection.id)}
                    />)}
                <div className="footer">
                    <ApplyButton onClick={() => applyChanges()} />
                </div>
            </div>
        </div>
    );
}

export default SettingsPage;

function ServicesSection({ sectionTitle, services, onClick }) {
    return (
        <div className="section">
            <div className="title">{sectionTitle}</div>
            <div className="options">{services.map((service, index) =>
                <ThreeStatesButton
                    key={index}
                    title={service.description}
                    available={service.available}
                    selected={service.selected}
                    onClick={() => onClick(service)} />)}
            </div>
        </div>
    );
}

function ThreeStatesButton({ title, available, selected, onClick }) {
    if (available)
        return (<div className={"standard-button " + (selected ? "red" : "grey")} onClick={() => onClick()}>{title}</div>);
    else
        return (
            <div className="unavailable">
                <div>{title}</div>
                <div className="tooltip">Service unavailable</div>
            </div>
        );
}