using Data;
using Repo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Service
{
    public class RecipeService : IRecipeService
    {
        private IRepository<Recipe> recipeRepository;

        public RecipeService(IRepository<Recipe> recipeRepository)
        {
            this.recipeRepository = recipeRepository;
        }
        public IEnumerable<Recipe> GetRecipes()
        {
            return recipeRepository.GetAll();
        }
        public Recipe GetRecipe(long id)
        {
            return recipeRepository.Get(id);
        }
        public void InsertRecipe(Recipe recipe)
        {
            recipeRepository.Insert(recipe);
        }
        public void UpdateRecipe(Recipe recipe)
        {
            recipeRepository.Update(recipe);
        }
        public void DeleteRecipe(long id)
        {
            Recipe recipe = GetRecipe(id);
            recipeRepository.Remove(recipe);
            recipeRepository.SaveChanges();
        }
    }
}
