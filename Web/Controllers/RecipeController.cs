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
        private readonly IReportService _reportService;

        public RecipeController(ApplicationContext applicationContext,
                                 IWebHostEnvironment webHostEnvironment,
                                 UserManager<ApplicationUser> userManager,
                                 RoleManager<IdentityRole> roleManager,
                                 IRecipeService recipeService,
                                 IIngredientService ingredientService,
                                 IMethodService methodService,
                                 ITipService tipService,
                                 IPostService postService,
                                 IReportService reportService)
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
            _reportService = reportService;
        }

        // Create / Update / Delete Recipe
        [HttpGet]
        [Authorize(Roles = "Admin, Manager, User")]
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
                    ImagePath = recipeEntity.ImagePath
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
                        UserId = userId
                    };
                    if (model.RecipeImage == null)
                    {
                        recipeEntity.ImagePath = model.ImagePath;
                    }
                    else
                    {
                        recipeEntity.ImagePath = GetImagePath(model.RecipeImage);
                    }

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
            if (imagePath != "/Image/test1.jpg" && imagePath != "/Image/emptyImage.png") { System.IO.File.Delete(wwwRootPath + imagePath);}
            return Ok();
        }

        // Partial work
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
            return PartialView("~/Views/Recipe/_ShowPosts.cshtml", model);
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
                Category = recipeEntity.Category,
                PrepTime = recipeEntity.PrepTime,
                CookTime = recipeEntity.CookTime,
                Marinade = recipeEntity.Marinade,
                Difficulty = recipeEntity.Difficulty,
                ImagePath = recipeEntity.ImagePath,
                Ingredients = _ingredientService.GetIngredients(recipeEntity.Id).ToList(),
                Methods = _methodService.GetMethods(recipeEntity.Id).ToList(),
                Tips = _tipService.GetTips(recipeEntity.Id).ToList(),
            };
            if (User.Identity.IsAuthenticated) ViewData["userIsAuth"] = "true";
            else ViewData["userIsAuth"] = "false";
            return PartialView("~/Views/Recipe/_ShowRecipe.cshtml", model);
        }
        [HttpPost]
        public async Task<IActionResult> PartialComments(long postId)
        {
            var model = new List<CommentViewModel>();
            var comments = _postService.GetComments(postId);
            foreach(var item in comments)
            {
                var commentUser = await _userManager.FindByIdAsync(item.UserId);
                model.Add(new CommentViewModel()
                {
                    CommentId = item.Id,
                    Text = item.Name,
                    UserName = commentUser.UserName,
                    UserId = commentUser.Id
                });
            }
            var userId = HttpContext.User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles[0].ToString();

            ViewData["postId"] = postId;
            ViewData["userId"] = user.Id;
            ViewData["userName"] = user.UserName;
            ViewData["userRole"] = role;

            return PartialView("~/Views/Recipe/_ShowComments.cshtml", model);
        }
        [HttpPost]
        public async Task<IActionResult> PartialObject(long objectId, string objectType)
        {
            ObjectViewModel model;
            if (objectType == "comment")
            {
                var commentEntity = _postService.GetComment(objectId);
                var user = await _userManager.FindByIdAsync(commentEntity.UserId);
                model = new ObjectViewModel()
                {
                    Title = user.UserName,
                    Description = commentEntity.Name
                };
            }
            else
            {
                var postEntity = _postService.GetPost(objectId);
                var recipeEntity = _recipeService.GetRecipe(postEntity.RecipeId);
                model = new ObjectViewModel()
                {
                    Title = recipeEntity.Title,
                    Description = recipeEntity.Description,
                    ImagePath = recipeEntity.ImagePath,
                    Ingredients = _ingredientService.GetIngredients(recipeEntity.Id).ToList(),
                    Methods = _methodService.GetMethods(recipeEntity.Id).ToList(),
                    Tips = _tipService.GetTips(recipeEntity.Id).ToList()
                };
            }
            return PartialView("~/Views/Recipe/_ShowObject.cshtml", model);

        }

        // Work with reports
        [Authorize(Roles = "User")]
        [HttpPost]
        public IActionResult ReportComment(long commentId, string userId, string targetId)
        {
            return Ok(_reportService.ReportComment(commentId, userId, targetId));
        }
        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> ReportPost(long postId, string targetName)
        {
            var target = await _userManager.FindByNameAsync(targetName);
            var userId = HttpContext.User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier).Value;
            return Ok(_reportService.ReportPost(postId, userId, target.Id));
        }


        // Change Language and subscribtion work
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
        [HttpPost]
        [Authorize]
        public IActionResult Subscribe(long postId, string subFlag)
        {
            string userId = HttpContext.User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier).Value;
            if (subFlag == "sub") { _postService.SubscribePost(postId, userId); }
            else _postService.UnsubscribePost(postId, userId);
            return Ok();
        }

        // Insert Tips, Methods, Ingredients, Image into db
        private string GetImagePath(IFormFile recipeImage, string prevImagePath = null)
        {
            if (recipeImage != null)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (prevImagePath != null && prevImagePath != "emptyImage.png")
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

        [Route("")]
        [Route("Recipe/Show")]
        [HttpGet]
        public IActionResult Show()
        {
            ViewData["returnAction"] = "/Recipe/Show";
            return View();
        }
    }
}
