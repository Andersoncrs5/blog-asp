using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.utils.Responses
{
    public class ResponsePagination<T>
    {
    
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public long TotalCount { get; set; }
        public bool HasNextPage => PageIndex < TotalPages;
        public bool HasPreviousPage => PageIndex > 1;
        public List<Link>? _links { get; set; }

        public ResponsePagination(IEnumerable<T> items, long count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
            Items = items;
            _links = new List<Link>();
        }

        public class Link
        {
            public string Href { get; set; }
            public string Rel { get; set; } 
            public string Method { get; set; }

            public Link(string href, string rel, string method)
            {
                Href = href;
                Rel = rel;
                Method = method;
            }
        }
    }
}