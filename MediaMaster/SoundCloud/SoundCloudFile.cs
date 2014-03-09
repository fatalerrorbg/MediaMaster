using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using MediaMaster.Utils;

namespace MediaMaster
{
    public class SoundCloudFile : MediaFile
    {
        public const string ResolveUrlFormat = "https://api.sndcdn.com/resolve?url={0}&_status_code_map%5B302%5D=200&_status_format=json&client_id=YOUR_CLIENT_ID";
        public const string FoundStatus = "302 - Found";
        public const string DownloadUrlFormat = "https://api.soundcloud.com/tracks/{0}/stream?consumer_key=htuiRd1JP11Ww0X72T1C3g&filter=all&order=created_at";
        public const string SoundXmlUrlFormat = "https://api.soundcloud.com/tracks/{0}?client_id=YOUR_CLIENT_ID";

        public SoundCloudFile(string url)
            :base(url, FileOrigin.SoundCloud)
        {
        }

        public override MediaFileMetadata InitializeMetadata()
        {
            string location = this.ResolveLocation();
            string trackId = this.GetTrackId(location);
            string downloadUrl = string.Format(DownloadUrlFormat, trackId);
            var xmlDoc = XDocument.Load(string.Format(SoundXmlUrlFormat, trackId));

            string fileName = xmlDoc.Descendants("title").First().Value;
            string thumbnailLink = xmlDoc.Descendants("artwork-url").First().Value;
            string description = xmlDoc.Descendants("description").First().Value;
            string duration = xmlDoc.Descendants("duration").First().Value;

            return new SoundCloudMetadata(this.Url, thumbnailLink, downloadUrl, fileName, trackId, description, duration);
        }

        private string GetTrackId(string location)
        {
            string tracks = "tracks";
            int tracksIndex = location.IndexOf(tracks) + tracks.Length + 1;
            int questionMarkIndex = location.LastIndexOf("?");
            string trackId = location.Substring(tracksIndex, location.Length - tracksIndex - (location.Length - questionMarkIndex));

            return trackId;
        }

        private string ResolveLocation()
        {
            string resolvedContent = MediaHelper.SendWebRequest(string.Format(ResolveUrlFormat, this.Url));
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dynamic responseJsonObject = serializer.Deserialize<object>(resolvedContent);
            if (responseJsonObject["status"] != FoundStatus || !((string)responseJsonObject["location"]).Contains("tracks"))
            {
                throw new ArgumentException("Sound not found, check if the url is correct " + this.Url);
            }

            string location = responseJsonObject["location"];
            return location;
        }
    }
}
