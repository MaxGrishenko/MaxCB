using System;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Data
{
    public class ApplicationUser : IdentityUser
    {
        public virtual Image Image { get; set; }
        public long? ImageId { get; set; }


        public virtual ICollection<PostUser> Posts { get; set; }
        public virtual ICollection<Recipe> Recipes { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
