﻿using System;
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
                foreach (var error in result.Errors)
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
        public async Task<IActionResult> Login(LoginViewModel model)
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
