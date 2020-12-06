using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Web.Hubs
{
    [Authorize]
    public class CommentsHub : Hub
    {
        private IPostService _postService;
        private IReportService _reportService;
        private UserManager<ApplicationUser> _userManager;
        public CommentsHub(IPostService postService, IReportService reportService, UserManager<ApplicationUser> userManager)
        {
            _postService = postService;
            _reportService = reportService;
            _userManager = userManager;
        }
        public async Task Enter(long postId)
        {
            string groupName = postId.ToString();
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
        public async Task Send(string text, long postId)
        {
            var user = await _userManager.GetUserAsync(Context.User);
            if (await _userManager.FindByIdAsync(user.Id) != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles[0] == "Admin" || roles[0] == "Manager" || roles[0] == "User")
                {
                    string groupName = postId.ToString();
                    long commentId = _postService.MakeComment(text, postId, user.Id);
                    await Clients.Group(groupName).SendAsync("Receive", text, user.Id, user.UserName, commentId);
                }
            }
        }
        public async Task Delete(long commentId, long postId, string targetId)
        {
            var user = await _userManager.GetUserAsync(Context.User);
            if (await _userManager.FindByIdAsync(user.Id) != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if ((roles[0] == "Admin" || roles[0] == "Manager") || (roles[0] == "User" && _postService.CommentOwnerCheck(commentId, user.Id)))
                {
                    string groupName = postId.ToString();
                    _reportService.DeleteReportsFromComment(commentId);
                    _postService.DeleteComment(commentId);
                    await Clients.Group(groupName).SendAsync("Remove", commentId);
                }
            }
        }
    }
}
