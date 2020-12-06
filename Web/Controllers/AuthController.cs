using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Interfaces;
using Web.Models;

namespace Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IPostService _postService;
        private readonly IReportService _reportService;
        private readonly IRecipeService _recipeService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(IWebHostEnvironment webHostEnvironment, 
                              IPostService postService,
                              IReportService reportService,
                              IRecipeService recipeService,
                              UserManager<ApplicationUser> userManager,
                              SignInManager<ApplicationUser> signInManager,
                              RoleManager<IdentityRole> roleManager)
        {
            this._webHostEnvironment = webHostEnvironment;
            this._postService = postService;
            this._reportService = reportService;
            this._recipeService = recipeService;
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
        }
       
        // AdminPanelWork
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminPanel()
        {
            ViewData["returnAction"] = "/Auth/AdminPanel";
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PartialUsers(string inputPar = null, string comboPar = "empty")
        {
            var model = new List<UserViewModel>();
            Dictionary<ApplicationUser, string> users = new Dictionary<ApplicationUser, string>();
            string[] roles = { "Moderator", "User", "Banned" };
            foreach(string role in roles)
            {
                var tempUsers = await _userManager.GetUsersInRoleAsync(role);
                foreach(var user in tempUsers)
                {
                    users.Add(user, role);
                }
            }
       
            foreach (var user in users)
            {
                if (comboPar == "empty" || comboPar.Contains(user.Value))
                {
                    var emailCheck = user.Key.Email.Substring(0, user.Key.Email.IndexOf('@')).ToLower();
                    var nameCheck = user.Key.UserName.ToLower();
                    if (inputPar == null || emailCheck.Contains(inputPar.ToLower()) || nameCheck.Contains(inputPar.ToLower()))
                    {
                        model.Add(new UserViewModel()
                        {
                            userId = user.Key.Id,
                            name = user.Key.UserName,
                            email = user.Key.Email,
                            role = user.Value
                        });
                    }
                }
            }

            return PartialView("~/Views/Auth/_ShowUsers.cshtml", model);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles[0] == "Admin") return Ok();
                foreach(var item in roles)
                {
                    await _userManager.RemoveFromRoleAsync(user, item);
                }
                var result = await _userManager.AddToRoleAsync(user, role);
            }
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // user comments (and reports on them)
                _postService.GetComments(userId).ToList().ForEach(c =>
                {
                    _reportService.DeleteReportsFromComment(c.Id);
                    _postService.DeleteComment(c.Id);
                });
                
                // user reports
                _reportService.GetReports(userId).ToList().ForEach(r =>
                {
                    _reportService.DeleteReport(r.Id);
                });

                // unsubscribe user from others
                _postService.UnsubscribeUser(userId);

                // delete posts
                _postService.GetPosts(userId).ToList().ForEach(u =>
                {
                    _postService.GetComments(u.Id).ToList().ForEach(c =>
                    {
                        _reportService.DeleteReportsFromComment(c.Id);
                    });
                    _reportService.DeleteReportsFromPost(u.Id);

                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string imagePath = _recipeService.GetRecipe(_postService.GetPost(u.Id).RecipeId).ImagePath;
                    _postService.DeletePost(u.Id);
                    if (imagePath != "/Image/test1.jpg" && imagePath != "/Image/emptyImage.png") { System.IO.File.Delete(wwwRootPath + imagePath); }
                });
                await _userManager.DeleteAsync(user);
            }
            return Ok();
        }

        // ManagerPanelWork
        [HttpGet]
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult ReportPanel()
        {
            var dict = new Dictionary<string, ReportViewModel>();
            _reportService.GetReports().ToList().ForEach(u =>
            {
                var key = u.TargetId;
                string type;
                long objectId;
                if (u.CommentId != -1)
                {
                    key = u.CommentId.ToString() + key;
                    type = "comment";
                    objectId = u.CommentId;
                }
                else
                {
                    key = u.PostId.ToString() + key;
                    type = "post";
                    objectId = u.PostId;
                }
                if (!dict.ContainsKey(key))
                {
                    dict.Add(key, new ReportViewModel()
                    {
                        Amount = 1,
                        ReportType = type,
                        ObjectId = objectId,
                        UserId = u.TargetId
                    });
                }
                else dict[key].Amount += 1;
            });
            var opa = dict.Values.ToList();
            ViewData["returnAction"] = "/Auth/ReportPanel";
            return View(opa);
        }
        [HttpPost]
        public IActionResult DeleteReports(long objectId, string type)
        {
            switch (type)
            {
                case "comment":
                    _reportService.DeleteReportsFromComment(objectId);
                    break;
                case "post":
                    _reportService.DeleteReportsFromPost(objectId);
                    break;
                default:
                    break;
            }
            return Ok();
        }
        [HttpPost]
        public IActionResult DeleteObject(long objectId, string type)
        {
            switch (type)
            {
                case "comment":
                    _postService.DeleteComment(objectId);
                    break;
                case "post":
                    _postService.DeletePost(objectId);
                    break;
                default:
                    break;
            }
            return Ok();
        }
        
        // Login/LogOut Work
        [HttpGet]
        public async Task<IActionResult> Registration(string returnAction)
        {
            ViewData["returnAction"] = "/Auth/Registration";
            RegisterViewModel model = new RegisterViewModel
            {
                ReturnUrl = returnAction,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Registration(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser()
                {
                    UserName = model.Name,
                    Email = model.Email,
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            ViewData["returnAction"] = "/Auth/Registration";
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Login(string returnAction)
        {
            ViewData["returnAction"] = "/Auth/Login";
            LoginViewModel model = new LoginViewModel
            {
                ReturnUrl = returnAction,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "Неправильный логин и (или) пароль");
                    }
                }
                else
                {
                    ModelState.AddModelError("Password", "Неправильный логин и (или) пароль");
                }
            }
            ViewData["returnAction"] = "/Auth/Login";
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return View(model);
        }
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Auth",
                                new { ReturnUrl = returnUrl });
            var properties = _signInManager
                .ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            LoginViewModel loginViewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View("Login", loginViewModel);
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "Error loading external login information.");
                return View("Login", loginViewModel);
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider,info.ProviderKey, isPersistent: true, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }

            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);

                if (email != null)
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                        };
                        await _userManager.CreateAsync(user);
                        await _userManager.AddToRoleAsync(user, "User");
                    }

                    await _userManager.AddLoginAsync(user, info);
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }
                ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
                ViewBag.ErrorMessage = "Please contact support on Pragim@PragimTech.com";

                return View("Error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // InitialAction
        public async Task<bool> CreateInitialRoles()
        {
            var results = new IdentityResult[]
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin")),
                await _roleManager.CreateAsync(new IdentityRole("Manager")),
                await _roleManager.CreateAsync(new IdentityRole("User")),
                await _roleManager.CreateAsync(new IdentityRole("Banned")),
            };
            if (results.All(x => x.Succeeded))
                return true;
            else
                return false;
        }
        public async Task<bool> UpdateRole(string userId = null, string role = "Admin")
        {
            userId = HttpContext.User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier).Value;
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.RemoveFromRoleAsync(user, "User");
                var result = await _userManager.AddToRoleAsync(user, role);
                if (result.Succeeded)
                    return true;
            }
            return false;
        }
    }
}
