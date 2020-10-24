using System;

namespace Data
{
    public class Tip : BaseEntity
    {
        public string Name { get; set; }

        public long RecipeId { get; set; }
        public Recipe Recipe { get; set; }
    }
}
