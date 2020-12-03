using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class ObjectViewModel
    {
        // Post(Title) / Comment(Creator)
        public string Title { get; set; }
        // Post(Description) / Comment(Text)
        public string Description { get; set; }
        // Only for posts
        public string ImagePath { get; set; } = string.Empty;
        public List<Ingredient> Ingredients { get; set; } = null;
        public List<Method> Methods { get; set; } = null;
        public List<Tip> Tips { get; set; } = null;
    }
}
