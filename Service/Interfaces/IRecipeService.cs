using Data;
using System;
using System.Collections.Generic;

namespace Service
{
    public interface IRecipeService
    {
        IEnumerable<Recipe> GetRecipes();
        Recipe GetRecipe(long id);
        void InsertRecipe(Recipe post);
        void UpdateRecipe(Recipe post);
        void DeleteRecipe(long id);
    }
}
