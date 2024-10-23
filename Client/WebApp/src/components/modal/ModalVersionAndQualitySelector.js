import ModalWindow from "./ModalWindow";
import "../../style/modal-version-quality-selector.css";
import { useEffect, useState } from 'react';

function ModalVersionAndQualitySelector({ visible, torrents, onQualityClick }) {
    const [content, setContent] = useState(null);

    useEffect(() => {
        if (torrents?.length > 0) {
            const torrentsByLanguage = [];
            const languages = [...new Set(torrents.map(t => t.languageVersion))];

            languages.forEach(language => {
                const sortedTorrents = torrents.filter(torrent => torrent.languageVersion === language).sort((t1, t2) => {
                    const [quality1, quality2] = [t1.quality.toLowerCase().trim(), t2.quality.toLowerCase().trim()]
                    return quality1 === quality2 ? 0 : (quality1 < quality2 ? -1 : 1);
                });

                torrentsByLanguage.push({
                    languageVersion: language,
                    torrents: sortedTorrents
                })
            });

            const computeQualityLabel = (torrent) => {
                const torrentIndex = torrents.findIndex(t => t.downloadUrl === torrent.downloadUrl);
                const qualityIndex = torrents.slice(0, torrentIndex).filter(t => t.quality === torrent.quality).length;
                return qualityIndex > 0 ? `${torrent.quality} (${qualityIndex})` : torrent.quality;
            }

            const versionsAndQualitiesTable = (<div className="modal-version-quality-container">
                {torrentsByLanguage.map((l, i) => (
                    <div className="version-column" key={i}>
                        <div className="version-name">{l.languageVersion}</div>
                        <div className="qualities-list">{l.torrents.map((t, j) => (
                            <div className={"quality" + (t.selected ? " selected" : "")} key={j} onClick={() => onQualityClick(t)}>{computeQualityLabel(t)}</div>
                        ))}</div>
                    </div>
                ))}
            </div>);

            setContent(versionsAndQualitiesTable)
        }

    }, [torrents]);

    const onCloseClick = () => {
        const selectedTorrent = torrents.find(torrent => torrent.selected);
        onQualityClick(selectedTorrent);
    }

    return (
        <ModalWindow visible={visible} content={content} onCloseClick={() => onCloseClick()} />
    )
}

export default ModalVersionAndQualitySelector;