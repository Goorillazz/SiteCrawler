using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SiteCrawler
{
    public class PageAnalyzer
    {
        public IEnumerable<string> GetSubpagesInSameDomain(string startpage)
        {
            var htmlCode = ReadHtml(startpage);
            if (!string.IsNullOrEmpty(htmlCode))
            {
                var aTagPattern = @"(<a.*?>.*?</a>)";
                foreach (Match match in Regex.Matches(htmlCode, aTagPattern, RegexOptions.IgnoreCase))
                {
                    string hrefPattern = @"href=[^\s]+html";
                    var hrefMatch = Regex.Match(match.Value, hrefPattern, RegexOptions.IgnoreCase);
                    if (hrefMatch.Success)
                    {
                        var count = "href=".Length + 1;
                        var subpage = hrefMatch.Value.Substring(count);
                        yield return subpage;
                    }
                }
            }        
        }

        private static string ReadHtml(string page)
        {
            var htmlCode = string.Empty;
            using (var client = new WebClient())
            {
                try
                {
                    htmlCode = client.DownloadString(page);
                }
                catch (WebException e)
                {
                }
            }
            return htmlCode;
        }
    }
}
