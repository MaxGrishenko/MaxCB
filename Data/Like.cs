using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public class Like : BaseEntity
    {
        public virtual ApplicationUser User { get; set; }
        public string UserId { get; set; }

        public virtual Post Post { get; set; }
        public long PostId { get; set; }
    }
}
