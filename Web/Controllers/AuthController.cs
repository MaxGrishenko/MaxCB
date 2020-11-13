using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualBasic.CompilerServices;
using Service.Interfaces;
using Web.Models;

namespace Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IPostService _postService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(IPostService postService,
                              UserManager<ApplicationUser> userManager,
                              SignInManager<ApplicationUser> signInManager,
                              RoleManager<IdentityRole> roleManager)
        {
            this._postService = postService;
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
        public async Task<IActionResult> PartialUsers(string inputPar = null, string comboPar = "empty")
        {
            var model = new List<UserViewModel>();
            foreach (var user in _userManager.Users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles[0].ToString();
                if (comboPar.Contains(role) || (comboPar == "empty" && role != "Admin")) 
                {
                    var emailCheck = user.Email.Substring(0, user.Email.IndexOf('@')).ToLower();
                    var nameCheck = user.UserName.ToLower();
                    if (inputPar == null || emailCheck.Contains(inputPar.ToLower()) || nameCheck.Contains(inputPar.ToLower()))
                    {
                        model.Add(new UserViewModel()
                        {
                            userId = user.Id,
                            name = user.UserName,
                            email = user.Email,
                            role = roles[0].ToString()
                        });
                    }
                }
            }
            return PartialView("_ShowUsers", model);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
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
                _postService.DeleteUserPosts(userId);
                _postService.DeleteUserPosts(userId);
                await _userManager.DeleteAsync(user);
            }
            return Ok();
        }
        // ManagerPanelWork
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult ReportPanel()
        {
            var dict = new Dictionary<string, ReportViewModel>();
            _postService.GetReports().ToList().ForEach(u =>
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
            return View(opa);
        }
        [HttpPost]
        public IActionResult DeleteReports(long objectId, string targetId, string type)
        {
            switch (type)
            {
                case "comment":
                    _postService.DeleteReportsFromComment(targetId, objectId);
                    break;
                case "post":
                    _postService.DeleteReportsFromPost(targetId, objectId);
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
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
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
