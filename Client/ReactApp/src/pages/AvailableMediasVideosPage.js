import MediaVideosPresentationList from "../components/media/list/MediasVideosList";
import Button from "../components/common/Button";
import Badge from "../components/common/Badge";
import CircularProgressBar from "../components/common/CircularProgressBar";
import ModalLoadingMessage from "../components/modal/ModalLoadingMessage";
import { getAvailableMediasVideos, deleteMediasVideos } from "../services/api/mediaVideosManagementApi";
import { useToast } from "../helpers/customHooks";
import { formatFileSize } from "../helpers/formatHelper";

import "../style/css/available-medias-videos-page.css";

import { useEffect, useRef, useMemo, useState } from "react";

function AvailableMediasVideosPage() {
    const availableMediasVideos = useRef([]);
    const [isLoading, setIsLoading] = useState(false);
    const [selectedVideosIds, setSelectedVideosIds] = useState([]);
    const [showDeletingModal, setShowDeletingModal] = useState(false);
    const [filterType, setFilterType] = useState(0);
    const [videosListKey, setVideosListKey] = useState(0);
    const showToast = useToast();

    useEffect(() => {
        loadAvailableMediasVideos();
    }, []);

    const filteredMediasVideos = useMemo(() => {
        if (filterType < 0 || !Array.isArray(availableMediasVideos.current) || availableMediasVideos.current.length === 0) {
            return [];
        }

        const filteredArray = availableMediasVideos.current.filter((media) => {
            if (filterType === 1) {
                return media.isMovie;
            } else if (filterType === 2) {
                return media.isSerie;
            }
            return media;
        });

        return filteredArray;
    }, [availableMediasVideos.current, filterType]);

    useEffect(() => {
        setSelectedVideosIds((prev) =>
            prev.filter((id) =>
                filteredMediasVideos.some((media) =>
                    media.videos.some((video) => video.id === id)
                ))
        );
    }, [filteredMediasVideos]);

    const totalFilesSize = useMemo(() => {
        if (filteredMediasVideos.length === 0) return null;

        const files = filteredMediasVideos.flatMap((media) => media.videos);
        const totalSize = files.reduce((acc, video) => acc + video.bytesLength, 0);

        return formatFileSize(totalSize);
    }, [filteredMediasVideos]);

    const loadAvailableMediasVideos = async () => {
        setIsLoading(true);
        availableMediasVideos.current = await getAvailableMediasVideos();
        setIsLoading(false);
    };

    const handleDeleteClick = async () => {
        setShowDeletingModal(true);
        const deletedFilesLength = await deleteMediasVideos(selectedVideosIds);
        setSelectedVideosIds([]);

        setShowDeletingModal(false);
        if (deletedFilesLength > 0) {
            var msg = deletedFilesLength === selectedVideosIds.length ? "All selected files have been deleted" : `${selectedVideosIds.length - deletedFilesLength} files have not been deleted`;
            showToast(msg);
        }
        loadAvailableMediasVideos();
    }

    const handleCancelClick = () => {
        setSelectedVideosIds([]);
        setVideosListKey((key) => key + 1);
    }

    return (
        <div className="available-medias-videos-page">
            <h1>Available Medias Videos</h1>
            <h3>{totalFilesSize ? `Total size: ${totalFilesSize}` : ''}</h3>
            {!isLoading && (
                <>
                    <div>
                        <Badge text="All" active={filterType === 0} onClick={() => setFilterType(0)} />
                        <Badge text="Movies" active={filterType === 1} onClick={() => setFilterType(1)} />
                        <Badge text="Series" active={filterType === 2} onClick={() => setFilterType(2)} />
                    </div>
                    <MediaVideosPresentationList key={videosListKey} mediasVideos={filteredMediasVideos} onVideoSelectionChanged={(videoId, isSelected) => {
                        setSelectedVideosIds(prev => {
                            if (isSelected) {
                                return [...prev, videoId];
                            } else {
                                return prev.filter(id => id !== videoId);
                            }
                        });
                    }} />
                </>
            )}
            <CircularProgressBar visible={isLoading} position="center" size="large" />
            <BottomBarActions visible={selectedVideosIds.length > 0} selectedVideosLength={selectedVideosIds.length} onDeleteClick={handleDeleteClick} onCancelClick={handleCancelClick} />
            <ModalLoadingMessage visible={showDeletingModal} loadingMessage={`Deleting ${selectedVideosIds.length} file(s)...`} />
        </div>
    );
}

export default AvailableMediasVideosPage;

function BottomBarActions({ visible, selectedVideosLength, onDeleteClick, onCancelClick }) {
    return (
        <div className={`bottom-bar-actions ${visible ? "visible" : ""}`}>
            <h3>{selectedVideosLength} video file(s) selected</h3>
            <div className="actions">
                <Button text="Delete" onClick={onDeleteClick} color="red" large />
                <Button text="Cancel" onClick={onCancelClick} color="gray" large />
            </div>
        </div>
    );
}
