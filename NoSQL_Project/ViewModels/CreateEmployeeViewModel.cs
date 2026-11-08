// Author: Student1
using System.ComponentModel.DataAnnotations;

namespace NoSQL_Project.ViewModels
{
    /// <summary>Binds the Create Employee form (ServiceDesk only).</summary>
    public class CreateEmployeeViewModel
    {
        [Required(ErrorMessage = "Full name is required.")]
        [Display(Name = "Full name")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "E-mail is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid e-mail address.")]
        [Display(Name = "E-mail address")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [DataType(DataType.Password)]
        [Display(Name = "Initial password")]
        public string Password { get; set; } = default!;

        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; } = default!;

        [Required(ErrorMessage = "Role is required.")]
        [Display(Name = "Type of user")]
        public string Role { get; set; } = default!;

        [Required(ErrorMessage = "Department is required.")]
        [Display(Name = "Department / branch")]
        public string DepartmentName { get; set; } = default!;

        [Phone]
        [Display(Name = "Phone number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Send password e-mail to user?")]
        public bool SendPassword { get; set; }
    }
}
