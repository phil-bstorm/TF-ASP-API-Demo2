using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAPI.Domain.Models
{
    public class PaginationParams
    {
        public int Page { get; set; } = 0;
        public int PageSize { get; set; } = 3;
    } 
}
