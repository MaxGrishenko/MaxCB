using Data;
using System;
using System.Collections.Generic;

namespace Service
{
    public interface IMethodService
    {
        IEnumerable<Method> GetMethods(long id);
        void DeleteMethods(long recipeId);
        void InsertMethod(Method method);
    }
}
