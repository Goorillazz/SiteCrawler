using System;
using System.Collections.Generic;

namespace SiteCrawler
{
    public class TraverseStrategy<T> : ITraverseStrategy<T>
    {
        private readonly Action<string> _traverseAction;
        private readonly Func<string, uint, T> _resultGenerator;
        private readonly PageAnalyzer _analyzer = new PageAnalyzer();

        public TraverseStrategy(Action<string> traverseAction, Func<string, uint, T> resultGenerator)
        {
            _traverseAction = traverseAction;
            _resultGenerator = resultGenerator;
        }

        public IEnumerable<T> CrawlSite(string startPage, uint depth, uint currentDepth)
        {
            _traverseAction.Invoke(startPage);
            yield return _resultGenerator(startPage, currentDepth);

            if (currentDepth >= depth) yield break;
            foreach (var p in CrawlSubsite(startPage, depth, currentDepth)) yield return p;
        }

        private IEnumerable<T> CrawlSubsite(string page, uint depth, uint currentDepth)
        {
            foreach (var subpage in _analyzer.GetSubpagesInSameDomain(page))
            {
                foreach (var result in CrawlSite(subpage, depth, currentDepth + 1))
                {
                    yield return result;
                }
            }
        }
    }

    public interface ITraverseStrategy<T>
    {
        IEnumerable<T> CrawlSite(string startPage, uint depth, uint currentDepth);
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
            return _traverseStrategy.CrawlSite(startPage, depth, depthZero);
        }
    }
}
