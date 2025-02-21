using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.PaginationModels
{
    public class PaginationResponseModel<T> where T : class
    {
        public List<T> Data { get; set; }
        public int TotalCount { get; set; }
    }
}
