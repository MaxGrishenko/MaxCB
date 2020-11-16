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
using Microsoft.AspNetCore.Localization;
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
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IRecipeService _recipeService;
        private readonly IIngredientService _ingredientService;
        private readonly IMethodService _methodService;
        private readonly ITipService _tipService;
        private readonly IPostService _postService;

        public RecipeController(ApplicationContext applicationContext,
                                 IWebHostEnvironment webHostEnvironment,
                                 UserManager<ApplicationUser> userManager,
                                 RoleManager<IdentityRole> roleManager,
                                 IRecipeService recipeService,
                                 IIngredientService ingredientService,
                                 IMethodService methodService,
                                 ITipService tipService,
                                 IPostService postService)
        {
            _applicationContext = applicationContext;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _roleManager = roleManager;
            _recipeService = recipeService;
            _ingredientService = ingredientService;
            _methodService = methodService;
            _tipService = tipService;
            _postService = postService;
        }
        // CrUD
        [HttpGet]
        [Authorize]
        public IActionResult AddorEdit(long recipeId = 0)
        {
            if (recipeId == 0)
            {
                ViewData["returnAction"] = "/Recipe/AddorEdit";
                return View(new RecipeViewModel() 
                { 
                    IsNew = true, 
                    Category = 0, 
                    Difficulty=0, 
                    ImagePath = "/Image/emptyImage.png"
                }); 
            }
            else
            {
                ViewData["returnAction"] = "/Recipe/AddorEdit?recipeId=" + recipeId.ToString();
                var recipeEntity = _recipeService.GetRecipe(recipeId);
                return View(new RecipeViewModel()
                {
                    Id = recipeEntity.Id,
                    Title = recipeEntity.Title,
                    Description = recipeEntity.Description,
                    Category = recipeEntity.Category,
                    Difficulty = recipeEntity.Difficulty,
                    Ingredients = _ingredientService.GetIngredients(recipeId).ToList().Select(u => u.Name.ToString()).ToArray(),
                    Methods = _methodService.GetMethods(recipeId).Select(u => u.Name.ToString()).ToArray(),
                    Tips = _tipService.GetTips(recipeId).Select(u => u.Name.ToString()).ToArray(),
                    ImagePath = recipeEntity.ImagePath,
                });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddorEdit(RecipeViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.IsNew)
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
                }
                else
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
                }
                return RedirectToAction("Show");
            }
            if (model.IsNew)
            {
                ViewData["returnAction"] = "/Recipe/AddorEdit/";
            }
            else
            {

                ViewData["returnAction"] = "/Recipe/AddorEdit?recipeId=" + model.Id.ToString();
            }
            return View(model);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Delete(long postId)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string imagePath = _recipeService.GetRecipe(_postService.GetPost(postId).RecipeId).ImagePath;
            _postService.DeletePost(postId);
            if (imagePath != "/Image/test1.jpg" && imagePath != "/Image/emptyImage.jpg") { System.IO.File.Delete(wwwRootPath + imagePath);}
            return Ok();
        }
        // Partial
        [HttpPost]
        public async Task<IActionResult> PartialPost(string typePar, string inpPar, int catPar, int difPar)
        {
            var model = new List<PostViewModel>();
            IEnumerable<Post> posts;
            string userId;
            ViewData["parameter"] = typePar;
            if (User.Identity.IsAuthenticated)
            {
                userId = HttpContext.User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier).Value;
                var currentUser = await _userManager.FindByIdAsync(userId);
                var roles = await _userManager.GetRolesAsync(currentUser);
                ViewData["CurrentUserName"] = currentUser.UserName;
                ViewData["CurrentUserRole"] = roles[0];
                posts = _postService.GetPosts(typePar, userId, inpPar, catPar, difPar);
            }
            else
            {
                userId = null;
                posts = _postService.GetPosts(typePar, userId, inpPar, catPar, difPar);
                ViewData["CurrentUserName"] = "null";
                ViewData["CurrentUserRole"] = "null";
            }
            foreach (var item in posts)
            {
                var recipeEntity = _recipeService.GetRecipe(item.RecipeId);
                model.Add(new PostViewModel()
                {
                    ImagePath = recipeEntity.ImagePath,
                    Title = recipeEntity.Title,
                    Description = recipeEntity.Description,
                    RecipeId = recipeEntity.Id,
                    PostId = item.Id,
                    SubscribeFlag = _postService.SubscribeCheck(item.Id, userId),
                    CreatorUser = await _userManager.FindByIdAsync(recipeEntity.UserId)
                });
            }
            return PartialView("_ShowPosts", model);
        }
        [HttpPost]
        public IActionResult PartialRecipe(long postId)
        {
            var recipeEntity = _recipeService.GetRecipe(_postService.GetPost(postId).RecipeId);
            var model = new DetailPostViewModel()
            {
                PostId = postId,
                Title = recipeEntity.Title,
                Description = recipeEntity.Description,
                CategoryName = "TODO INT->STRING",
                PrepTime = recipeEntity.PrepTime,
                CookTime = recipeEntity.CookTime,
                Marinade = recipeEntity.Marinade,
                DifficultyName = "TODO INT->STRING",
                ImagePath = recipeEntity.ImagePath,
                Ingredients = _ingredientService.GetIngredients(recipeEntity.Id).ToList(),
                Methods = _methodService.GetMethods(recipeEntity.Id).ToList(),
                Tips = _tipService.GetTips(recipeEntity.Id).ToList(),
            };
            return PartialView("_ShowRecipe", model);
        }
        [HttpPost]
        public async Task<IActionResult> PartialComments(long postId)
        {
            var model = new List<CommentViewModel>();
            var comments = _postService.GetComments(postId);
            foreach(var item in comments)
            {
                model.Add(new CommentViewModel()
                {
                    CommentId = item.Id,
                    Name = item.Name,
                    User = await _userManager.FindByIdAsync(item.UserId)
                });
            }
            var userId = HttpContext.User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _userManager.FindByIdAsync(userId);
            ViewData["postId"] = postId;
            ViewData["email"] = user.Email;
           
            return PartialView("_ShowComments", model);
        }
        // Work with Comment
        [HttpPost]
        public IActionResult MakeComment(string name, long postId)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier).Value;
            return Ok(_postService.MakeComment(name,postId,userId));
        }
        [HttpPost]
        public IActionResult DeleteComment(long commentId)
        {
            _postService.DeleteComment(commentId);
            return Ok(commentId);
        }
        [HttpPost]
        public IActionResult ReportComment(long commentId, string targetId)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier).Value;
            if (!_postService.CheckReportCommentExist(userId, commentId))
            {
                var reportEntity = new Report()
                {
                    TargetId = targetId,
                    CommentId = commentId
                };
                _postService.MakeReport(reportEntity, userId);
                return Ok("Add");
            }
            return Ok("Exist");
        }
        // Work with Post
        [HttpPost]
        public async Task <IActionResult> ReportPost(long postId, string targetName)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier).Value;
            if (!_postService.CheckReportPostExist(userId, postId))
            {
                var target = await _userManager.FindByNameAsync(targetName);
                var reportEntity = new Report()
                {
                    TargetId = target.Id,
                    PostId = postId
                };
                _postService.MakeReport(reportEntity, userId);
                return Ok("Add");
            }
            return Ok("Exist");
        }



        // 
        [HttpGet]
        public IActionResult SetLanguage(string culture, string returnAction)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
            return Redirect(returnAction);
        }

        [Route("")]
        [Route("Recipe/Show")]
        [HttpGet]
        public IActionResult Show()
        {
            ViewData["returnAction"] = "/Recipe/Show";
            return View();
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

        [Authorize]
        public IActionResult AddConst()
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier).Value;
            Recipe recipeEntity = new Recipe()
            {
                Title = "Плов со свининой",
                Description = "Плов - это простая еда. И готовится плов легко и просто! Без всяких понтов и заморочек!",
                Category = 3,
                PrepTime = 20,
                CookTime = 80,
                Marinade = 0,
                Difficulty = 2,
                ImagePath = "/Image/test1.jpg",
                UserId = userId
            };
            _recipeService.InsertRecipe(recipeEntity);

            var ingredient1 = new Ingredient() { Name = "Морковь 2 штуки", Recipe = recipeEntity };
            var ingredient2 = new Ingredient() { Name = "Свинина 500 г", Recipe = recipeEntity };
            var ingredient3 = new Ingredient() { Name = "Репчатый лук 2 штуки", Recipe = recipeEntity };
            var ingredient4 = new Ingredient() { Name = "Рис 2 стакана", Recipe = recipeEntity };
            var ingredient5 = new Ingredient() { Name = "Растительное масло 50 мл", Recipe = recipeEntity };
            var ingredient6 = new Ingredient() { Name = "Соль по вкусу", Recipe = recipeEntity };
            var ingredient7 = new Ingredient() { Name = "Специи для плова", Recipe = recipeEntity };
            var ingredient8 = new Ingredient() { Name = "Чеснок 1 головка", Recipe = recipeEntity };
            var method1 = new Method() { Name = "Мясо вымыть, обсушить, разрезать на кусочки.", Recipe = recipeEntity };
            var method2 = new Method() { Name = "Очистить, помыть и нарезать тонкими полукольцами репчатый лук.", Recipe = recipeEntity };
            var method3 = new Method() { Name = "Морковь очистить, помыть и нарезать соломкой.", Recipe = recipeEntity };
            var method4 = new Method() { Name = "Сделать зирвак (это основа плова, а именно - мясо, морковь, лук и специи). Для этого разогреть казанок. Налить растительное масло. Хорошо его прожарить. Выложить подготовленный лук. Жарить до золотистого цвета около 5–7 минут, помешивая", Recipe = recipeEntity };
            var method5 = new Method() { Name = "Выложить подготовленное мясо. Все готовить до состояния, когда мясо покроется зажаренной корочкой, около 10 минут.", Recipe = recipeEntity };
            var method6 = new Method() { Name = "Затем добавить морковь. Обжарить все вместе, помешивая, 3–5 минут.", Recipe = recipeEntity };
            var method7 = new Method() { Name = "Вскипятить чайник. Обжаренные овощи и мясо залить кипятком. Посолить, поперчить, добавить специи для плова. Варить на среднем огне 20 минут.", Recipe = recipeEntity };
            var method8 = new Method() { Name = "Положить рис, осторожно разровнять его по поверхности. В серединку поместить головку чеснока. Долить воды так, чтобы она покрывала рис выше на 2 см. Варить плов на максимальном огне без крышки почти до полного испарения жидкости, около 10–15 минут.", Recipe = recipeEntity };
            var method9 = new Method() { Name = "Как только вода испарилась, сделать минимальный огонь. В плове сделать несколько отверстий ручкой ложки. Накрыть крышкой и оставить плов со свининой упариваться на 15 минут.", Recipe = recipeEntity };
            var tip1 = new Tip() { Name = "Вода должна покрывать мясо с овощами где-то сантиметра на два.", Recipe = recipeEntity };
            var tip2 = new Tip() { Name = "Рис не перемешивать - аккуратно разровнять.", Recipe = recipeEntity };

            _ingredientService.InsertIngredient(ingredient1);
            _ingredientService.InsertIngredient(ingredient2);
            _ingredientService.InsertIngredient(ingredient3);
            _ingredientService.InsertIngredient(ingredient4);
            _ingredientService.InsertIngredient(ingredient5);
            _ingredientService.InsertIngredient(ingredient6);
            _ingredientService.InsertIngredient(ingredient7);
            _ingredientService.InsertIngredient(ingredient8);
            _methodService.InsertMethod(method1);
            _methodService.InsertMethod(method2);
            _methodService.InsertMethod(method3);
            _methodService.InsertMethod(method4);
            _methodService.InsertMethod(method5);
            _methodService.InsertMethod(method6);
            _methodService.InsertMethod(method7);
            _methodService.InsertMethod(method8);
            _methodService.InsertMethod(method9);
            _tipService.InsertTip(tip1);
            _tipService.InsertTip(tip2);
            var postEntity = new Post()
            {
                RecipeId = recipeEntity.Id
            };
            _postService.InsertPost(postEntity, userId);

            return RedirectToAction("ShowMy");
        }
    }
}
