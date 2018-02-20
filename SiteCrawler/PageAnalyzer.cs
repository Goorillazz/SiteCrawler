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
        public IEnumerable<string> GetSubpagesInSameDomain(string page, string startPage)
        {
            var htmlCode = ReadHtml(page);
            if (!string.IsNullOrEmpty(htmlCode))
            {
                var aTagPattern = @"(<a.*?>.*?</a>)";
                foreach (Match match in Regex.Matches(htmlCode, aTagPattern, RegexOptions.IgnoreCase))
                {
                    string hrefPattern = @"href=[^\s]+html";
                    var hrefMatch = Regex.Match(match.Value, hrefPattern, RegexOptions.IgnoreCase);
                    if (hrefMatch.Success && ValidSubpage(GetHrefValue(hrefMatch), startPage))
                    {
                        var subpage = CreateSubpageLink(hrefMatch, page);

                        yield return subpage;
                    }
                }
            }        
        }

        private static string GetHrefValue(Match hrefMatch)
        {
            var count = "href=".Length + 1;
            return hrefMatch.Value.Substring(count);
        }

        private static string CreateSubpageLink(Match hrefMatch,string page)
        {            
            var subpage = GetHrefValue(hrefMatch);

            if (subpage.StartsWith("/"))
                return page + subpage;

            return subpage;                       
        }

        private bool ValidSubpage(string subpage, string startPage)
        {
            return subpage.StartsWith("/") || subpage.StartsWith(startPage);
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
