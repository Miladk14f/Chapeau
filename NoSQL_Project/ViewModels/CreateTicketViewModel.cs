// Author: Student3
using System.ComponentModel.DataAnnotations;

namespace NoSQL_Project.ViewModels
{
    /// <summary>Binds the Create Ticket form.</summary>
    public class CreateTicketViewModel
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title may not exceed 200 characters.")]
        [Display(Name = "Subject of incident")]
        public string Title { get; set; } = default!;

        [Required(ErrorMessage = "Description is required.")]
        [Display(Name = "Description")]
        public string Description { get; set; } = default!;

        [Required(ErrorMessage = "Type is required.")]
        [Display(Name = "Type of incident")]
        public string Type { get; set; } = default!;

        [Required(ErrorMessage = "Priority is required.")]
        [Display(Name = "Priority")]
        public string Priority { get; set; } = default!;

        /// <summary>Deadline option in days: "7", "14", "28" or "180" (6 months).</summary>
        [Required(ErrorMessage = "Deadline is required.")]
        [Display(Name = "Deadline / follow up")]
        public string DeadlineOption { get; set; } = "7";

        [Required(ErrorMessage = "Date reported is required.")]
        [Display(Name = "Date / time reported")]
        public DateTime DateReported { get; set; } = DateTime.Now;

        /// <summary>ServiceDesk may set a different reporter. Regular employees always report as themselves.</summary>
        [Display(Name = "Reported by")]
        public string? ReportedByUserId { get; set; }

        /// <summary>ServiceDesk may assign directly on creation.</summary>
        [Display(Name = "Assign to")]
        public string? AssignedToUserId { get; set; }
    }
}
