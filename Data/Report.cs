using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public class Report : BaseEntity
    {
        public string TargetId { get; set; }
        public long CommentId { get; set; } = -1;
        public long PostId { get; set; } = -1;

        public virtual IEnumerable<ReportUser> Users { get; set; }
    }
}
