using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Notely.Models
{
    public class ApplicationUser :IdentityUser
    {
        [Required]
        [MaxLength(100)]
        [Display(Name = "Full Name")]

        public string Fullname { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? ProfileImagePath { get; set; }
        public ICollection<Note> Notes { get; set; } = new List<Note>();
    }
}
