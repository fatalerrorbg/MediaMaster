using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MediaMaster.Utils;

namespace MediaMaster.Resolver
{
    public class VboxResolver : MediaResolver
    {
        public override IEnumerable<string> ResolveByName(string name)
        {
            string response = MediaHelper.SendGoogleSearchRequest(string.Format("{0} {1}", name, Constants.Vbox7));
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(response);

            var cites = doc.DocumentNode
                .Descendants("cite")
                .Where(x =>
                {
                    string cleanText = this.GetCleanCiteText(x.InnerHtml);
                    if (string.IsNullOrEmpty(cleanText))
                    {
                        return false;
                    }

                    string expression = "vbox7.com/play:[A-Za-z0-9]";
                    return Regex.Match(cleanText, expression).Success;
                })
                .Select(x => this.GetCleanCiteText(x.InnerHtml));
            
            return cites;
        }

        private string GetCleanCiteText(string rawString)
        {
            if (string.IsNullOrEmpty(rawString))
            {
                return string.Empty;
            }

            return string.Join(string.Empty,
                rawString.Split(new string[] { "</b", "<", "<b", ">", "b>", "\"" },
                StringSplitOptions.RemoveEmptyEntries));
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
