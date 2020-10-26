using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public class PostUser : BaseEntity
    {
        public Post Post { get; set; }
        public long PostId { get; set; }

        public ApplicationUser User { get; set; }
        public string UserId { get; set; }
    }
}
