using MonoTorrent.Client;
using System.Threading.Tasks;

namespace WebHostStreaming.Torrent
{
    public class TorrentDownloadStatusWatcher
    {
        private readonly TorrentManager torrentManager;
        private bool watchProgress;
        public bool FoundPeers { get; private set; }
        public bool DownloadStarted { get; private set; }
        public bool DownloadInProgress { get; private set; }
        public TorrentDownloadStatusWatcher(TorrentManager torrentManager)
        {
            this.torrentManager = torrentManager;
        }

        public void StartWatchingProgress()
        {
            watchProgress = true;
            torrentManager.PeersFound += TorrentManager_PeersFound;
            torrentManager.TorrentStateChanged += TorrentManager_TorrentStateChanged;
        }

        public void StopWatchingProgress()
        {
            watchProgress = false;
            torrentManager.PeersFound -= TorrentManager_PeersFound;
            torrentManager.TorrentStateChanged -= TorrentManager_TorrentStateChanged;
        }

        private void TorrentManager_PeersFound(object sender, PeersAddedEventArgs e)
        {
            FoundPeers = true;
        }

        private void TorrentManager_TorrentStateChanged(object sender, TorrentStateChangedEventArgs e)
        {
            if (e.NewState == TorrentState.Downloading)
            {
                while (watchProgress)
                {
                    DownloadStarted = e.TorrentManager.PartialProgress > 0;
                    DownloadInProgress = e.TorrentManager.PartialProgress > 0.5;
                    watchProgress = !DownloadInProgress;

                    Task.Delay(2000).Wait();
                }
            }
        }
    }
}
