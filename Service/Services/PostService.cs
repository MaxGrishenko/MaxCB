using Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Repo;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class PostService : IPostService
    {
        private IRepository<Post> postRepository;
        private IRepository<PostUser> postUserRepository;

        private IRepository<Recipe> recipeRepository;
        private IRepository<Comment> commentRepository;
        
        private IRepository<Report> reportRepository;
        private IRepository<ReportUser> reportUserRepository;

        public PostService(IRepository<Post> postRepository, 
                           IRepository<PostUser> postUserRepository,
                           IRepository<Recipe> recipeRepository, 
                           IRepository<Comment> commentRepository,
                           IRepository<Report> reportRepository,
                           IRepository<ReportUser> reportUserRepository)
        {
            this.postRepository = postRepository;
            this.postUserRepository = postUserRepository;
            this.recipeRepository = recipeRepository;
            this.commentRepository = commentRepository;
            this.reportRepository = reportRepository;
            this.reportUserRepository = reportUserRepository;
        }

        public IEnumerable<Post> GetPosts(string typePar, string userId, string inpPar, int catPar, int difPar)
        {
            var posts = new List<Post>();
            if (typePar == "all")
            {
                var postsEntity = postRepository.GetAll();
                foreach (var postEntity in postsEntity)
                {
                    var recipeEntity = recipeRepository.Get(postEntity.Id);
                    if (catPar != 0 && catPar != recipeEntity.Category)
                    {
                        continue;
                    }
                    if (difPar != 0 && difPar != recipeEntity.Difficulty)
                    {
                        continue;
                    }
                    if (inpPar != null && !recipeEntity.Title.ToLower().Contains(inpPar.ToLower()))
                    {
                        continue;
                    }
                    posts.Add(postEntity);
                }
            }
            else
            {
                var postUsers = postUserRepository.GetAll();
                foreach (var postUser in postUsers)
                {
                    if (userId == postUser.UserId || userId == null)
                    {
                        var postEntity = GetPost(postUser.PostId);
                        var recipeEntity = recipeRepository.Get(postEntity.Id);
                        if (catPar != 0 && catPar != recipeEntity.Category)
                        {
                            continue;
                        }
                        if (difPar != 0 && difPar != recipeEntity.Difficulty)
                        {
                            continue;
                        }
                        if (inpPar != null && !recipeEntity.Title.ToLower().Contains(inpPar.ToLower()))
                        {
                            continue;
                        }
                        posts.Add(postEntity);
                    }
                }
            }
            return posts;
        }
        
        public IEnumerable<Comment> GetComments(long postId)
        {
            var comments = new List<Comment>();
            commentRepository.GetAll().ToList().ForEach(u =>
            {
                if (u.PostId == postId)
                {
                    comments.Add(commentRepository.Get(u.Id));
                }
            });
            return comments;
        }
        public long MakeComment(string name, long postId, string userId)
        {
            var commentEntity = new Comment()
            {
                Name = name,
                PostId = postId,
                UserId = userId
            };
            commentRepository.Insert(commentEntity);
            return commentEntity.Id;
        }
        public void DeleteComment(long commentId)
        {
            commentRepository.Remove(commentRepository.Get(commentId));
            commentRepository.SaveChanges();
        }

        
        public Post GetPost(long id)
        {
            return postRepository.Get(id);
        }
        public void SubscribePost(long id, string userId)
        {
            postUserRepository.Insert(new PostUser()
            {
                PostId = id,
                UserId = userId
            });
        }
        public void UnsubscribePost(long id, string userId)
        {
            postUserRepository.GetAll().ToList().ForEach(u =>
            {
                if (u.PostId == id && u.UserId == userId)
                {
                    postUserRepository.Remove(u);
                    postUserRepository.SaveChanges();
                }
            });
        }
        public bool SubscribeCheck(long id, string userId)
        {
            var postUsers = postUserRepository.GetAll().ToList();
            foreach(var item in postUsers)
            {
                if (item.PostId == id && item.UserId == userId)
                {
                    return true;

                }
            }
            return false;
        }
        public void InsertPost(Post post, string userId)
        {
            postRepository.Insert(post);
            var postUserEntity = new PostUser()
            {
                PostId = post.Id,
                UserId = userId
            };
            postUserRepository.Insert(postUserEntity);
        }
        public void UpdatePost(Post post)
        {
            postRepository.Update(post);
        }
        public void DeletePost(long id)
        {
            Post postEntity = GetPost(id);
            postUserRepository.GetAll().ToList().ForEach(u =>
            {
                if (u.PostId == id)
                {
                    postUserRepository.Remove(u);
                }
            });
            recipeRepository.Remove(recipeRepository.Get(postEntity.RecipeId));
            postRepository.Remove(postEntity);

            recipeRepository.SaveChanges();
            postRepository.SaveChanges();
            postUserRepository.SaveChanges();
        }

        public void DeleteUserComments(string userId)
        {
            commentRepository.GetAll().ToList().ForEach(u => {
                if (u.UserId == userId)
                {
                    commentRepository.Remove(u);
                }
            });
            commentRepository.SaveChanges();
        }

        public void DeleteUserPosts(string userId)
        {
            DeleteUserComments(userId);

            var postsId = new List<long>();
            postUserRepository.GetAll().ToList().ForEach(u =>
            {
                var postEntity = postRepository.Get(u.PostId);
                var recipeEntity = recipeRepository.Get(postEntity.RecipeId);
                if (recipeEntity.UserId == userId)
                {
                    postsId.Add(u.PostId);
                }
            });
            
            foreach (var postId in postsId)
            {
                postUserRepository.GetAll().ToList().ForEach(u =>
                {
                    if (u.PostId == postId)
                    {
                        DeletePost(postId);
                    }
                });
            }
        }

        // Work with reports
        public IEnumerable<Report> GetReports()
        {
            var reports = new List<Report>();
            reportUserRepository.GetAll().ToList().ForEach(u =>
            {
                reports.Add(reportRepository.Get(u.ReportId));
            });
            return reports;
        }
        public void MakeReport(Report report, string userId)
        {
            reportRepository.Insert(report);
            reportUserRepository.Insert(new ReportUser()
            {
                ReportId = report.Id,
                UserId = userId,
            });
        }

        public bool CheckReportCommentExist(string userId, long commentId)
        {
            var reportUsers = reportUserRepository.GetAll().ToList();
            foreach(var reportUser in reportUsers)
            {
                if (reportUser.UserId == userId && reportRepository.Get(reportUser.ReportId).CommentId == commentId)
                {
                    return true;
                }
            }
            return false;
        }
        public bool CheckReportPostExist(string userId, long postId)
        {
            var reportUsers = reportUserRepository.GetAll().ToList();
            foreach (var reportUser in reportUsers)
            {
                if (reportUser.UserId == userId && reportRepository.Get(reportUser.ReportId).PostId == postId)
                {
                    return true;
                }
            }
            return false;
        }
        
        public void DeleteReportsFromComment(string targetId, long commentId)
        {
            reportUserRepository.GetAll().ToList().ForEach(u =>
            {
                var report = reportRepository.Get(u.ReportId);
                if (report.TargetId == targetId && report.CommentId == commentId)
                {
                    reportUserRepository.Remove(u);
                    reportRepository.Remove(report);
     
                }
            });
            reportUserRepository.SaveChanges();
            reportRepository.SaveChanges();
        }
        public void DeleteReportsFromPost(string targetId, long postId)
        {
            reportUserRepository.GetAll().ToList().ForEach(u =>
            {
                var report = reportRepository.Get(u.ReportId);
                if (report.TargetId == targetId && report.PostId == postId)
                {
                    reportUserRepository.Remove(u);
                    reportRepository.Remove(report);  
                }
            });
            reportUserRepository.SaveChanges();
            reportRepository.SaveChanges();
        }
    }
}
