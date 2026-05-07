using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Notely.Models;


public class ProfileViewModel
{
    [Required(ErrorMessage = "Full name is required")]
    [MaxLength(100)]
    [Display(Name = "Full Name")]
    [StringLength(100)]
     public string FullName { get; set; } = string.Empty;


    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    [Display(Name = "Email Address")]
     public string Email { get; set; } = string.Empty;

    public string? ProfileImagePath { get; set; }

    [Display(Name = "Profile Photo")]
    [NotMapped]
    public IFormFile? ProfileImage { get; set; }

        public DateTime MemberSince { get; set; }

        public List<Note> Notes { get; set; } = new List<Note>();


        public int TotalNotes => Notes.Count;
        public int PublicNotes => Notes.Count(n => n.State);
        public int PrivateNotes => Notes.Count(n => !n.State);
    }

