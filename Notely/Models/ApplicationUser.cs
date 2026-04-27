using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Notely.Models
{
    public class ApplicationUser :IdentityUser
    {

        [Key]
        public int Id { get; set; } 
        [Required]
        [MaxLength(100)]
        [Display(Name = "Full Name")]

        public string Fullname { get; set; } = string.Empty;


        [Required]
        [EmailAddress]
        public string Email { get; set; }= string.Empty;

        public DateTime Created_at { get; set; }

        [Required]
        public string Hashed_password { get; set; }

        [MaxLength(300)]
        public string? ProfileImagePath { get; set; }
        public ICollection<Note> Notes { get; set; } = new List<Note>();
    }
}
