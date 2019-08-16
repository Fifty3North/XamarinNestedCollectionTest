using System;
using System.Collections.Generic;
using System.Text;

namespace NestedCollectionPerformanceTest.Models
{
    public class SearchResult
    {
        public List<Movie> Movies { get; }
        public int Page { get; }
        public int PageCount { get; }
        public int ResultCount { get; }

        public SearchResult(List<Movie> movies, int page, int pageCount, int resultCount)
        {
            Movies = movies;
            Page = page;
            PageCount = pageCount;
            ResultCount = resultCount;
        }
    }
}
