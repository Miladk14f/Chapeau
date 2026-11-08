// Author: Student3
using System.ComponentModel.DataAnnotations;

namespace NoSQL_Project.ViewModels
{
    /// <summary>Binds the Edit Ticket form (ServiceDesk only).</summary>
    public class EditTicketViewModel
    {
        public string Id { get; set; } = default!;

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200)]
        [Display(Name = "Subject of incident")]
        public string Title { get; set; } = default!;

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = default!;

        [Required]
        [Display(Name = "Type of incident")]
        public string Type { get; set; } = default!;

        [Required]
        public string Priority { get; set; } = default!;

        [Required]
        public string Status { get; set; } = default!;

        [Required]
        [Display(Name = "Deadline / follow up")]
        public string DeadlineOption { get; set; } = "7";

        [Required]
        [Display(Name = "Date / time reported")]
        public DateTime DateReported { get; set; }

        [Display(Name = "Reported by")]
        public string? ReportedByUserId { get; set; }

        [Display(Name = "Assign to")]
        public string? AssignedToUserId { get; set; }

        /// <summary>Optional note appended to the embedded audit log on save.</summary>
        [Display(Name = "Change note (optional)")]
        public string? Note { get; set; }

        /// <summary>Deadline stored in DB, shown read-only on the form.</summary>
        public DateTime CurrentDeadline { get; set; }
    }
}
