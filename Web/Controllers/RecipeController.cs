using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repo;
using Service;
using Service.Interfaces;
using Web.Models;

namespace Web.Controllers
{
    public class RecipeController : Controller
    {
        private readonly ApplicationContext _applicationContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRecipeService _recipeService;
        private readonly IIngredientService _ingredientService;
        private readonly IMethodService _methodService;
        private readonly ITipService _tipService;
        private readonly IPostService _postService;

        public RecipeController(ApplicationContext applicationContext,
                                 IWebHostEnvironment webHostEnvironment,
                                 UserManager<ApplicationUser> userManager,
                                 IRecipeService recipeService,
                                 IIngredientService ingredientService,
                                 IMethodService methodService,
                                 ITipService tipService,
                                 IPostService postService)
        {
            _applicationContext = applicationContext;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _recipeService = recipeService;
            _ingredientService = ingredientService;
            _methodService = methodService;
            _tipService = tipService;
            _postService = postService;
        }






        [HttpGet]
        [Authorize]
        public IActionResult Add()
        {
            RecipeViewModel model = new RecipeViewModel();
            model.Categories = GetCategories();
            model.Difficulties = GetDifficulties();
            return View(model);
        }
        [HttpPost]
        public IActionResult Add(RecipeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier).Value;
                var recipeEntity = new Recipe()
                {
                    Title = model.Title,
                    Description = model.Description,
                    Category = model.Category,
                    PrepTime = (int)model.PrepTime,
                    CookTime = (int)model.CookTime,
                    Marinade = (int)model.Marinade,
                    Difficulty = model.Difficulty,
                    ImagePath = GetImagePath(model.RecipeImage),
                    UserId = userId
                };
                _recipeService.InsertRecipe(recipeEntity);
                GetIngredients(model.Ingredients, recipeEntity);
                GetMethods(model.Methods, recipeEntity);
                GetTips(model.Tips, recipeEntity);

                var postEntity = new Post()
                {
                    RecipeId = recipeEntity.Id
                };
                _postService.InsertPost(postEntity, userId);

                return RedirectToAction("Index", "Home");
            }
            else return View(model);
        }
        [HttpGet]
        [Authorize]
        public IActionResult Edit(long id)
        {
            Recipe recipeEntity = _recipeService.GetRecipe(id);
            RecipeViewModel model = new RecipeViewModel()
            {
                Id = recipeEntity.Id,
                Title = recipeEntity.Title,
                Description = recipeEntity.Description,
                Categories = GetCategories(),
                Category = recipeEntity.Category,
                PrepTime = recipeEntity.PrepTime,
                CookTime = recipeEntity.CookTime,
                Marinade = recipeEntity.Marinade,
                Difficulties = GetDifficulties(),
                Difficulty = recipeEntity.Difficulty,
                Ingredients = _ingredientService.GetIngredients(id).ToList().Select(u => u.Name.ToString()).ToArray(),
                Methods = _methodService.GetMethods(id).Select(u => u.Name.ToString()).ToArray(),
                Tips = _tipService.GetTips(id).Select(u => u.Name.ToString()).ToArray(),
                ImageName = recipeEntity.ImagePath.Substring(7)
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult Edit(RecipeViewModel model)
        {
            Recipe recipeEntity = _recipeService.GetRecipe(model.Id);

            _ingredientService.DeleteIngredients(recipeEntity.Id);
            _methodService.DeleteMethods(recipeEntity.Id);
            _tipService.DeleteTips(recipeEntity.Id);
            GetIngredients(model.Ingredients, recipeEntity);
            GetMethods(model.Methods, recipeEntity);
            GetTips(model.Tips, recipeEntity);

            recipeEntity.Title = model.Title;
            recipeEntity.Description = model.Description;
            recipeEntity.Category = model.Category;
            recipeEntity.PrepTime = (int)model.PrepTime;
            recipeEntity.CookTime = (int)model.CookTime;
            recipeEntity.Marinade = (int)model.Marinade;
            recipeEntity.Difficulty = model.Difficulty;
            if (model.RecipeImage != null)
            {
                recipeEntity.ImagePath = GetImagePath(model.RecipeImage, recipeEntity.ImagePath.Substring(7));
            }
            else recipeEntity.ImagePath = GetImagePath(model.RecipeImage);

            _recipeService.UpdateRecipe(recipeEntity);
            return RedirectToAction("ShowAll");
        }
        [HttpPost]
        [Authorize]
        public IActionResult Delete(long postId)
        {
            //_postService.DeletePost(postId);
            //Todo удалить изображение по Post->Recipe->ImagePath
            return Ok();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ShowMy()
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier).Value;
            var posts = _postService.GetPosts(userId).ToList();
            var models = new List<PostViewModel>();
            foreach (var item in posts)
            {
                var recipeEntity = _recipeService.GetRecipe(item.RecipeId);
                models.Add(new PostViewModel()
                {
                    ImagePath = recipeEntity.ImagePath,
                    Title = recipeEntity.Title,
                    Description = recipeEntity.Description,
                    RecipeId = recipeEntity.Id,
                    PostId = item.Id,
                    SubscribeFlag = _postService.SubscribeCheck(item.Id, userId),
                    CreatorUser = await _userManager.FindByIdAsync(recipeEntity.UserId),
                    CurrentUser = await _userManager.FindByIdAsync(userId)
                });
            }
            return View(models);
        }
        public async Task<IActionResult> ShowAll()
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier).Value;
            var models = new List<PostViewModel>();
            var posts = _postService.GetPosts().ToList();
            foreach(var item in posts)
            {
                var recipeEntity = _recipeService.GetRecipe(item.RecipeId);
                models.Add(new PostViewModel()
                {
                    ImagePath = recipeEntity.ImagePath,
                    Title = recipeEntity.Title,
                    Description = recipeEntity.Description,
                    RecipeId = recipeEntity.Id,
                    PostId = item.Id,
                    SubscribeFlag = _postService.SubscribeCheck(item.Id, userId),
                    CreatorUser = await _userManager.FindByIdAsync(recipeEntity.UserId),
                    CurrentUser = await _userManager.FindByIdAsync(userId)
                });
            }
            return View(models);
        }
        [Authorize]
        public IActionResult AddConst()
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier).Value;
            Recipe recipeEntity = new Recipe()
            {
                Title = "Тестовый рецепт",
                Description = "Стандратное описание",
                Category = 2,
                PrepTime = 10,
                CookTime = 30,
                Marinade = 60,
                Difficulty = 2,
                ImagePath = "/Image/blini.jpg",
                UserId = userId
            };
            _recipeService.InsertRecipe(recipeEntity);
            Ingredient ingredient1 = new Ingredient()
            {
                Name = "1-ый ингредиент",
                Recipe = recipeEntity
            };
            Ingredient ingredient2 = new Ingredient()
            {
                Name = "2-ый ингредиент",
                Recipe = recipeEntity
            };
            Ingredient ingredient3 = new Ingredient()
            {
                Name = "3-ый ингредиент",
                Recipe = recipeEntity
            };
            Ingredient ingredient4 = new Ingredient()
            {
                Name = "4-ый ингредиент",
                Recipe = recipeEntity
            };
            Method method1 = new Method()
            {
                Name = "1-ый шаг",
                Recipe = recipeEntity
            };
            Method method2 = new Method()
            {
                Name = "2-ый шаг",
                Recipe = recipeEntity
            };
            Method method3 = new Method()
            {
                Name = "3-ый шаг",
                Recipe = recipeEntity
            };
            Tip tip1 = new Tip()
            {
                Name = "1-ая подсказка",
                Recipe = recipeEntity
            };
            _ingredientService.InsertIngredient(ingredient1);
            _ingredientService.InsertIngredient(ingredient2);
            _ingredientService.InsertIngredient(ingredient3);
            _ingredientService.InsertIngredient(ingredient4);
            _methodService.InsertMethod(method1);
            _methodService.InsertMethod(method2);
            _methodService.InsertMethod(method3);
            _tipService.InsertTip(tip1);

            var postEntity = new Post()
            {
                RecipeId = recipeEntity.Id
            };
            _postService.InsertPost(postEntity, userId);

            return RedirectToAction("ShowMy");
        }
        [HttpPost]
        [Authorize]
        public IActionResult Subscribe(long postId, string subFlag)
        {
            string userId = HttpContext.User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier).Value;
            if (subFlag == "sub") { _postService.SubscribePost(postId, userId); }
            else _postService.UnsubscribePost(postId, userId);
            return Ok();
        }
        [HttpPost]
        public IActionResult GetPartial(long postId)
        {
            Recipe recipeEntity = _recipeService.GetRecipe(_postService.GetPost(postId).RecipeId);
            return PartialView("_ShowRecipe", recipeEntity);
        }




        private string GetImagePath(IFormFile recipeImage, string prevImagePath = null)
        {
            if (recipeImage != null)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (prevImagePath != null)
                {
                    System.IO.File.Delete(Path.Combine(wwwRootPath + "/Image/", prevImagePath));
                }
                
                string fileName = Path.GetFileNameWithoutExtension(recipeImage.FileName);
                string extension = Path.GetExtension(recipeImage.FileName);
                fileName = fileName + DateTime.Now.ToString("eemmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/Image/", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    recipeImage.CopyTo(fileStream);
                }
                return Path.Combine("/Image/", fileName);
            }
            else return null;
        }
        private void GetTips(string[] tips, Recipe recipe)
        {
            foreach (string tip in tips)
            {
                Tip tipEntity = new Tip()
                {
                    Name = tip,
                    Recipe = recipe
                };
                _tipService.InsertTip(tipEntity);
            }
        }
        private void GetMethods(string[] methods, Recipe recipe)
        {
            foreach (string method in methods)
            {
                Method methodEntity = new Method()
                {
                    Name = method,
                    Recipe = recipe
                };
                _methodService.InsertMethod(methodEntity);
            }
        }
        private void GetIngredients(string[] ingredients, Recipe recipe)
        {
            foreach (string ingredient in ingredients)
            {
                Ingredient ingredientEntity = new Ingredient()
                {
                    Name = ingredient,
                    Recipe = recipe
                };
                _ingredientService.InsertIngredient(ingredientEntity);
            }
        }
        private IEnumerable<SelectListItem> GetCategories()
        {
            return new SelectListItem[]
            {
                new SelectListItem() {Text = "Не указана", Value = "0"},
                new SelectListItem() {Text = "Завтрак", Value = "1"},
                new SelectListItem() {Text = "Обед", Value = "2"},
                new SelectListItem() {Text = "Ужин", Value = "3"},
                new SelectListItem() {Text = "Другое", Value = "4"},
            };
        }
        private IEnumerable<SelectListItem> GetDifficulties()
        {
            return new SelectListItem[]
            {
                new SelectListItem() {Text = "Не указана", Value = "0"},
                new SelectListItem() {Text = "Просто", Value = "1"},
                new SelectListItem() {Text = "Нормально", Value = "2"},
                new SelectListItem() {Text = "Тяжело", Value = "3"},
                new SelectListItem() {Text = "Гордон Рамзи", Value = "4"},
            };
        }
    }
}
