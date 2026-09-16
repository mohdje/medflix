import "../style/css/error-logs-page.css";
import { getErrorLogs, deleteErrorLogs } from "../services/api/errorLogsApi";
import ModalErrorLogInfos from "../components/modal/ModalErrorLogInfos";
import Button from "../components/common/Button";
import CircularProgressBar from "../components/common/CircularProgressBar";
import { formatDateTime } from "../helpers/formatHelper";
import { useEffect, useState } from "react";

function ErrorLogsPage() {
    const [errorLogs, setErrorLogs] = useState([]);
    const [selectedErrorLog, setSelectedErrorLog] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        async function fetchErrorLogs() {
            try {
                const logs = await getErrorLogs();
                const sortedLogs = Array.isArray(logs)
                    ? [...logs].sort((firstLog, secondLog) => (
                        new Date(secondLog.dateTime).getTime() - new Date(firstLog.dateTime).getTime()
                    ))
                    : [];
                setErrorLogs(sortedLogs);
            }
            catch (error) {
                console.error("Failed to fetch error logs:", error);
                setErrorLogs([]);
            }
            finally {
                setLoading(false);
            }
        }

        fetchErrorLogs();
    }, []);

    async function handleDeleteErrorLogs() {
        try {
            setLoading(true);
            setErrorLogs([]);
            setSelectedErrorLog(null);

            await deleteErrorLogs();
        }
        catch (error) {
            console.error("Failed to delete error logs:", error);
        }
        finally {
            setLoading(false);
        }
    }

    return (
        <div className="error-logs-page">
            <h1>Error Logs</h1>
            <Button
                text="Delete logs"
                color="red"
                onClick={handleDeleteErrorLogs}
                disabled={loading || errorLogs.length === 0}
            />
            {loading ? (
                <CircularProgressBar size="large" visible={loading} />
            ) : errorLogs.length === 0 ? (
                <p>No errors to display.</p>
            ) : (
                <table>
                    <thead>
                        <tr>
                            <th>Timestamp</th>
                            <th>Message</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        {errorLogs.map((log, index) => (
                            <tr key={log.id ?? `${log.message ?? "error"}-${index}`}>
                                <td>{formatDateTime(log.dateTime)}</td>
                                <td>{log.message ?? "Unknown error"}</td>
                                <td>
                                    <button onClick={() => setSelectedErrorLog(log)}>+</button>
                                </td>

                            </tr>
                        ))}
                    </tbody>
                </table>
            )}
            <ModalErrorLogInfos
                visible={selectedErrorLog !== null}
                errorLog={selectedErrorLog}
                onCloseClick={() => setSelectedErrorLog(null)}
            />
        </div>
    );
}

export default ErrorLogsPage;