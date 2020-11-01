using Data;
using Repo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Service
{
    public class TipService : ITipService
    {
        private IRepository<Tip> tipRepository;
        public TipService(IRepository<Tip> tipRepository)
        {
            this.tipRepository = tipRepository;
        }

        public IEnumerable<Tip> GetTips(long recipeId)
        {
            var tips = new List<Tip>();
            tipRepository.GetAll().ToList().ForEach(u =>
            {
                if (u.RecipeId == recipeId)
                {
                    tips.Add(u);
                }
            });
            if (tips.Count == 1)
            {
                tips[0].Name = string.Empty;
            }
            return tips;
        }
        public void DeleteTips(long recipeId)
        {
            tipRepository.GetAll().ToList().ForEach(u =>
            {
                if (u.RecipeId == recipeId)
                {
                    tipRepository.Delete(u);
                }
            });
        }
        public void InsertTip(Tip tip)
        {
            tipRepository.Insert(tip);
        }
    }
}
