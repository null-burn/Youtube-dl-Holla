using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDL_Holla.Helpers
{
    public class PreReq
    {
        public static string youtubedlExe = @"Youtube-dl\youtube-dl.exe";
        public bool YoutubeDLExists()
        {
            if (File.Exists(Directory.GetCurrentDirectory() + @"\" + youtubedlExe))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
