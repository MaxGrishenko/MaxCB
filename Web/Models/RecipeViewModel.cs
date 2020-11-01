using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Web.Attributes;

namespace Web.Models
{
    public class RecipeViewModel
    {
        [HiddenInput]
        public long Id { get; set; }

        [HiddenInput]
        public bool IsNew { get; set; } = false;

        [Required (ErrorMessage = "Не указано название рецепта")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Длина названия должна быть от 3 до 30 символов")]
        [RegularExpression("[a-zA-Zа-яА-Я ]+", ErrorMessage = "Некорректный ввод *Только латинские и кириллические буквы*")]
        [Display(Name = "Название рецепта: ", Prompt = "Введите название вашего рецепта")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Не указано краткое описание рецепта")]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "Длина описания должна быть от 10 до 100 символов")]
        [RegularExpression(@"^[a-zA-Zа-яА-Я.,?!:\-\(\) ]+$", ErrorMessage = "Некорректный ввод *Только латинские, кириллические буквы и символы (.,? !:-) *")]
        [Display(Name = "Краткое описание: ", Prompt = "Краткое описание вашего рецепта")]
        public string Description { get; set; }

        [Display(Name = "Категория блюда:")]
        public int Category { get; set; } = 0;
        public IEnumerable<SelectListItem> Categories { get; set; }

        [Display(Name = "Сложность приготовления:")]
        public int Difficulty { get; set; } = 0;
        public IEnumerable<SelectListItem> Difficulties { get; set; }

        [Display(Name = "Подготовки:")]
        public int PrepTime { get; set; } = 0;
        [Display(Name = "Приготовления:")]
        public int CookTime { get; set; } = 0;
        [Display(Name = "Мариновки:")]
        public int Marinade { get; set; } = 0;

        [StringArrayRequired(ErrorMessage = "Один или несколько ингредиентов не указаны!")]
        [StringArrayLength(3, 300, ErrorMessage = "Описание ингредиента должна быть от 3 до 300 символов")]   
        [StringArrayRegular(@"^[a-zA-Zа-яА-Я0-9.,?!:\-\(\) ]+$", ErrorMessage = "Некорректный ввод * Только латинские, кириллические буквы, цифры и символы (.,? !:-) *")]
        [Display(Name = "Используемые ингредиенты:", Prompt = "Введите первый ингредиент!")]
        public string[] Ingredients { get; set; }

        [StringArrayRequired(ErrorMessage = "Один или несколько шагов не указано!")]
        [StringArrayLength(10, 300, ErrorMessage = "Описание шага должно быть от 10 до 300 символов")]
        [StringArrayRegular(@"^[a-zA-Zа-яА-Я0-9.,?!:\-\(\) ]+$", ErrorMessage = "Некорректный ввод * Только латинские, кириллические буквы, цифры и символы (.,? !:-) *")]
        [Display(Name = "Шаги приготовления:", Prompt = "Введите первый шаг притовления!")]
        public string[] Methods { get; set; }

        [StringArrayLength(3, 300, ErrorMessage = "Описание подсказки должно быть от 3 до 300 символов")]
        [StringArrayRegular(@"^[a-zA-Zа-яА-Я0-9.,?!:\-\(\) ]+$", ErrorMessage = "Некорректный ввод * Только латинские, кириллические буквы, цифры и символы (.,? !:-) *")]
        [Display(Name = "Подсказки и советы:", Prompt = "Поделитесь какой-нибудь хитростью в приготовлении!")]
        public string[] Tips { get; set; }

        public IFormFile RecipeImage { get; set; }

        [Display(Name = "Загрузите фото вашего блюда!")]
        public string ImagePath { get; set; }
    }
}
