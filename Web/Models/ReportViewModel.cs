using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class ReportViewModel
    {
        public string UserId { get; set;}
        public int Amount { get; set; }
        public string ReportType { get; set; }
        public long ObjectId { get; set; }
    }
}
