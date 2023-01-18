﻿using BlogLab.Models.Blog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogLab.Repository
{
    public interface IBlogRepository
    {
        public Task<Blog> UpsertAsync(BlogCreate blogcreate, int applicationUserId);
        public Task<PageResults<Blog>> GetAllAsync(BlogPaging blogPaging);

        public Task<Blog> GetAsync(int blogId);
        public Task<List<Blog>> GetAllByUserIdAsync(int applicationUserId);

        public Task<List<Blog>> GetallFamousAsync();
        public Task<int> DeleteAsync(int blogId);
    }
}
