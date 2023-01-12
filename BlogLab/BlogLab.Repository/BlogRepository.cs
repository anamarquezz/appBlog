using BlogLab.Models.Blog;
using BlogLab.Models.Photo;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using BlogLab.Models.Account;
using System.Reflection.Metadata;

namespace BlogLab.Repository
{
    public class BlogRepository : IBlogRepository
    {

        private readonly IConfiguration _config;

        public BlogRepository(IConfiguration config)
        {
            _config = config;
        }
        public async Task<int> DeleteAsync(int blogId)
        {
            int affectedRows = 0;

            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                                                
                affectedRows = await connection.ExecuteAsync(
                    "Blog_Delete",
                    new { BlogId = blogId },
                    commandType: CommandType.StoredProcedure);
            }

            return affectedRows;
        }

        public async Task<PageResults<Blog>> GetAllAsunc(BlogPaging blogPaging)
        {
            var results = new PageResults<Blog>();

            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                using (var multi = await connection.QueryMultipleAsync("Blog_GetAll",
                    new
                    {
                        Offset = (blogPaging.Page - 1) * blogPaging.PageSize,
                        Pagesize = blogPaging.PageSize
                    },
                    commandType: CommandType.StoredProcedure))
                {
                    results.Items = multi.Read<Blog>();
                    results.TotalCount = multi.ReadFirst<int>();
                }
            }

            return results;

        }

        public async Task<List<Blog>> GetAllByUserIdAsync(int applicationUserId)
        {
            IEnumerable<Blog> blogs;
            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                blogs = await connection.QueryAsync<Blog>(
                    "Blog_GetByUserId",
                    new { applicationUserId = applicationUserId },
                    commandType: CommandType.StoredProcedure);
            }
            return blogs.ToList();

        }

        public async Task<List<Blog>> GetallFamousAsync()
        {
            IEnumerable<Blog> famousblogs;
            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                famousblogs = await connection.QueryAsync<Blog>(
                "Blog_GetAllFamous",
                    new { },
                    commandType: CommandType.StoredProcedure);
            }
            return famousblogs.ToList();
        }

        public async Task<Blog> GetAsync(int blogId)
        {

            Blog blog;
            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                blog = await connection.QueryFirstOrDefaultAsync<Blog>(
                "Blog_Get",
                    new { BlogId = blogId },
                    commandType: CommandType.StoredProcedure);
            }
            return blog;
        }

        public async Task<Blog> UpsertAsync(BlogCreate blogcreate, int applicationUserId)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("BlogId", typeof(string));
            dataTable.Columns.Add("Title", typeof(string));
            dataTable.Columns.Add("Content", typeof(string));
            dataTable.Columns.Add("PhotoId", typeof(string));

            dataTable.Rows.Add(blogcreate.BlogId, blogcreate.Title, blogcreate.Content, blogcreate.PhotoId);

            int? newBlogId;

            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                newBlogId = await connection.ExecuteScalarAsync<int?>(
                    "Blog_Upsert",
                    new { blog = dataTable.AsTableValuedParameter("dbo.BlogType"), ApplicationUserId = applicationUserId },
                    commandType: CommandType.StoredProcedure
                    );
            }

            newBlogId = newBlogId ?? blogcreate.BlogId;
            Blog blog = await GetAsync(newBlogId.Value);

            return blog;

        }
    }
}
