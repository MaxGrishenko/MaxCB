using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class RecipeViewModel
    {
        [HiddenInput]
        public long Id { get; set; }

        [Display(Name = "Название рецепта: ", Prompt = "Введите название вашего рецепта")]
        public string Title { get; set; }
        [Display(Name = "Краткое описание: ", Prompt = "Краткое описание вашего рецепта")]
        public string Description { get; set; }

        [Display(Name = "Выберите категорию:")]
        public int Category { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }


        [Display(Name = "Подготовки:", Prompt = "В минутах")]
        public int? PrepTime { get; set; } = null;
        [Display(Name = "Приготовления:", Prompt = "В минутах")]
        public int? CookTime { get; set; } = null;
        [Display(Name = "Мариновки:", Prompt = "В минутах")]
        public int? Marinade { get; set; } = null;

        [Display(Name = "Сложность приготовления:")]
        public int Difficulty { get; set; }
        public IEnumerable<SelectListItem> Difficulties { get; set; }

        [Display(Name = "Используемые ингредиенты:")]
        public string[] Ingredients { get; set; }
        [Display(Name = "Шаги приготовления:")]
        public string[] Methods { get; set; }
        [Display(Name = "Подсказки и советы:")]
        public string[] Tips { get; set; }

        [Display(Name = "Фото готового блюда:")]
        public IFormFile RecipeImage { get; set; }
        public string ImageName { get; set; }
    }
}
