using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SiteCrawler;

namespace Client
{
    class Program
    {
        public static string FolderName { get; } = "Level_";

        static void Main(string[] args)
        {
            string startPage = args[0];
            
            uint depth = Convert.ToUInt32(args[1]);

            CreateDirectories(depth);
            
            var traverseStrategy = new TraverseStrategy<string>(SaveSite, ResultGenerator);
            var crawler = new Crawler<string>(traverseStrategy);
            var results =  crawler.CrawlSite(startPage, depth);

            foreach (var result in results)
            {
                Console.WriteLine(result);
            }
            
            Console.ReadLine();
        }

        private static void CreateDirectories(uint depth)
        {
            for (int i = 0; i <= depth; i++)
            {                
                Directory.CreateDirectory($"{FolderName}{i}");
            }
        }

        private static void SaveSite(string currentPage, uint depth)
        {            
            var fileName = CreateNema(currentPage);
            var pathAndFile = $"{FolderName}{depth}\\" + fileName + ".html";

            if (File.Exists(pathAndFile))
                return;

            using (var client = new WebClient())
            {
                client.DownloadFile(currentPage, pathAndFile);
            }
        }

        private static string ResultGenerator(string contenct, uint depth)
        {
            return $"Die Tiefe {depth} bei der Seite {contenct}";
        }

        private static string CreateNema(string page)
        {
            return page.Replace("/", string.Empty)
                       .Replace(".", string.Empty)
                       .Replace(":", string.Empty);
        }


    }
}
