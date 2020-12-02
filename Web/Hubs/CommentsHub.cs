using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Hubs
{
    [Authorize]
    public class CommentsHub : Hub
    {
        private IPostService _postService;
        public CommentsHub(IPostService postService)
        {
            _postService = postService;
        }
        public async Task SendComment(string text, string userId, string userName, long postId)
        {
            long commentId = _postService.MakeComment(text, postId, userId);
            await Clients.All.SendAsync("ReceiveComment", text, userName, postId, commentId);
        }
        public async Task DeleteComment(long commentId, long postId)
        {
            _postService.DeleteComment(commentId);
            await Clients.All.SendAsync("RemoveComment", commentId, postId);
        }

    }
}
