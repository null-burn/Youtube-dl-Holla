using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDL_Holla.Helpers
{
    public class Url
    {
        public static bool Valid(string url)
        {
            //https://stackoverflow.com/questions/7578857/how-to-check-whether-a-string-is-a-valid-http-url
            Uri uriResult;
            bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return result;
        }
    }
}
