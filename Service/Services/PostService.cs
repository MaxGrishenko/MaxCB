using Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Repo;
using Service.Interfaces;
using System;
using System.Collections.Generic;
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
         
        public PostService(IRepository<Post> postRepository, IRepository<PostUser> postUserRepository, IRepository<Recipe> recipeRepository)
        {
            this.postRepository = postRepository;
            this.postUserRepository = postUserRepository;
            this.recipeRepository = recipeRepository;
        }

        public IEnumerable<Post> GetPosts(string userId)
        {
            var posts = new List<Post>();
            postUserRepository.GetAll().ToList().ForEach(u =>
            {
                if (u.UserId == userId)
                {
                    posts.Add(GetPost(u.PostId));
                }
            });
            return posts;
        }
        public IEnumerable<Post> GetPosts()
        {
            return postRepository.GetAll();
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
    }
}
