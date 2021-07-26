using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebHostStreaming.Helpers
{
    public static class TorrentHelper
    {
        const string TORRENT_HEXA_SIGNATURE = "64383a616e6e6f756e6365";
        public static bool IsValidTorrentFile(string path)
        {
            var bytes = File.ReadAllBytes(path);
            var signature = new StringBuilder();
            for (int i = 0; i < 11; i++)
            {
                signature.Append(string.Format("{0:X2}", bytes[i]));
            }

            return TORRENT_HEXA_SIGNATURE.Equals(signature.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        public static void FixTorrentFile(string path)
        {          
            var bytes = File.ReadAllBytes(path);
            var signature = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                signature.Append(string.Format("{0:X2}", bytes[i]));
            }

            var startIndex = signature.ToString().IndexOf(TORRENT_HEXA_SIGNATURE, StringComparison.OrdinalIgnoreCase);

            var newBytes = new byte[bytes.Length - (startIndex / 2)];
            Array.Copy(bytes, (startIndex / 2), newBytes, 0, newBytes.Length);
            File.WriteAllBytes(path, newBytes);
        }
    }
}
