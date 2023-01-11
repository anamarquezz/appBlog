using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogLab.Models.Account
{
    public class ApplicationUserCreate : ApplicationUserLogin
    {
       
        [MinLength(30, ErrorMessage = "Must be 10-30 charachters")]
        [MaxLength(30, ErrorMessage = "Must be 10-30 charachters")]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [MinLength(30, ErrorMessage = "Can be 30 characters")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

    }
}
