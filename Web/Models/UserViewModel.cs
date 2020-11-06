using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class UserViewModel
    {
        public string userId { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string role { get; set; }
    }
}
