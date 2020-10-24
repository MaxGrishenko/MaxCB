using System;

namespace Data
{
    public class Method : BaseEntity
    {
        public string Name { get; set; }

        public long RecipeId { get; set; }
        public Recipe Recipe { get; set; }
    }
}
