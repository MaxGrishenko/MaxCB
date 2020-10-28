using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class PostViewModel
    {
        public string ImagePath { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public long RecipeId { get; set; }
        public long PostId { get; set; }
        public bool SubscribeFlag { get; set; }

        public ApplicationUser CreatorUser { get; set; }
        public ApplicationUser CurrentUser { get; set; }
    }
}
