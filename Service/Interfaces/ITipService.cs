using Data;
using System;
using System.Collections.Generic;

namespace Service
{
    public interface ITipService
    {
        IEnumerable<Tip> GetTips(long recipeId);
        void DeleteTips(long recipeId);
        void InsertTip(Tip tip);
    }
}
