using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public class ReportUser : BaseEntity
    {
        public Report Report { get; set; }
        public long ReportId { get; set; }

        public ApplicationUser User { get; set; }
        public string UserId { get; set; }
    }
}
