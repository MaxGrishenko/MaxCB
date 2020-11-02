using Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Interfaces
{
    public interface IPostService
    {
        IEnumerable<Post> GetPosts(string UserId);
        IEnumerable<Post> GetPosts();
        public IEnumerable<Comment> GetComments(long postId);
        Post GetPost(long id);
        public long MakeComment(string name, long postId, string userId);
        public void DeleteComment(long commentId);
        void SubscribePost(long id, string userId);
        void UnsubscribePost(long id, string userId);
        bool SubscribeCheck(long id, string userId);
        void InsertPost(Post post, string userId);
        void UpdatePost(Post post);
        void DeletePost(long id);
    }
}
