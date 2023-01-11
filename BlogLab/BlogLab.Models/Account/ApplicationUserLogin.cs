using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogLab.Models.Account
{
    public class ApplicationUserLogin
    {
        [Required(ErrorMessage = "Username is required")]
        [MinLength(5, ErrorMessage = "Must be 5 charachters")]
        [MaxLength(20,ErrorMessage = "Must be 20 charachters")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(10, ErrorMessage = "Must be 5-10 charachters")]
        [MaxLength(50, ErrorMessage = "Must be 10-50 charachters")]
        public string password { get; set; }
        public int MyProperty { get; set; }
    }
}
