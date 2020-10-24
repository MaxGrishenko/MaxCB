using Data;
using System;
using System.Collections.Generic;

namespace Service
{
    public interface IIngredientService
    {
        IEnumerable<Ingredient> GetIngredients(long recipeId);
        void DeleteIngredients(long recipeId);
        void InsertIngredient(Ingredient ingredient);
    }
}
