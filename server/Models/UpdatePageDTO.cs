using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class UpdatePageDTO
    {
        public string Title { get; set; } = null!;

        [MaxLength(200)]
        [Required]
        public string Slug { get; set; } = null!;

        public string Content { get; set; } = string.Empty;
    }
}
