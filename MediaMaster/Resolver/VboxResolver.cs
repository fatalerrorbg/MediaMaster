using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MediaMaster.Utils;

namespace MediaMaster.Resolver
{
    public class VboxResolver : MediaResolver
    {
        public override string ResolveByName(string name)
        {
            string response = MediaHelper.SendGoogleSearchRequest(string.Format("{0} {1}", name, Constants.Vbox7));
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(response);

            var splitted = doc.DocumentNode
                .Descendants("cite")
                .First()
                .InnerHtml
                .Split(new string[] { "</b", "<", "<b", ">", "b>", "\"" }, StringSplitOptions.RemoveEmptyEntries);

            return string.Join(string.Empty, splitted);
        }

        public override string ResolveByUrl(string url)
        {
            string response = MediaHelper.SendGoogleSearchRequest(url);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(response);

            string name = doc.DocumentNode.
                Descendants("h3").
                First(x => x.Attributes["class"] != null && x.Attributes["class"].Value == "r").
                Descendants().
                First().InnerText.Replace(" / VBOX7", "");

            return name;
        }
    }
}
