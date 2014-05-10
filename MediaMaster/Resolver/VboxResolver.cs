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
        //add \b
        protected const string Expression = "vbox7.com/play:[A-Za-z0-9]";

        public override IEnumerable<string> ResolveByName(string name)
        {
            return this.ResolveByNameCore(name, Constants.Vbox7, Expression);
        }

        public override string ResolveByUrl(string url)
        {
            HtmlDocument doc = this.GetResponseFromUrl(url);

            string name = doc.DocumentNode.
                Descendants("h3").
                First(x => x.Attributes["class"] != null && x.Attributes["class"].Value == "r").
                Descendants().
                First().InnerText.Replace(" / VBOX7", "");

            return name;
        }
    }
}
