using Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Interfaces
{
    public interface IPostService
    {
        IEnumerable<Post> GetPosts();
        IEnumerable<Post> GetPosts(string userId);
        IEnumerable<PostUser> GetPostUsers();
        Post GetPost(long id);
        
        void SubscribePost(long id, string userId);
        void UnsubscribePost(long id, string userId);
        void UnsubscribeUser(string userId);
        bool SubscribeCheck(long id, string userId);

        void InsertPost(Post post, string userId);
        void DeletePost(long id);

        // Work with comments
        IEnumerable<Comment> GetComments(long postId);
        IEnumerable<Comment> GetComments(string userId);
        Comment GetComment(long id);
        long MakeComment(string name, long postId, string userId);
        void DeleteComment(long commentId);
        bool CommentOwnerCheck(long commentId, string userId);
    }
}
