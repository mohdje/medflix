using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebHostStreaming.Extensions
{
    public static class StringExtensions
    {
        public static string ToMD5Hash(this string text)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(text);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public static string DecodeBase64(this string base64string)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(base64string));
        }

        public static string ToTorrentFolderPath(this string url)
        {
            return Path.Combine(Helpers.AppFolders.TorrentsFolder, url.ToMD5Hash());
        }

        public static string GetContentType(this string filePath)
        {
            var videoFormat = Path.GetExtension(filePath);

            if (videoFormat == ".mp4")
                return "video/mp4";
            else if (videoFormat == ".avi")
                return "video/x-msvideo";
            else if (videoFormat == ".mkv")
                return "video/x-matroska";
            else
                return null;
        }
    }
}
