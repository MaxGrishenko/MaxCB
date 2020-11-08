using System;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Data
{
    public class ApplicationUser : IdentityUser
    {
        public string ImagePath { get; set; }
        public virtual ICollection<PostUser> Posts { get; set; }
        public virtual ICollection<ReportUser> Reports {get; set; }

        public virtual ICollection<Recipe> Recipes { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
