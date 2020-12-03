using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data
{
    public class Recipe : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Category { get; set; }
        public int PrepTime { get; set; }
        public int CookTime { get; set; }
        public int Marinade { get; set; }
        public int Difficulty { get; set; }
        public string ImagePath { get; set; }

        public virtual Post Post { get; set; }
        
        public virtual ApplicationUser User { get; set; }
        public string UserId { get; set; }

        public virtual IEnumerable<Ingredient> Ingredients { get; set; }
        public virtual IEnumerable<Method> Methods { get; set; }
        public virtual IEnumerable<Tip> Tips { get; set; }
    }
}
