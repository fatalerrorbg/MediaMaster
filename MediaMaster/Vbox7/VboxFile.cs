using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using HtmlAgilityPack;

namespace MediaMaster
{
    public class VboxFile : WebFile<VboxFileMetadata>
    {
        public const string InfoUrl = "http://www.vbox7.com/etc/ext.do?key={0}&antiCacheStamp=5316311";
        public const string DownloadUrlKey = "flv_addr";
        public const string ThumbnailKey = "jpg_addr";
        public const string SubsEnabledKey = "subsEnabled";
        public const string RelatedKey = "related";
        public const string IdElementKey = "play-video-title";

        public VboxFile(string url)
            :base(url, FileOrigin.Vbox7)
        {

        }

        public override bool IsValid
        {
            get 
            {
                return Path.GetExtension(this.Metadata.DownloadLink) != SupportedConversionFormats.Flv;
            }
        }

        protected override VboxFileMetadata InitializeMetadata()
        {
            string infoResponse = string.Empty;
            string videoId = this.ParseVideoId();
            string fileName = string.Empty;

            using(WebClient wc = new WebClient())
	        {
		        string response = wc.DownloadString(string.Format(Constants.GoogleSearchQueryUrl, this.Url));
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(response);

                fileName = doc.DocumentNode.Descendants("h3").First().Descendants().First().InnerText.Replace(" / VBOX7", "");
	        }

            using (WebClient wc = new WebClient())
            {
                infoResponse = wc.DownloadString(string.Format(InfoUrl, videoId));
            }

            string[] keyValuePairsRaw = infoResponse.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string,  string> fileInfo = new Dictionary<string, string>();
            foreach (string pair in keyValuePairsRaw)
            {
                string[] splittedPairs = pair.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (splittedPairs.Length >= 2)
                {
                    fileInfo[splittedPairs[0]] = splittedPairs[1];
                }
            }

            return new VboxFileMetadata(this.Url, fileInfo[VboxFile.ThumbnailKey], fileInfo[VboxFile.DownloadUrlKey], videoId, fileName);
        }

        private string ParseVideoId()
        {
            int idStartIndex = this.Url.LastIndexOf(':');
            string id = string.Empty;
            try
            {
                id = this.Url.Substring(idStartIndex + 1, this.Url.Length - idStartIndex - 1);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new InvalidDataException("Video url is not valid, not id can be parsed out of it", ex);
            }

            return id;
        }
    }
}