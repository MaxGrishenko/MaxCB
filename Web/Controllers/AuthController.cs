using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly  UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(UserManager<ApplicationUser> userManager,
                              SignInManager<ApplicationUser> signInManager,
                              RoleManager<IdentityRole> roleManager)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
        }

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

        // Admin
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminPanel()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> PartialUsers(String inputPar = null, string comboPar = null)
        {
            var model = new List<UserViewModel>();
            foreach (var user in _userManager.Users)
            {
                bool findFlag = false;
                if (comboPar != null)
                {
                    //wrong => continue
                }
                if (inputPar != null)
                {
                    if (user.Email.Substring(0, user.Email.IndexOf('@')).ToLower().Contains(inputPar.ToLower()))
                        findFlag = true;
                    /*
                    if (user.UserName.ToLower().Contains(inputPar.ToLower()))
                        findFlag = true;
                    */
                }
                //if (!findFlag) continue;

                var roles = await _userManager.GetRolesAsync(user);
                model.Add(new UserViewModel()
                {
                    userId = user.Id,
                    name = user.UserName,
                    email = user.Email,
                    role = roles[0].ToString()
                }); 
            }

            return PartialView("_ShowUsers", model);
        }
        
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



        [Authorize(Roles = "Admin, Manager")]
        public IActionResult ReportPanel()
        {
            return View();
        }



            [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Registration(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser()
                {
                    UserName = model.Email,
                    Email = model.Email,
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }
        [HttpPost]
        public async Task<IActionResult> Login (LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    // проверяем, принадлежит ли URL приложению
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
