using System;
using System.Collections.Generic;

namespace SiteCrawler
{
    public class TraverseStrategy<T> : ITraverseStrategy<T>
    {
        private readonly Action<string, uint> _traverseAction;
        private readonly Func<string, uint, T> _resultGenerator;
        private readonly PageAnalyzer _analyzer = new PageAnalyzer();

        public TraverseStrategy(Action<string, uint> traverseAction, Func<string, uint, T> resultGenerator)
        {
            _traverseAction = traverseAction;
            _resultGenerator = resultGenerator;
        }

        public IEnumerable<T> CrawlSite(string page, string startpage, uint depth, uint currentDepth)
        {
            _traverseAction.Invoke(page, currentDepth);
            yield return _resultGenerator(page, currentDepth);

            if (currentDepth >= depth) yield break;
            foreach (var p in CrawlSubsite(page, startpage, depth, currentDepth)) yield return p;
        }

        private IEnumerable<T> CrawlSubsite(string page, string startpage, uint depth, uint currentDepth)
        {
            foreach (var subpage in _analyzer.GetSubpagesInSameDomain(page, startpage))
            {
                foreach (var result in CrawlSite(subpage, startpage, depth, currentDepth + 1))
                {
                    yield return result;
                }
            }
        }
    }

    public interface ITraverseStrategy<T>
    {
        IEnumerable<T> CrawlSite(string page, string startpage, uint depth, uint currentDepth);
    }

    public class Crawler<T>
    {
        private readonly ITraverseStrategy<T> _traverseStrategy;

        public Crawler(ITraverseStrategy<T> traverseStrategy)
        {
            _traverseStrategy = traverseStrategy;
        }

        public IEnumerable<T> CrawlSite(string startPage, uint depth)
        {
            uint depthZero = 0;
            return _traverseStrategy.CrawlSite(startPage, startPage, depth, depthZero);
        }
    }
}
