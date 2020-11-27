using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class DetailPostViewModel
    {
        public long PostId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Category { get; set; }
        public int PrepTime { get; set; }
        public int CookTime { get; set; }
        public int Marinade { get; set; }
        public int Difficulty { get; set; }
        public string ImagePath { get; set; }
        public List<Ingredient> Ingredients { get; set; }
        public List<Method> Methods { get; set; }
        public List<Tip> Tips { get; set; }
    }
}
