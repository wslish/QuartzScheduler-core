using System;
using System.Collections.Generic;
using System.Text;

namespace QuartzScheduler.Model
{
    public class PaginationInfo
    {
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int ActualPage { get; set; }
        public int TotalPages { get; set; }

    }
}
