using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Core.Model.Common
{
    public class PagedResult<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)System.Math.Ceiling(TotalCount / (double)PageSize);
        public IEnumerable<T> Data { get; set; }
    }
}
