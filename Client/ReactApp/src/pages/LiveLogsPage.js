import "../style/css/live-logs-page.css";
import Button from "../components/common/Button";
import { useEffect, useRef, useState } from "react";
import { listenToLiveLogs, stopListenToLiveLogs } from "../services/api/logsApi";

function LiveLogsPage() {
    const [logs, setLogs] = useState([]);
    const [isPaused, setIsPaused] = useState(false);
    const logsContainerRef = useRef(null);

    useEffect(() => {
        if (isPaused) {
            stopListenToLiveLogs();
            return;
        }

        listenToLiveLogs(
            (logLine) => {
                setLogs((previousLogs) => [...previousLogs, logLine]);
            },
            (error) => {
                console.error("SSE error", error);
            }
        );

        return () => {
            stopListenToLiveLogs();
        };
    }, [isPaused]);

    useEffect(() => {
        if (logsContainerRef.current) {
            logsContainerRef.current.scrollTop = logsContainerRef.current.scrollHeight;
        }
    }, [logs]);

    return (
        <div className="live-logs-page">
            <h1>Live Logs</h1>
            <Button
                text={isPaused ? "Play" : "Pause"}
                color={isPaused ? "red" : "gray"}
                onClick={() => setIsPaused((previousValue) => !previousValue)}
            />
            <div className="live-logs-container" ref={logsContainerRef} role="log" aria-live="polite">
                {logs.length === 0 ? (
                    <div className="live-log-line">Waiting for logs...</div>
                ) : (
                    logs.map((line, index) => (
                        <div key={`${line}-${index}`} className="live-log-line">
                            {line}
                        </div>
                    ))
                )}
            </div>
        </div>
    );
}

export default LiveLogsPage;
