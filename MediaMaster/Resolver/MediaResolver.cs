using HtmlAgilityPack;
using MediaMaster.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MediaMaster.Resolver
{
    public abstract class MediaResolver
    {
        public abstract IEnumerable<string> ResolveByName(string name);

        protected virtual IEnumerable<string> ResolveByNameCore(string name, string additionalParameters, string expressionToMatchWhenFilteringCites)
        {
            string response = MediaHelper.SendGoogleSearchRequest(string.Format("{0} {1}", name, Constants.Vbox7));
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(response);

            var cites = this.GetCitesFromHtml(doc, expressionToMatchWhenFilteringCites);
            return cites;
        }

        protected virtual IEnumerable<string> GetCitesFromHtml(HtmlDocument doc, string expressionToMatch)
        {
            var cites = doc.DocumentNode
                .Descendants("cite")
                .Where(x =>
                {
                    string cleanText = this.GetCleanCiteText(x.InnerHtml);
                    if (string.IsNullOrEmpty(cleanText))
                    {
                        return false;
                    }

                    return Regex.IsMatch(cleanText, expressionToMatch);
                })
                .Select(x => this.GetCleanCiteText(x.InnerHtml));

            return cites;
        }

        protected virtual string GetCleanCiteText(string rawString)
        {
            if (string.IsNullOrEmpty(rawString))
            {
                return string.Empty;
            }

            return string.Join(string.Empty,
                rawString.Split(new string[] { "</b", "<", "<b", ">", "b>", "\"" },
                StringSplitOptions.RemoveEmptyEntries));
        }

        public abstract string ResolveByUrl(string url);

        protected HtmlDocument GetResponseFromUrl(string url)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(MediaHelper.SendGoogleSearchRequest(url));

            return doc;
        }
    }
}
