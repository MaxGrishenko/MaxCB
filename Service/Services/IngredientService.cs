using Data;
using Repo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Service
{
    public class IngredientService : IIngredientService
    {
        private IRepository<Ingredient> ingredientRepository;
        public IngredientService(IRepository<Ingredient> recipeRepository)
        {
            this.ingredientRepository = recipeRepository;
        }

        public IEnumerable<Ingredient> GetIngredients(long recipeId)
        {
            var ingredients = new List<Ingredient>();
            ingredientRepository.GetAll().ToList().ForEach(u =>
            {
                if (u.RecipeId == recipeId)
                {
                    ingredients.Add(u);
                }
            });
            return ingredients;
        }
        public void DeleteIngredients(long recipeId)
        {
            ingredientRepository.GetAll().ToList().ForEach(u =>
            {
                if (u.RecipeId == recipeId)
                {
                    ingredientRepository.Delete(u);
                }
            });
        }
        public void InsertIngredient(Ingredient ingredient)
        {
            ingredientRepository.Insert(ingredient);
        }
    }
}
