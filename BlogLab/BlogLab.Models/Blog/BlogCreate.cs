using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogLab.Models.Blog
{
    public class BlogCreate
    {
        public int BlogId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [MinLength(10, ErrorMessage = "Can be 10-50 characters")]
        [MaxLength(50, ErrorMessage = "Can be 10-50 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required")]
        [MinLength(30, ErrorMessage = "Can be 10-300 characters")]
        [MaxLength(300,ErrorMessage = "Can be 10-300 characters")]
        public string Content { get; set; }
        public int? PhotoId { get; set; }

    }
}
