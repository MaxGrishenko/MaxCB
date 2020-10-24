using Data;
using Repo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Service
{
    public class MethodService : IMethodService
    {
        private IRepository<Method> methodRepository;
        public MethodService(IRepository<Method> methodRepository)
        {
            this.methodRepository = methodRepository;
        }

        public IEnumerable<Method> GetMethods(long recipeId)
        {
            var methods = new List<Method>();
            methodRepository.GetAll().ToList().ForEach(u =>
            {
                if (u.RecipeId == recipeId)
                {
                    methods.Add(u);
                }
            });
            return methods;
        }
        public void DeleteMethods(long recipeId)
        {
            methodRepository.GetAll().ToList().ForEach(u =>
            {
                if (u.RecipeId == recipeId)
                {
                    methodRepository.Delete(u);
                }
            });
        }
        public void InsertMethod(Method method)
        {
            methodRepository.Insert(method);
        }
    }
}
