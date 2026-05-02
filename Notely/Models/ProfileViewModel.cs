using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Notely.Models;

public class ProfileViewModel
{
    [Required]
    [MaxLength(100)]
    [Display(Name = "Full Name")]
    public string Fullname { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    public string? ProfileImagePath { get; set; }

    [NotMapped]
    public IFormFile? ProfileImage { get; set; }
}
