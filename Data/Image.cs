using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data
{
    public class Image : BaseEntity
    {
        public string Path { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Recipe Recipe { get; set; }
    }
}
