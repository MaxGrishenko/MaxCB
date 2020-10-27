using Data;
using Microsoft.AspNetCore.Identity;
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

        public PostService(IRepository<Post> postRepository, IRepository<PostUser> postUserRepository)
        {
            this.postRepository = postRepository;
            this.postUserRepository = postUserRepository;
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
            Post post = GetPost(id);
            postRepository.Remove(post);
            postRepository.SaveChanges();
        }
    }
}
