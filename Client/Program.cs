using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiteCrawler;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Action<string> traverseAction = s => { };
            var traverseStrategy = new TraverseStrategy<string>(traverseAction, ResultGenerator);
            var crawler = new Crawler<string>(traverseStrategy);

            var results =  crawler.CrawlSite("http://www.spiegel.de", 1);

            foreach (var result in results)
            {
                Console.WriteLine(result);
            }

            
            Console.ReadLine();
        }

        private static string ResultGenerator(string contenct, uint depth)
        {
            return $"Die Tiefe {depth} bei der Seite {contenct}";
        }
    }
}
