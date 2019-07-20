using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDL_Holla.Helpers
{
    internal static class Util
    {
        public static class Messages
        {
            public static string InvalidUrl = "Invalid URL, try again!";
            public static string InvalidFolderPath = "Invalid folder path, try again!";
            public static string MissingYoutubeDl = "Missing youtube-dl.exe, please review included instructions.";
            public static string NothingFound = "Nothing found to download, check your URL";
            public static string youtubedlFound = "Youtube-dl.exe found!";
            public static string youtubedlNotFound = "Youtube-dl.exe required (DOWNLOAD HERE)";
        }

        public static double BytesToMBs(long bytes)
        {
            return bytes / 1024f / 1024f;
        }
    }
}
