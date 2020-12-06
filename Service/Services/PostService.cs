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

        public PostService(IRepository<Post> postRepository,
                           IRepository<PostUser> postUserRepository,
                           IRepository<Recipe> recipeRepository,
                           IRepository<Comment> commentRepository)
        {
            this.postRepository = postRepository;
            this.postUserRepository = postUserRepository;
            this.recipeRepository = recipeRepository;
            this.commentRepository = commentRepository;
        }
        public IEnumerable<Post> GetPosts()
        {
            return postRepository.GetAll().ToList();
        }
        public IEnumerable<Post> GetPosts(string userId)
        {
            var posts = new List<Post>();
            postRepository.GetAll().ToList().ForEach(u =>
            {
                if (recipeRepository.Get(u.RecipeId).UserId == userId){
                    posts.Add(u);
                }
            });
            return posts;
        }
        public IEnumerable<PostUser> GetPostUsers()
        {
            return postUserRepository.GetAll().ToList();
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
        public void UnsubscribeUser(string userId)
        {
            postUserRepository.GetAll().ToList().ForEach(u =>
            {
                if (u.UserId == userId)
                {
                    if (recipeRepository.Get(postRepository.Get(u.PostId).RecipeId).UserId != userId)
                    {
                        postUserRepository.Remove(u);
                    }
                }
            });
            postUserRepository.SaveChanges();
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

        public IEnumerable<Comment> GetComments(long postId)
        {
            var comments = new List<Comment>();
            commentRepository.GetAll().ToList().ForEach(u =>
            {
                if (u.PostId == postId)
                {
                    comments.Add(u);
                }
            });
            return comments;
        }
        public IEnumerable<Comment> GetComments(string userId)
        {
            var comments = new List<Comment>();
            commentRepository.GetAll().ToList().ForEach(u =>
            {
                if (u.UserId == userId)
                {
                    comments.Add(u);
                }
            });
            return comments;
        }
        public Comment GetComment(long id)
        {
            return commentRepository.Get(id);
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
        public bool CommentOwnerCheck(long commentId, string userId)
        {
            var comments = commentRepository.GetAll().ToList();
            foreach (var comment in comments)
            {
                if ((comment.Id == commentId) && (comment.UserId == userId))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
