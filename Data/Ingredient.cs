using System;

namespace Data
{
    public class Ingredient : BaseEntity
    {
        public string Name { get; set; }

        public Recipe Recipe { get; set; }
        public long RecipeId { get; set; }       
    }
}
