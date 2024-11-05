using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class CreatePageDTO
    {
        public string Title { get; set; } = "New Page";

        [MaxLength(200)]
        [Required]
        public string Slug { get; set; } = Guid.NewGuid().ToString();

        public string Content { get; set; } = string.Empty;
    }
}
