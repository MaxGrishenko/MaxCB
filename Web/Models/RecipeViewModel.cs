using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
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

        [Required (ErrorMessage = "title_required")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "title_length")]
        [RegularExpression("[a-zA-Zа-яА-Я ]+", ErrorMessage = "title_regular")]
        [Display(Name = "title_name", Prompt = "title_promt")]
        public string Title { get; set; }

        [Required(ErrorMessage = "description_required")]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "description_length")]
        [RegularExpression(@"^[a-zA-Zа-яА-Я.,?!:\-\(\) ]+$", ErrorMessage = "description_regular")]
        [Display(Name = "description_name", Prompt = "description_promt")]
        public string Description { get; set; }

        [Display(Name = "categories_name")]
        public int Category { get; set; } = 0;

        [Display(Name = "difficulties_name")]
        public int Difficulty { get; set; } = 0;

        [Display(Name = "prep_time_name")]
        public int PrepTime { get; set; } = 0;
        [Display(Name = "cook_time_name")]
        public int CookTime { get; set; } = 0;
        [Display(Name = "marinade_name")]
        public int Marinade { get; set; } = 0;

        [StringArrayRequired(ErrorMessage = "ingredients_required")]
        [StringArrayLength(3, 300, ErrorMessage = "ingredients_length")]   
        [StringArrayRegular(@"^[a-zA-Zа-яА-Я0-9.,?!:\-\(\) ]+$", ErrorMessage = "ingredients_regular")]
        [Display(Name = "ingredients_name", Prompt = "ingredients_promt")]
        public string[] Ingredients { get; set; }

        [StringArrayRequired(ErrorMessage = "methods_required")]
        [StringArrayLength(10, 300, ErrorMessage = "methods_length")]
        [StringArrayRegular(@"^[a-zA-Zа-яА-Я0-9.,?!:\-\(\) ]+$", ErrorMessage = "methods_regular")]
        [Display(Name = "methods_name", Prompt = "methods_promt")]
        public string[] Methods { get; set; }

        [StringArrayLength(3, 300, ErrorMessage = "tips_length")]
        [StringArrayRegular(@"^[a-zA-Zа-яА-Я0-9.,?!:\-\(\) ]+$", ErrorMessage = "tips_regular")]
        [Display(Name = "tips_name", Prompt = "tips_promt")]
        public string[] Tips { get; set; }

        public IFormFile RecipeImage { get; set; }

        [Display(Name = "image_path_name")]
        public string ImagePath { get; set; }
    }
}
