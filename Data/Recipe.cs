using System;

namespace Data
{
    public class Recipe : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        // FK?
        public int Category { get; set; }
        public int PrepTime { get; set; }
        public int CookTime { get; set; }
        public int Marinade { get; set; }
        public int Makes { get; set; }
        public int Difficulty { get; set; }
        // FK
        public List<Ingredient> Ingredients { get; set; }
        // FK
        public List<Method> Methods { get; set; }
        // FK
        public List<Tip> Tips { get; set; }
        public string ImageName { get; set; }
    }
}
