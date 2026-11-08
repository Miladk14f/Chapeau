// Author: Student3
using System.ComponentModel.DataAnnotations;

namespace NoSQL_Project.ViewModels
{
    /// <summary>Binds the Login form fields.</summary>
    public class LoginViewModel
    {
        [Required(ErrorMessage = "E-mail is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid e-mail address.")]
        [Display(Name = "E-mail / Username")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
