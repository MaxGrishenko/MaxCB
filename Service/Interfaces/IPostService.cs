using Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Interfaces
{
    public interface IPostService
    {
        IEnumerable<Post> GetPosts(string typePar, string userId, string inpPar, int catPar, int difPar);
        
        Post GetPost(long id);

        void SubscribePost(long id, string userId);
        void UnsubscribePost(long id, string userId);
        bool SubscribeCheck(long id, string userId);
        void InsertPost(Post post, string userId);
        void UpdatePost(Post post);
        void DeletePost(long id);
        void DeleteUserPosts(string userId);

        // Work with comments
        IEnumerable<Comment> GetComments(long postId);
        Comment GetComment(long id);
        long MakeComment(string name, long postId, string userId);
        void DeleteComment(long commentId);
        void DeleteUserComments(string userId);
        bool CommentOwnerCheck(long commentId, string userId);
    }
}
