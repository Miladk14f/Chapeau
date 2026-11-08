// Author: Student1
using System.ComponentModel.DataAnnotations;

namespace NoSQL_Project.ViewModels
{
    /// <summary>Binds the Edit Employee form (ServiceDesk only).</summary>
    public class EditEmployeeViewModel
    {
        public string Id { get; set; } = default!;

        [Required(ErrorMessage = "Full name is required.")]
        [Display(Name = "Full name")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "E-mail is required.")]
        [EmailAddress]
        [Display(Name = "E-mail address")]
        public string Email { get; set; } = default!;

        [Required]
        [Display(Name = "Type of user")]
        public string Role { get; set; } = default!;

        [Required]
        [Display(Name = "Department / branch")]
        public string DepartmentName { get; set; } = default!;

        [Phone]
        [Display(Name = "Phone number")]
        public string? PhoneNumber { get; set; }

        /// <summary>Leave blank to keep existing password.</summary>
        [MinLength(8, ErrorMessage = "New password must be at least 8 characters.")]
        [DataType(DataType.Password)]
        [Display(Name = "New password (leave blank to keep current)")]
        public string? NewPassword { get; set; }
    }
}
