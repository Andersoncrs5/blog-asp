using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Blog.utils
{
    public class PaginatedList<T>
    {
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public long TotalCount { get; set; }
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
        public List<Link>? _links { get; set; }

        private PaginatedList(IEnumerable<T> items, long count, int pageIndex, int pageSize)
        {
            Items = items;
            TotalCount = count;
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            _links = new List<Link>();
        }

        public static async Task<PaginatedList<T>> CreateAsync(
            IQueryable<T> source, int pageIndex, int pageSize)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 10; 

            long count = await source.CountAsync(); 
            
            List<T>? items = await source
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedList<T>(items, count, pageIndex, pageSize);
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