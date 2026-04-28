using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [MaxLength(300)]
        public string? ProfileImagepath { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public ICollection<Note> Notes { get; set; } = new List<Note>();
    }
}
