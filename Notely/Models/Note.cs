using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Notely.Models
{
    public class Note
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Content is required.")]
        [MaxLength(5000, ErrorMessage = "Content cannot exceed 5000 characters.")]
        public string Content { get; set; } = string.Empty;
        [Display(Name = "Make Public")]
        public bool State { get; set; } = false;
        [MaxLength(500)]
        public string? ImagePath { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string UserId{ get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }



    }
}
