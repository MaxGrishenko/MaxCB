using Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Interfaces
{
    public interface IPostService
    {
        IEnumerable<Post> GetPosts(string typePar, string userId, string inpPar, int catPar, int difPar);
        IEnumerable<Comment> GetComments(long postId);
        Post GetPost(long id);
        long MakeComment(string name, long postId, string userId);
        void DeleteComment(long commentId);
        void SubscribePost(long id, string userId);
        void UnsubscribePost(long id, string userId);
        bool SubscribeCheck(long id, string userId);
        void InsertPost(Post post, string userId);
        void UpdatePost(Post post);
        void DeletePost(long id);

        void DeleteUserComments(string userId);
        void DeleteUserPosts(string userId);

        //Work with Reports
        IEnumerable<Report> GetReports();
        void MakeReport(Report report, string userId);
        bool CheckReportCommentExist(string userId, long commentId);
        bool CheckReportPostExist(string userId, long postId);
        void DeleteReportsFromComment(string targetId, long commentId);
        void DeleteReportsFromPost(string targetId, long postId);
    }
}
