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
        private IReportService _reportService;
        public CommentsHub(IPostService postService, IReportService reportService)
        {
            _postService = postService;
            _reportService = reportService;
        }
        public async Task SendComment(string text, string userId, string userName, long postId)
        {
            long commentId = _postService.MakeComment(text, postId, userId);
            await Clients.All.SendAsync("ReceiveComment", text, userId, userName, postId, commentId);
        }
        public async Task DeleteComment(long commentId, long postId, string targetId)
        {
            _reportService.DeleteReportsFromComment(commentId, targetId);
            _postService.DeleteComment(commentId);
            await Clients.All.SendAsync("RemoveComment", commentId, postId);
        }
    }
}
