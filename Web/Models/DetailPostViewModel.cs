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
        public string CategoryName { get; set; }
        public int PrepTime { get; set; }
        public int CookTime { get; set; }
        public int Marinade { get; set; }
        public string DifficultyName { get; set; }
        public string ImagePath { get; set; }
        public List<Ingredient> Ingredients { get; set; }
        public List<Method> Methods { get; set; }
        public List<Tip> Tips { get; set; }
    }
}
