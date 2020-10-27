﻿using Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Interfaces
{
    public interface IPostService
    {
        IEnumerable<Post> GetPosts(string UserId);
        IEnumerable<Post> GetPosts();
        Post GetPost(long id);
        void SubscribePost(long id, string userId);
        void InsertPost(Post post, string userId);
        void UpdatePost(Post post);
        void DeletePost(long id);
    }
}