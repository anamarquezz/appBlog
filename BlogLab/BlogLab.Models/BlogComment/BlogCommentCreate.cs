using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogLab.Models.BlogComment
{
    public class BlogCommentCreate
    {
        public int BlogCommentId { get; set; }
        public int? ParentBlogCommentId { get; set; }
        public int BlogId { get; set; }

        [Required(ErrorMessage = "Content is required")]
        [MinLength(10, ErrorMessage = "Must be 10-300 charachters")]
        [MaxLength(300, ErrorMessage = "Must be 10-300 charachters")]
        public string Content { get; set; }
    }
}
