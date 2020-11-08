using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public class Post : BaseEntity
    {
        public virtual Recipe Recipe { get; set; }
        public long RecipeId { get; set; }

        public virtual IEnumerable<PostUser> Users { get; set; }
        public virtual IEnumerable<Rating> Ratings { get; set; }
        public virtual IEnumerable<Comment> Comments { get; set; }
    }
}
