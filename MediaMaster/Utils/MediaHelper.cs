using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace MediaMaster.Utils
{
    public static class MediaHelper
    {
        public static string SendGoogleSearchRequest(string searchString)
        {
            using (WebClient wc = new WebClient())
            {
                return wc.DownloadString(string.Format(Constants.GoogleSearchQueryUrl, searchString));
            }
        }
    }
}
